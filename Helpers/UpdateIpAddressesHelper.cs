
using CountryIP.Repositories;
using Microsoft.Extensions.Caching.Memory;
using CountryIP.Entities;

namespace CountryIP.Helpers
{
    public class UpdateIpAddressesHelper
    {
        private  CountryRepository countryRepository;
        private  IpAddressesRepository ipAddressesRepository;
        private IMemoryCache _cache ;   

        public UpdateIpAddressesHelper(IMemoryCache cache)
        {            
            this.countryRepository = new CountryRepository(new test_dbContext());
            this.ipAddressesRepository = new IpAddressesRepository(new test_dbContext());
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        public async Task UpdateIpAddresses()
        {


            List<Ipaddress> ipaddresses= new List<Ipaddress>();
            int batchSize = 100;
            do
            {
                // countryRepository = new CountryRepository(new test_dbContext());
                // ipAddressesRepository = new IpAddressesRepository(new test_dbContext());
                // _cache=memCache;
                int page = 0;
                ipaddresses.AddRange(ipAddressesRepository.GetIpaddressesBatch(batchSize,page));
                System.Console.WriteLine($"---Updating {ipaddresses.Count()} Addresses");
                foreach (Ipaddress address in ipaddresses)
                {
                    HttpClient client = new HttpClient();
                    string[] response = ( await client.GetStringAsync($"https://ip2c.org/?ip={address.Ip}")).Split(";");
                    string threeLetterCodeToUpdate = response[2];
                    var key = "Countries:"+address.Ip;
                    Country? countryToUpdate = null;
                    try
                    {
                        countryToUpdate =  countryRepository.GetCountryByThreeLetterCode(threeLetterCodeToUpdate);
                        if(address.CountryId != countryToUpdate.Id)
                        {   
                            address.CountryId = countryToUpdate.Id;
                            System.Console.WriteLine($"---Updating ip: {address.Ip.ToString()}");
                            await ipAddressesRepository.UpdateIpAddress(address);
                            _cache.Remove(key);   
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
                        System.Console.WriteLine("---Adding New Country");
                        Country newCountry =  await countryRepository.InsertNewCountry(countryToAdd);
                        address.Country= newCountry;
                        address.CountryId = newCountry.Id;
                        System.Console.WriteLine($"---Updating ip: {address.Ip.ToString()}");
                        await ipAddressesRepository.UpdateIpAddress(address);
                        _cache.Remove(key);       
                    }
                             
                }
 
                page++;
            } while (ipaddresses.Count() > batchSize);
            
            System.Console.WriteLine("Task Ended"); 
        }

    }
}