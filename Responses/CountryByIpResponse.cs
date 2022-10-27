namespace CountryIP.Responses
{
    public partial class CountryByIpResponse
    {
        public string Name { get; set; } = null!;
        public string TwoLetterCode { get; set; } = null!;
        public string ThreeLetterCode { get; set; } = null!;
    }
}
