using System.Collections.Generic;

namespace AzUtil.Core
{
    //https://unctad.org/page/data-protection-and-privacy-legislation-worldwide
    public static class World
    {
        public static Country Default => new Country(null, OptIn.Single, OptIn.Single, true, true);
        public static Dictionary<string, Country> Countries =>
            new Dictionary<string, Country>()
            {
                //Very stringent
                { "ch", new Country("Switzerland",OptIn.Double, OptIn.Double) },
                { "de", new Country("Germany",OptIn.Double, OptIn.Double) },
                
                //Default
                { "au", new Country("Australia") },
                { "nz", new Country("New Zealand") },
                { "ca", new Country("Canada") },
                { "uk", new Country("UK") },
                
                //US Like Requirement
                { "us", new Country("USA",OptIn.PreTick, OptIn.No) },
                { "sg", new Country("Singapore",OptIn.PreTick, OptIn.No) }, //Must mention <ADV> in Email Subject
                { "my", new Country("Malaysia",OptIn.PreTick, OptIn.No) },
                
                //No Requirements at all
                { "ae", new Country("UAE",OptIn.No, OptIn.No,false,false) },
                { "bn", new Country("Brunei",OptIn.No, OptIn.No,false,false) },
                { "lk", new Country("Sri Lanka",OptIn.No, OptIn.No,false,false) },
                { "bd", new Country("Bangladesh",OptIn.No, OptIn.No,false,false) },
                { "cu", new Country("Cuba",OptIn.No, OptIn.No,false,false) },
                { "gt", new Country("Guatemala",OptIn.No, OptIn.No,false,false) },
                { "bz", new Country("Belize",OptIn.No, OptIn.No,false,false) },
                { "ht", new Country("Haiti",OptIn.No, OptIn.No,false,false) },
                { "gw", new Country("Guinea Bisau",OptIn.No, OptIn.No,false,false) },
                { "sl", new Country("Sierra Leone",OptIn.No, OptIn.No,false,false) },
                { "lr", new Country("Liberia",OptIn.No, OptIn.No,false,false) },
                { "cm", new Country("Cameroon",OptIn.No, OptIn.No,false,false) },
                { "cf", new Country("Central African Republic",OptIn.No, OptIn.No,false,false) },
                { "sd", new Country("Sudan",OptIn.No, OptIn.No,false,false) },
                { "er", new Country("Erithea",OptIn.No, OptIn.No,false,false) },
                { "et", new Country("Ethiopia",OptIn.No, OptIn.No,false,false) },
                { "bi", new Country("Burundi",OptIn.No, OptIn.No,false,false) },
                { "so", new Country("Somalia",OptIn.No, OptIn.No,false,false) },
                { "cd", new Country("Congo",OptIn.No, OptIn.No,false,false) },
                { "cg", new Country("Congo",OptIn.No, OptIn.No,false,false) },
                { "mz", new Country("Mozambique",OptIn.No, OptIn.No,false,false) },
                { "sy", new Country("Syria",OptIn.No, OptIn.No,false,false) },
                { "af", new Country("Afghanistan",OptIn.No, OptIn.No,false,false) },
                { "kh", new Country("Cambodia",OptIn.No, OptIn.No,false,false) },
                { "tl", new Country("Timor-Leste",OptIn.No, OptIn.No,false,false) },
                { "pg", new Country("PNG",OptIn.No, OptIn.No,false,false) },
            };
    }
    
    public class Country
    {
        public Country(string countryName)
        {
            CountryName = countryName;
            SMS = World.Default.SMS;
            Email = World.Default.Email;
            SMSOptOut = World.Default.SMSOptOut;
            EmailOptOut = World.Default.EmailOptOut;
        }
        public Country(string countryName, OptIn sms, OptIn email, bool smsOptOut = true, bool emailOptOut = true)
        {
            CountryName = countryName;
            SMS = sms;
            Email = email;
            SMSOptOut = smsOptOut;
            EmailOptOut = emailOptOut;
        }
    
        public string CountryName { get; set; }
        public OptIn SMS { get; set; }
        public OptIn Email { get; set; }
        public bool SMSOptOut { get; set; } = true;
        public bool EmailOptOut { get; set; } = true;
    }
    
    public enum OptIn
    {
        No=0, 
        PreTick=1, 
        Single = 2,
        Double=3
    }
}