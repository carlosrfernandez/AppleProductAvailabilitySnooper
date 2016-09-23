using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Apple.Snooper
{
    public class AppleHttpClient
    {
        private const string Url = @"http://www.apple.com/es/shop/retail/pickup-message?parts.0={0}&location=08020";

        public async Task<string> CheckiPhoneAvailability(string modelNumber)
        {
            var uriString = string.Format(Url, modelNumber);
            var uri = new Uri(uriString);

            var handler = new HttpClientHandler();
            using (var client = new HttpClient(handler))
            {
                return await client.GetStringAsync(uri);
            }
        }
    }
}