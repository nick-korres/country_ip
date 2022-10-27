using CountryIP.Entities;
using System.Net;



namespace CountryIP.Repositories
{
    public class CountryRepository
    {

        
        private test_dbContext context;

        public CountryRepository(test_dbContext context)
        {
            this.context = context;
        }

        public List<Country>  GetCountries()
        {
            return context.Countries.ToList();
        }

        public  Country  GetCountryByIP(string Ip)
        {
    
            var result =  context.Countries.Join(
                context.Ipaddresses,
                country=>country.Id,
                ipAddress=>ipAddress.CountryId,
                ( country,Ipaddresses)=> new {country = country ,address = Ipaddresses})
                .Where(res=>res.address.Ip == Ip).FirstOrDefault();
            Console.WriteLine(result.country.Name);
            return result.country;
        }


        public  Country  GetCountryByThreeLetterCode(string ThreeLetterCode)
        {
    
            Country  country =  context.Countries.Where(country=>country.ThreeLetterCode == ThreeLetterCode).First();
            return country;
        }

        public async Task<Country>  InsertNewCountry(Country newCountry)
        {
            var countryToInsert = new Country()
            {
                ThreeLetterCode = newCountry.ThreeLetterCode,
                TwoLetterCode = newCountry.TwoLetterCode,
                Name = newCountry.Name
            };
            await context.Countries.AddAsync(countryToInsert);
            await context.SaveChangesAsync();

            return countryToInsert;
        }


        
    }
}