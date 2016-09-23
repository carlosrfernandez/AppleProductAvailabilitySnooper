using System.Collections;
using System.Collections.Generic;

namespace Apple.Snooper
{
    public class EmailConfig
    {
        public string Smtp { get; }
        public string From { get; }
        public string Password { get; }
        public IEnumerable<string> Destinations { get; }

        public EmailConfig(string smtp, string @from, string password, IEnumerable<string> destinations)
        {
            Smtp = smtp;
            From = @from;
            Password = password;
            Destinations = destinations;
        }
    }
}