using GraphQL.Conventions;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using YellowDuck.Api.Constants;
using YellowDuck.Api.DbModel.Entities.Alerts;
using YellowDuck.Api.Extensions;
using YellowDuck.Api.Gql.Interfaces;
using YellowDuck.Api.Gql.Schema.GraphTypes;

namespace YellowDuck.Api.Authorization.Requirements
{
    public class CanManageAlertRequirement : IAuthorizationRequirement, IDescribedRequirement
    {
        public string Describe()
        {
            return "Current user is creator of the alert";
        }
    }

    public class CanManageAlertRequirementHandler : AuthorizationHandler<CanManageAlertRequirement>
    {
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, CanManageAlertRequirement requirement)
        {
            Id alertId;

            switch (context.Resource)
            {
                case IResolutionContext ctx when ctx.Source is AlertGraphType alertGraphType:
                    alertId = alertGraphType.Id;
                    break;
                case IResolutionContext ctx when ctx.GetInputValue() is IHaveAlertId haveAlertId:
                    alertId = haveAlertId.AlertId;
                    break;
                case Id id when id.IsIdentifierForType<Alert>():
                    alertId = id;
                    break;
                default:
                    return;
            }

            if (context.User.HasClaim(AppClaimTypes.AlertOwner, alertId.ToString()))
            {
                context.Succeed(requirement);
                return;
            }
        }
    }
}
