//using Npgsql;

namespace AzUtil.Core
{
    public class IPLocation
    {
        public IPLocation(string city,string region, string country, string countryCode)
        {
            City = city;
            Region = region;
            Country = country;
            CountryCode = countryCode;
        }

        public string City { get; set; }
        public string Region { get; set; }
        public string Country { get; set; }
        public string CountryCode { get; set; }
    }
}
