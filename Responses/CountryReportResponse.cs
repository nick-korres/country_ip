namespace CountryIP.Responses
{
    public partial class CountryReportResponse
    {
        public string CountryName { get; set; } = null!;
        public int AddressesCount { get; set; } = 0!;
        public DateTime? LastAddressUpdated { get; set; } = null!;
    }
}
