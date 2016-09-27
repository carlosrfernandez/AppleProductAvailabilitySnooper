using System;
using System.Configuration;
using System.Linq;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Apple.Snooper
{
    class Program
    {
        private static readonly ILog Log = LogManager.GetLogger("MyLogger");
        private static readonly IMailer Mailer = new Mailer();
        
        static void Main()
        {
            var emailFromArg = ReadConfig();
            var url = ConfigurationManager.AppSettings["RequestUrl"];
            var productModels = ConfigurationManager.AppSettings["ProductModels"].Split(';');
            var storeCode = ConfigurationManager.AppSettings["Store"];
            var location = ConfigurationManager.AppSettings["Location"];

            var httpclient = new AppleHttpClient(url);
            
            foreach (var model in productModels)
            {
                CheckProductAvailability(httpclient, model, emailFromArg, storeCode, location);
            }
        }

        private static void CheckProductAvailability(AppleHttpClient httpclient, string model, 
            EmailConfig emailFromArg, string storeCode, string location)
        {
            var jsonString = httpclient.CheckiPhoneAvailability(model, location).Result;
            var json = JsonConvert.DeserializeObject(jsonString) as JObject;

            if (json == null)
            {
                Log.Error($"{DateTime.Now} - Could not read the response data.");
                Console.WriteLine($"{DateTime.Now} - Could not read Request data.");
                return;
            }

            var stores = json["body"]["stores"];
            if (stores == null)
            {
                Log.Error($"{DateTime.Now} - No stores found in the response json.");
                return;
            }

            var ourStore = storeCode != string.Empty
                ? stores.FirstOrDefault(x => x["storeNumber"].ToString() == storeCode)
                : stores.First();

            if (ourStore == null)
            {
                Console.WriteLine("Store not found. ");
                Log.Error($"{DateTime.Now} - Store not found.");
                return;
            }

            var modelDescription = ourStore["partsAvailability"][model]["storePickupProductTitle"].ToString();
            var partsAvailabilityString = ourStore["partsAvailability"][model]["pickupSearchQuote"].ToString();

            if (partsAvailabilityString != Constants.MensajeNoDisponible)
            {
                Log.Info($"{DateTime.Now} - iPhone {modelDescription} available.");
                var emailResult =
                    Mailer.Notify(emailFromArg, modelDescription, ourStore["storeName"].ToString()).Result;

                if (emailResult) return;

                Console.WriteLine("Error sending e-mail.");
                Log.Error($"{DateTime.Now} - Error sending e-mail.");
            }
            else
            {
                // retry later?
                Console.WriteLine($"{DateTime.Now} - iPhone {modelDescription} not available");
                Log.Info($"{DateTime.Now} - iPhone {modelDescription} not available");
            }
        }

        private static EmailConfig ReadConfig()
        {
            var emailServer = ConfigurationManager.AppSettings["SmtpServer"];
            var emailFrom = ConfigurationManager.AppSettings["From"];
            var emailDestinations = ConfigurationManager.AppSettings["Destinations"].Split(';');
            var password = ConfigurationManager.AppSettings["Password"];

            return new EmailConfig(emailServer, emailFrom, password, emailDestinations);
        }
    }
}
