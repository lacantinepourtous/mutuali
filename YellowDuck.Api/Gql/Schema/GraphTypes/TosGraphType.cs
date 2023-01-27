using GraphQL.Conventions;
using System;
using YellowDuck.Api.DbModel.Entities;
using YellowDuck.Api.DbModel.Enums;

namespace YellowDuck.Api.Gql.Schema.GraphTypes
{
    public class TosGraphType
    {
        private readonly TosVersion acceptedVersion;
        public DateTime acceptationDate;
        public string acceptationIpAddress;

        public TosGraphType(string userId, TosVersion acceptedVersion, DateTime acceptationDate, string acceptationIpAddress)
        {
            Id = Id.New<AppUser>(userId);
            this.acceptedVersion = acceptedVersion;
            this.acceptationDate = acceptationDate;
            this.acceptationIpAddress = acceptationIpAddress;
        }

        public Id Id { get; }
        public bool HasAcceptedVersion1 => acceptedVersion >= TosVersion.Version1;
        public bool HasAcceptedLatest => acceptedVersion >= TosVersion.Latest;
    }
}
