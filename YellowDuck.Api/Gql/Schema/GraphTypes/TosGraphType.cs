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

        public TosGraphType(AppUser appUser)
        {
            Id = Id.New<AppUser>(appUser.Id);
            this.acceptedVersion = appUser.AcceptedTos;
            this.acceptationDate = appUser.TosAcceptationDate;
        }

        public Id Id { get; }
        public bool HasAcceptedVersion2 => acceptedVersion >= TosVersion.Version2;
        public bool HasAcceptedVersion1 => acceptedVersion >= TosVersion.Version1;
        public bool HasAcceptedLatest => acceptedVersion >= TosVersion.Latest;
        public TosVersion LatestVersionAvailable => TosVersion.Latest;
    }
}
