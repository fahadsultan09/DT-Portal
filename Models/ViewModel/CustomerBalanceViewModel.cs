namespace Models.ViewModel
{
    public class CustomerBalanceViewModel
    {
        public string DistributorCode { get; set; }
        public string DistributorName { get; set; }
        public string City { get; set; }
        public decimal Balance { get; set; }
        public string DebitCredit { get; set; }
    }
}
