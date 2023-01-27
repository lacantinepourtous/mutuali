using GraphQL.Language.AST;
using GraphQL.Validation;
using System.Linq;

namespace YellowDuck.Api.Plugins.GraphQL
{
    public class NoIntrospection : IValidationRule
    {
        private static readonly string[] introspectionFields = { "__schema", "__type" };

        public INodeVisitor Validate(ValidationContext context)
        {
            return new EnterLeaveListener(x =>
            {
                x.Match<Field>(field =>
                {
                    if (!introspectionFields.Contains(field.Name)) return;

                    context.ReportError(
                        new ValidationError(
                            context.OriginalQuery,
                            "no_introspection",
                            "Introspection query is not allowed",
                            field
                        )
                    );
                });
            });
        }
    }
}
