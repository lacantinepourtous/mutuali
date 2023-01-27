using GraphQL.Conventions;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using YellowDuck.Api.Constants;
using YellowDuck.Api.DbModel.Entities.Ads;
using YellowDuck.Api.Extensions;
using YellowDuck.Api.Gql.Interfaces;
using YellowDuck.Api.Gql.Schema.GraphTypes;

namespace YellowDuck.Api.Authorization.Requirements
{
    public class CanManageAdRequirement : IAuthorizationRequirement, IDescribedRequirement
    {
        public string Describe()
        {
            return "Current user is creator of the ad";
        }
    }

    public class CanManageAdRequirementHandler : AuthorizationHandler<CanManageAdRequirement>
    {
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, CanManageAdRequirement requirement)
        {
            Id adId;

            switch (context.Resource)
            {
                case IResolutionContext ctx when ctx.Source is AdGraphType adGraphType:
                    adId = adGraphType.Id;
                    break;
                case IResolutionContext ctx when ctx.GetInputValue() is IHaveAdId haveAdId:
                    adId = haveAdId.AdId;
                    break;
                case Id id when id.IsIdentifierForType<Ad>():
                    adId = id;
                    break;
                default:
                    return;
            }

            if (context.User.HasClaim(AppClaimTypes.AdOwner, adId.ToString()))
            {
                context.Succeed(requirement);
                return;
            }
        }
    }
}
