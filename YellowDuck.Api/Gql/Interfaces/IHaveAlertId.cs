using GraphQL.Conventions;

namespace YellowDuck.Api.Gql.Interfaces
{
    interface IHaveAlertId
    {
        Id AlertId { get; }
    }
}
