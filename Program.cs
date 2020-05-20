using System;
using RestSharp;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Threading;

namespace ConsumeAPIData
{
    class Program
    {
        /// <summary>
        /// JObject will be used to access the information from JSON result
        /// </summary>
        public static JObject data;

        /// <summary>
        /// Function to be used to collect a list of domain names that match specific criteria.
        /// </summary>
        /// <param name="stateCode"></param>
        /// <param name="exceptString"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="api"></param>
        /// <returns></returns>
        public static string GetReverseWhoIs(string stateCode, string exceptString, string startDate, string endDate, string api)
        {
            var client = new RestClient("https://reverse-whois.whoisxmlapi.com/api/v2?createdDateFrom="+startDate+"&createdDateTo="+endDate);
            var request = new RestRequest(Method.POST);
            request.AddHeader("X-Authentication-Token", api);
            request.AddHeader("accept", "application/json");
            request.AddHeader("content-type", "application/json");
            request.AddParameter("application/json", "{\"basicSearchTerms\":{" +
                "\"include\": [\""+stateCode+"\", \"city\"], \"exclude\":[\"This domain is for sale\", \""+exceptString+"\",\"MISSING_WHOIS_DATA\", " +
                "\"REDACTED FOR PRIVACY\"]}, \"searchType\": \"current\", " +
                "\"mode\": \"purchase\", \"apiKey\": \"" + api + "\", " +
                "\"responseFormat\": \"json\"}", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);

            return response.Content;
        }

        public static string GetWhoIs(string domain, string api)
        {
            var client = new RestClient("https://www.whoisxmlapi.com/whoisserver/WhoisService?apiKey="+api+"&domainName="+domain+"&outputFormat=JSON");
            var request = new RestRequest(Method.POST);
            request.AddHeader("X-Authentication-Token", api);
            request.AddHeader("accept", "application/json");
            request.AddHeader("content-type", "application/json");
            IRestResponse response = client.Execute(request);

            return response.Content;
        }

        public static void getDetails(string contact, JObject data)
        {
            //collect domain details from the JSON data
            if (contact.ToLower() == "registrant")
            {
                //get registrant info
                Data.validateInfo("reg", data);
            }
            else if (contact.ToLower() == "administrativecontact")
            {
                //get administrativeContact info
                Data.validateInfo("admin", data);
            }
            else
            {
                //get tecnicalContact info
                Data.validateInfo("tech", data);
            }
        }

        /// <summary>
        /// Function to be used to determine if date entered is valid
        /// </summary>
        /// <param name="dateString"></param>
        /// <returns></returns>
        public static bool ValidateDate(string dateString)
        {
            DateTime d;
            bool chValidity = DateTime.TryParseExact(
               dateString,
               "yyyy-MM-dd",
               CultureInfo.InvariantCulture,
               DateTimeStyles.None,
               out d);
            return chValidity;
        } 
        static void Main(string[] args)
        {
            Console.WriteLine("Enter a state code to receive domains for (eg. TX || AZ): ");
            string stateCode = Console.ReadLine().ToUpper();
            string startdate;
            string enddate;
            while (true) 
            {
                Console.WriteLine("Enter start date of domain creation (FORMAT - YYYY/MM/DD): ");
                startdate = Console.ReadLine();
                bool valid1 = ValidateDate(startdate);

                Console.WriteLine("Enter end date of domain creation (FORMAT - YYYY/MM/DD): ");
                enddate = Console.ReadLine();
                bool valid2 = ValidateDate(enddate);
                if (valid1 && valid2) { break; } else { Console.WriteLine("\nInvalid date\n"); continue;}
            }
            Console.WriteLine("Enter any information you want to exclude from domain results (e.g. gandi.net): ");
            string exceptString = Console.ReadLine();

            var domainNamesJSON = GetReverseWhoIs(stateCode, exceptString, startdate, enddate, "at_2DKxP889BrGuanjVs05QhGvHEpoPo");
            //api key entered at last parameter

            var domainNames = JObject.Parse(domainNamesJSON);
            Console.WriteLine(domainNames);
            
            Console.WriteLine("Press any key to read domains information");
            Console.ReadKey();

            for(int i = 0; i< Int32.Parse(domainNames["domainsCount"].ToString()); i++)
            {
                domain.Url = domainNames["domainsList"][i].ToString();
               
                Console.WriteLine("DOMAIN: " + (i + 1).ToString() +
                    "     Domain address: "+domain.Url+
                    "\n----------------------------------------------------");
                var jsonData = GetWhoIs(domainNames["domainsList"][i].ToString(),
                    "at_9yGCNNsCBn3jcUi6RoUAzxdf1tCUJ");
                //api key entered at last parameter

                data = JObject.Parse(jsonData);
                try
                {
                    domain.createDate = data["WhoisRecord"]["createdDate"].ToString();
                    Console.WriteLine("Domain create Date: "+domain.createDate);
                }
                catch(Exception)
                {
                    try
                    {
                        domain.createDate = data["WhoisRecord"]["registryData"]["createdDate"].ToString();
                        Console.WriteLine("Domain create Date: " + domain.createDate);
                    }
                    catch
                    {
                        
                    }
                }

                if (!Data.gotInfo)
                {
                    getDetails("registrant", data);
                    Thread.Sleep(500);
                }
                if (!Data.gotInfo)
                {
                    getDetails("administrativecontact", data);
                    Thread.Sleep(500);
                }
                if (!Data.gotInfo)
                {
                    getDetails("", data);
                }
                
                //any first parameter other than administrativeContact OR registrant 
                //will result in the function searching for technicalContact information

                Console.WriteLine();
                Data.gotInfo = false;

            }
            Console.WriteLine("Total rows created for domains: "+Data.insertCount.ToString());

            
        }
    }

}
