using YellowDuck.Api.DbModel.Entities;
using YellowDuck.Api.Extensions;
using YellowDuck.Api.Gql.Interfaces;
using YellowDuck.Api.Gql.Schema.GraphTypes;
using GraphQL.Conventions;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace YellowDuck.Api.Authorization.Requirements
{
    public class CanManageUserRequirement : IAuthorizationRequirement, IDescribedRequirement
    {
        public string Describe()
        {
            return "Current user is Admin, or is the user being accessed.";
        }
    }

    public class CanManageUserRequirementHandler : AuthorizationHandler<CanManageUserRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            CanManageUserRequirement requirement)
        {
            if (context.User.IsAdmin())
            {
                context.Succeed(requirement);
            }
            else
            {
                string userId;

                switch (context.Resource)
                {
                    case IResolutionContext ctx when ctx.Source is UserGraphType userGraphType:
                        userId = userGraphType.Id.IdentifierForType<AppUser>();
                        break;
                    case IResolutionContext ctx when ctx.GetInputValue() is IHaveUserId haveUserId:
                        userId = haveUserId.UserId.IdentifierForType<AppUser>();
                        break;
                    case Id id when id.IsIdentifierForType<AppUser>():
                        userId = id.IdentifierForType<AppUser>();
                        break;
                    default:
                        return Task.CompletedTask;
                }

                if (context.User.GetUserId() == userId)
                {
                    context.Succeed(requirement);
                }
            }

            return Task.CompletedTask;
        }
    }
}