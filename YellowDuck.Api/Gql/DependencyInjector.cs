using GraphQL.Conventions;
using System;
using System.Reflection;

namespace YellowDuck.Api.Gql
{
    public class DependencyInjector : IDependencyInjector
    {
        private readonly IServiceProvider services;

        public DependencyInjector(IServiceProvider services)
        {
            this.services = services;
        }

        public object Resolve(TypeInfo typeInfo)
        {
            return services.GetService(typeInfo);
        }
    }
}