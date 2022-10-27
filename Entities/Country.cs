

namespace CountryIP.Entities
{
    public partial class Country
    {
        
        public Country()
        {
            Ipaddresses = new HashSet<Ipaddress>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string TwoLetterCode { get; set; } = null!;
        public string ThreeLetterCode { get; set; } = null!;
        public DateTime CreatedAt { get; set; }

        public virtual ICollection<Ipaddress> Ipaddresses { get; set; } 
    }
}
