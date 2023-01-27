using System.Collections.Generic;

namespace YellowDuck.Api.Requests.Queries
{
    public interface IIdListQuery<TId>
    {
        IEnumerable<TId> Ids { get; set; }
    }

    public interface IHaveGroup<TGroup>
    {
        TGroup Group { get; set; }
    }
}