using CountryIP.Entities;
using System.Net;



namespace CountryIP.Repositories
{
    public class IpAddressesRepository
    {
        private test_dbContext context;

        public IpAddressesRepository(test_dbContext context)
        {
            this.context = context;
        }

      

        public  Ipaddress  GetIpaddressByIp(string Ip)
        {
            Ipaddress? address = context.Ipaddresses.Where(address=>address.Ip==Ip).First();
            return address;
        }

        public List<Ipaddress>  GetIpaddressesBatch(int rowsPerPage ,int page)
        {
            List<Ipaddress> ipaddresses = context.Ipaddresses.Skip(rowsPerPage*page).Take(rowsPerPage).ToList();
            return ipaddresses;
        }


        public async Task<HttpResponseMessage> UpdateIpAddress (Ipaddress newValues)
        {
            Ipaddress existingAddress= await context.Ipaddresses.FindAsync(newValues.Id);
            if(existingAddress != null )
            {
                existingAddress.CountryId = (newValues.CountryId != null) ? newValues.CountryId : existingAddress.CountryId;
                await context.SaveChangesAsync();
            }

            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        
    }
}