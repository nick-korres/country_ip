using Microsoft.AspNetCore.Mvc;
using CountryIP.Repositories;
using CountryIP.Entities;
using Microsoft.Extensions.Caching.Memory;
using CountryIP.Responses;
using CountryIP.Helpers;

namespace CountryIP.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CountriesController: ControllerBase
    {
        private readonly CountryRepository countryRepository;

        private readonly IpAddressesRepository ipAddressesRepository;

        public static IMemoryCache _cache ;   
        
        public CountriesController(IMemoryCache? cache){
            this.countryRepository = new CountryRepository(new test_dbContext());
            this.ipAddressesRepository = new IpAddressesRepository(new test_dbContext());
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }


        
        [HttpGet("/{ip}")]
        public async Task<CountryByIpResponse> GetCountryByIP(string ip)
        {

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromSeconds(60))
                .SetAbsoluteExpiration(TimeSpan.FromSeconds(3600))
                .SetPriority(CacheItemPriority.Normal);
            var key = "Countries:"+ip;
            Console.WriteLine("Trying to fetch the country from cache.");
            if (_cache.TryGetValue(key, out Country country))
            {
                Console.WriteLine("Country found in cache.");
            }else{
                Console.WriteLine("Country not found in cache. Fetching from database.");
                country =  countryRepository.GetCountryByIP(ip);
                _cache.Set(key, country, cacheEntryOptions);
                
            }
            Console.WriteLine("Returning ... ");
            HttpClient client = new HttpClient();
            string[] response = ( await client.GetStringAsync($"https://ip2c.org/?ip={ip}")).Split(";");
            // System.Console.WriteLine(response);
            // 1;GR;GRC;Greece
            string threeLetterCodeToUpdate = response[2];
            if(threeLetterCodeToUpdate != null)
            {       
                Ipaddress address = ipAddressesRepository.GetIpaddressByIp(ip);
                System.Console.WriteLine(threeLetterCodeToUpdate);
                Country? countryToUpdate = null;
                try
                {
                    countryToUpdate =  countryRepository.GetCountryByThreeLetterCode(threeLetterCodeToUpdate);
                     if(address.CountryId != countryToUpdate.Id)
                    {   
                        System.Console.WriteLine("Country already exists");
                        address.CountryId = countryToUpdate.Id;
                        System.Console.WriteLine("Updating ip");
                        await ipAddressesRepository.UpdateIpAddress(address);
                    }
                }
                catch (System.Exception)
                {
                    Country countryToAdd = new Country()
                    {
                        TwoLetterCode = response[1],
                        ThreeLetterCode = response[2],
                        Name = response[3]
                    };
                    System.Console.WriteLine("Adding New Country");
                    Country newCountry =  await countryRepository.InsertNewCountry(countryToAdd);
                    address.Country= newCountry;
                    address.CountryId = newCountry.Id;
                    System.Console.WriteLine("Updating ip");
                    await ipAddressesRepository.UpdateIpAddress(address);
                    _cache.Set(key, newCountry, cacheEntryOptions); 
                }                
   
            }
            return new CountryByIpResponse()
            {
                Name = country.Name,
                ThreeLetterCode = country.ThreeLetterCode,
                TwoLetterCode= country.TwoLetterCode
            };

        }

        [HttpPost("/reports")]
        public async Task<IEnumerable<CountryReportResponse>?> GetCountryReports(string[]? TwoLetterList)
        {
            
            return await SqlCommandHelper.QueryCountryReports(TwoLetterList);
        }
    }
}
