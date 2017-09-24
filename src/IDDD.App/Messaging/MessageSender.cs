using IDDD.Domain.Messaging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace IDDD.App.Messaging
{

    public class MessageSender : IEmailSender
    {

        private readonly EmailSettings _emailSettings;

        public MessageSender(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }
        public async Task SendEmailAsync(string email, string subject, string message)
        {

            using (var client = new HttpClient { BaseAddress = new Uri(_emailSettings.BaseUri) })
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                    Convert.ToBase64String(Encoding.ASCII.GetBytes(_emailSettings.ApiKey)));

                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("from", _emailSettings.From),
                    new KeyValuePair<string, string>("to", email),
                    new KeyValuePair<string, string>("subject", subject),
                    new KeyValuePair<string, string>("text", message)
                });

                await client.PostAsync(_emailSettings.RequestUri, content).ConfigureAwait(false);
            }
            
        }
    }
}
