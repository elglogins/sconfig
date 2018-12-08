using System;

namespace Sconfig.Exceptions
{
    public class ValidationCodeException : Exception
    {
        public ValidationCodeException()
        {
        }

        public ValidationCodeException(Enum code)
            : base(code.ToString())
        {
        }

        public ValidationCodeException(Enum code, Exception inner)
            : base(code.ToString(), inner)
        {
        }
    }
}
