using GraphQL.Conventions;
using System.Reflection;
using YellowDuck.Api.Plugins.GraphQL;
using GraphQL.NodaTime;
using NodaTime;

namespace YellowDuck.Api.Gql
{
    public static class GraphQLEngineFactory
    {
        public static GraphQLEngine Create()
        {
            var engine = GraphQLEngine.New()
                .RegisterScalarType<LocalDate, LocalDateGraphType>()
                .RegisterScalarType<OffsetDateTime, RfcOffsetDateTimeGraphType>()
                .WithQueryAndMutation<Schema.Query, Schema.Mutation>()
                .BuildSchema();
            // Gros hack poche:
            // J'ai des problèmes d'exécution concurrente qui cause des erreurs avec Entity Framework.
            // Comme on est sur la fin du budget j'ai pas vraiment le temps d'investiguer pour trouver une solution clean.
            // Ce hack injecte un DocumentExecuter custom qui désactive la parallélisation.
            // Et comme GraphQL Conventions ne permet pas de configurer ça, je dois le faire par réflection.
            typeof(GraphQLEngine)
                .GetField("_documentExecutor", BindingFlags.NonPublic | BindingFlags.Instance)
                .SetValue(engine, new SerialDocumentExecuter());
            return engine;
        }
    }
}
