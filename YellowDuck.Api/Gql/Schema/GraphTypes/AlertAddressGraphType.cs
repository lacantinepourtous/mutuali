using GraphQL.Conventions;
using System.Threading.Tasks;
using YellowDuck.Api.DbModel.Entities.Alerts;
using YellowDuck.Api.Extensions;
using YellowDuck.Api.Gql.Interfaces;

namespace YellowDuck.Api.Gql.Schema.GraphTypes
{
    public class AlertAddressGraphType : LazyGraphType<AlertAddress>
    {
        public AlertAddressGraphType(IAppUserContext ctx, long id) : base(() => ctx.LoadAlertAddress(id))
        {
            Id = Id.New<AlertAddress>(id);
        }

        public AlertAddressGraphType(AlertAddress address) : base(address)
        {
            Id = address.GetIdentifier();
        }

        public Id Id { get; }

        public Task<string> StreetNumber => WithData(x => x.StreetNumber);
        public Task<string> Route => WithData(x => x.Route);
        public Task<string> Locality => WithData(x => x.Locality);
        public Task<string> PostalCode => WithData(x => x.PostalCode);
        public Task<double> Latitude => WithData(x => x.Latitude);
        public Task<double> Longitude => WithData(x => x.Longitude);
        public Task<string> Neighborhood => WithData(x => x.Neighborhood);
        public Task<string> Sublocality => WithData(x => x.Sublocality);
    }
}
