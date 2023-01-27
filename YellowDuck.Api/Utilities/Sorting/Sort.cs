using GraphQL.Conventions;

namespace YellowDuck.Api.Utilities.Sorting
{
    [InputType]
    public class Sort<TSortField> where TSortField : struct
    {
        public TSortField Field { get; set; }
        public SortOrder Order { get; set; }
    }
}