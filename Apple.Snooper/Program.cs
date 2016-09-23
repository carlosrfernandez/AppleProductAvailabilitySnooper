using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using log4net;
using log4net.Config;
using log4net.Core;
using log4net.Repository.Hierarchy;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Apple.Snooper
{
    class Program
    {
        private static readonly ILog Log = LogManager.GetLogger("MyLogger");
        

        static void Main(string[] args)
        {
            var emailFromArg = ReadConfig();
            var mailClient = new Mailer();
            var httpclient = new AppleHttpClient();
            var iPhoneModels = ConfigurationManager.AppSettings["iPhoneModels"].Split(';');
            foreach (var model in iPhoneModels)
            {
                CheckiPhoneAvailability(httpclient, model, mailClient, emailFromArg);
            }
        }

        private static void CheckiPhoneAvailability(AppleHttpClient httpclient, string model, Mailer mailClient,
            EmailConfig emailFromArg)
        {
            var jsonString = httpclient.CheckiPhoneAvailability(model).Result;
            var json = JsonConvert.DeserializeObject(jsonString) as JObject;

            if (json == null)
            {
                Log.Error("Could not read the response data.");
                Console.WriteLine("Could not read Request data.");
                return;
            }

            var stores = json["body"]["stores"];

            var ourStore = stores.FirstOrDefault(x => x["storeNumber"].ToString() == Constants.StoreCode);

            if (ourStore == null)
            {
                Console.WriteLine("Store not found. ");
                Log.Warn("Store not found. ");
                return;
            }
            var modelDescription = ourStore["partsAvailability"][model]["storePickupProductTitle"].ToString();
            var partsAvailabilityString = ourStore["partsAvailability"][model]["pickupSearchQuote"].ToString();

            if (partsAvailabilityString != Constants.MensajeNoDisponible)
            {
                Log.Info($"iPhone {modelDescription} available.");
                var emailResult = mailClient.Notify(emailFromArg, modelDescription, ourStore["storeName"].ToString()).Result;
                if (!emailResult)
                {
                    //
                    Console.WriteLine("Error sending e-mail.");
                    Log.Error("Error sending e-mail.");
                }

                // TODO send e-mail.
            }
            else
            {
                // retry later?
                Console.WriteLine($"iPhone {modelDescription} not available");
                Log.Info($"iPhone {modelDescription} not available");
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
