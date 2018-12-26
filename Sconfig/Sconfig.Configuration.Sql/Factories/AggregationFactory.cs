using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sconfig.Contracts.Aggregation.Enums;
using Sconfig.Contracts.Aggregation.Reads;
using Sconfig.Interfaces.Factories;
using Sconfig.Interfaces.Models;
using Sconfig.Interfaces.Repositories;

namespace Sconfig.Configuration.Sql.Factories
{
    public class AggregationFactory : IAggregationFactory
    {
        private readonly IConfigurationGroupRepository _configurationGroupRepository;
        private readonly IConfigurationItemRepository _configurationItemRepository;

        public AggregationFactory(IConfigurationGroupRepository configurationGroupRepository, IConfigurationItemRepository configurationItemRepository)
        {
            _configurationGroupRepository = configurationGroupRepository;
            _configurationItemRepository = configurationItemRepository;
        }

        protected virtual IEnumerable<BaseConfigurationAggregationEntity> Normalize(IEnumerable<IConfigurationItemModel> items, IEnumerable<IConfigurationGroupModel> groups)
        {
            var entities = new List<BaseConfigurationAggregationEntity>();
            var groupedConfigurationItemsWithTransformations = items.GroupBy(c => new { c.Name, c.ParentId, c.ProjectId, c.ApplicationId });

            foreach (var groupedItems in groupedConfigurationItemsWithTransformations)
                entities.Add(MapTransformable(groupedItems.ToList()));

            foreach (var group in groups)
                entities.Add(Map(group));

            return entities;
        }

        protected virtual ConfigurationItemAggregationEntity MapTransformable(IEnumerable<IConfigurationItemModel> items)
        {
            ConfigurationItemAggregationEntity entity = null;
            foreach (var transformation in items)
            {
                if (entity == null)
                {
                    entity = new ConfigurationItemAggregationEntity()
                    {
                        Name = transformation.Name,
                        ParentId = transformation.ParentId,
                        SortingIndex = transformation.SortingIndex,
                        Type = AggregationEntityType.Item,
                        ProjectId = transformation.ProjectId,
                        ApplicationId = transformation.ApplicationId,
                        Values = new List<AggregationTransformableEntityValue>()
                    };
                }

                entity.Values.Add(new AggregationTransformableEntityValue()
                {
                    Id = transformation.Id,
                    EnvironmentId = transformation.EnvironmentId,
                    Value = transformation.Value
                });
            }

            return entity;
        }

        protected virtual ConfigurationGroupAggregationEntity Map(IConfigurationGroupModel group)
        {
            return new ConfigurationGroupAggregationEntity()
            {
                Name = group.Name,
                ParentId = group.ParentId,
                SortingIndex = group.SortingIndex,
                ApplicationId = group.ApplicationId,
                ProjectId = group.ProjectId,
                Type = AggregationEntityType.Group,
                Id = group.Id
            };
        }

        protected virtual IEnumerable<BaseConfigurationAggregationEntity> Group(IEnumerable<BaseConfigurationAggregationEntity> entities)
        {
            var groupEntities = entities.Where(c => c is BaseConfigurationGroupAggregationEntity).Cast<BaseConfigurationGroupAggregationEntity>();
            foreach (var item in entities)
            {
                if (String.IsNullOrWhiteSpace(item.ParentId))
                    continue;

                var parent = groupEntities.First(c => c.Id == item.ParentId);
                if (parent.Children == null)
                    parent.Children = new List<BaseConfigurationAggregationEntity>();

                parent.Children.Add(item);
            }

            return entities.Where(c => String.IsNullOrWhiteSpace(c.ParentId));
        }

        protected virtual void Sort(IEnumerable<BaseConfigurationAggregationEntity> entities)
        {
            // sort top level
            var sortedEntities = entities.OrderBy(c => String.IsNullOrWhiteSpace(c.ApplicationId))
                .ThenBy(c => c.SortingIndex);

            // sort top level group children
            foreach (var topLevelGroup in sortedEntities.Where(c => c is BaseConfigurationGroupAggregationEntity).Cast<BaseConfigurationGroupAggregationEntity>())
                SortRecursivly(topLevelGroup);
        }

        protected virtual void SortRecursivly(BaseConfigurationGroupAggregationEntity group)
        {
            if (group.Children == null || !group.Children.Any())
                return;

            group.Children = group.Children.OrderBy(c => c.SortingIndex).ToList();
            foreach (var childGroup in group.Children.Where(c => c is BaseConfigurationGroupAggregationEntity).Cast<BaseConfigurationGroupAggregationEntity>())
                SortRecursivly(childGroup);
        }

        protected virtual AggregationsTreeContract Aggregate(IEnumerable<IConfigurationGroupModel> groups, IEnumerable<IConfigurationItemModel> items)
        {
            var entities = Normalize(items, groups);
            entities = Group(entities);
            Sort(entities);

            return new AggregationsTreeContract()
            {
                Configurations = entities.Where(c => String.IsNullOrWhiteSpace(c.ParentId))
            };
        }

        public virtual async Task<AggregationsTreeContract> InitAggregationsTreeForApplication(string projectId, string applicationId)
        {
            var groups = new List<IConfigurationGroupModel>();
            var items = new List<IConfigurationItemModel>();

            groups.AddRange(await _configurationGroupRepository.GetByApplication(projectId, applicationId));
            groups.AddRange(await _configurationGroupRepository.GetByProject(projectId));

            items.AddRange(await _configurationItemRepository.GetByApplication(projectId, applicationId));
            items.AddRange(await _configurationItemRepository.GetByProject(projectId));

            return Aggregate(groups, items);
        }

        public virtual async Task<AggregationsTreeContract> InitAggregationsTreeForProject(string projectId)
        {
            var configurationGroups = await _configurationGroupRepository.GetByProject(projectId);
            var configurationItems = await _configurationItemRepository.GetByProject(projectId);
            return Aggregate(configurationGroups, configurationItems);
        }
    }
}
