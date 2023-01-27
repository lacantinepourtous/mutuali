using YellowDuck.Api.Gql.Interfaces;
using GraphQL.Conventions;
using System.Threading.Tasks;

namespace YellowDuck.Api.Gql.Schema.GraphTypes
{
    [Name("ProfileGraphType")]
    public interface IProfileGraphType
    {
        Task<UserGraphType> User(IAppUserContext ctx);

        Id Id { get; }
        Task<string> FirstName { get; }
        Task<string> LastName { get; }
    }
}