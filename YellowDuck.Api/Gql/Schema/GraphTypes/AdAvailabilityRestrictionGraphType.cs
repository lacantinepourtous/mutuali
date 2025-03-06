using GraphQL.Conventions;
using System;
using System.Threading.Tasks;
using YellowDuck.Api.DbModel.Entities.Ads;
using YellowDuck.Api.Extensions;

namespace YellowDuck.Api.Gql.Schema.GraphTypes
{
    public class AdAvailabilityRestrictionGraphType : LazyGraphType<AdAvailabilityRestriction>
    {

        public AdAvailabilityRestrictionGraphType(AdAvailabilityRestriction availabilityRestriction) : base(availabilityRestriction)
        {
            Id = availabilityRestriction.GetIdentifier();
        }

        public Id Id { get; }

        public Task<bool> Day => WithData(x => x.Day);
        public Task<bool> Evening => WithData(x => x.Evening);
        public Task<DateTime> StartDate => WithData(x => x.StartDate);
    }
}
