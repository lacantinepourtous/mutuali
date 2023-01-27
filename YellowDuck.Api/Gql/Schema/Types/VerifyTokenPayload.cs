using YellowDuck.Api.Gql.Schema.Enums;
using YellowDuck.Api.Gql.Schema.GraphTypes;

namespace YellowDuck.Api.Gql.Schema.Types
{
    public class VerifyTokenPayload
    {
        public TokenStatus Status { get; set; }
        public UserGraphType User { get; set; }
    }
}
