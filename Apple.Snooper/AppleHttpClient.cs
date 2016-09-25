using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Apple.Snooper
{
    public class AppleHttpClient
    {
        private readonly string _url;
        public AppleHttpClient(string url)
        {
            _url = url;
        }

        public async Task<string> CheckiPhoneAvailability(string modelNumber, string location)
        {
            var uriString = string.Format(_url, modelNumber, location);
            var uri = new Uri(uriString);

            var handler = new HttpClientHandler();
            using (var client = new HttpClient(handler))
            {
                return await client.GetStringAsync(uri);
            }
        }
    }
}