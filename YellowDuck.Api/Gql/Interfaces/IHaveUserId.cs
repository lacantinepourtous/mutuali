using GraphQL.Conventions;

namespace YellowDuck.Api.Gql.Interfaces
{
    public interface IHaveUserId
    {
        Id UserId { get; }
    }
}