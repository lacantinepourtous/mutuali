using System;

namespace YellowDuck.Api.Plugins.MediatR
{
    public abstract class RequestValidationException : Exception
    {
        protected RequestValidationException() { }
        protected RequestValidationException(string message) : base(message) { }
    }
}