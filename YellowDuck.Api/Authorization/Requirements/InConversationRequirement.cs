using GraphQL.Conventions;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using YellowDuck.Api.DbModel;
using YellowDuck.Api.DbModel.Entities;
using YellowDuck.Api.Extensions;
using YellowDuck.Api.Gql.Schema.GraphTypes;

namespace YellowDuck.Api.Authorization.Requirements
{
    public class InConversationRequirement : IAuthorizationRequirement, IDescribedRequirement
    {
        public string Describe()
        {
            return "Current user has access to conversation";
        }
    }

    public class InConversationRequirementHandler : AuthorizationHandler<InConversationRequirement>
    {
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, InConversationRequirement requirement)
        {
            Id userId;

            switch (context.Resource)
            {
                case IResolutionContext ctx when ctx.Source is UserGraphType adGraphType:
                    userId = adGraphType.Id;
                    break;
                case IResolutionContext ctx when ctx.GetInputValue() is  IHaveStringIdentifier haveStringIdentifier:
                    userId = haveStringIdentifier.Id;
                    break;
                case Id id when id.IsIdentifierForType<AppUser>():
                    userId = id;
                    break;
                default:
                    return;
            }

            if (userId.IdentifierForType<AppUser>() == context.User.GetUserId())
            {
                context.Succeed(requirement);
            }
        }
    }
}
