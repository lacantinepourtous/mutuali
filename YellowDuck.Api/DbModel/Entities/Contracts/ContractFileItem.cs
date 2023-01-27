namespace YellowDuck.Api.DbModel.Entities.Contracts
{
    public class ContractFileItem
    {
        public long Id { get; set; }
        public long ContractId { get; set; }

        public string FileId { get; set; }
    }
}
