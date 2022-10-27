

using System.Data.SqlClient;
using CountryIP.Responses;

namespace CountryIP.Helpers 
{
    public class SqlCommandHelper
    {
        private const string connectionString = "Server=localhost,1433;Database=test_db;user=sa;password=R5gU85VTxQt6txN6";
        private static async Task<List<Dictionary<string,object>>>  RunQueryStringSelect(string querySting)
        {

            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand command = new SqlCommand(querySting,connection);
            connection.Open();
            SqlDataReader? reader = await command.ExecuteReaderAsync();
            List<Dictionary<string,object>>rows = new List<Dictionary<string,object>>();
            int i=0;
            while (reader.Read())
            {
                Dictionary<string,object>columns = new Dictionary<string,object>();
                for(int j=0;j<reader.FieldCount;j++)
                {
                    string ColumnName =reader.GetName(j);
                    System.Console.WriteLine(ColumnName);
                    System.Console.WriteLine(reader[j]);

                    columns.Add(ColumnName,reader[j]);
                }
                rows.Add(columns);
                i++;
            }

            return rows;
        }

        public static async Task<IEnumerable<CountryReportResponse>?> QueryCountryReports(string[]? TwoLetterList)
        {
            string QueryString = @"SELECT 
                DISTINCT (c.Name) as CountryName,
                (SELECT COUNT(*) FROM IPAddresses i2 WHERE i2.CountryId = i.CountryId) as AddressesCount,
                (SELECT MAX(i3.UpdatedAt) FROM IPAddresses i3 WHERE i3.CountryId = c.Id  ) as LastAddressUpdated
                FROM Countries c 
                LEFT JOIN IPAddresses i on c.Id = i.CountryId 
                WHERE c.TwoLetterCode ";

            if(TwoLetterList == null || TwoLetterList.Count() <=0)
            {
                QueryString +="IS NOT NULL ;";
            }else
            {
                string CountryCodes = "'"+String.Join("','",TwoLetterList)+"'";
                QueryString +=$"IN({CountryCodes}) ;" ;
            }

            List<Dictionary<string,object>> response = await RunQueryStringSelect(QueryString);
            DateTime temp;
            return response.Select(r => new CountryReportResponse()
                {
                    CountryName = r["CountryName"].ToString(),
                    AddressesCount = (int)(r["AddressesCount"]),
                    LastAddressUpdated =  DateTime.TryParse(r["LastAddressUpdated"].ToString(),out temp) ? temp : null
                } 
            );
            

        }
    }
}
