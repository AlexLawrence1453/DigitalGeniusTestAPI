using Microsoft.Extensions.Options;
using DigitalGeniusTestAPI.Configuration;
using System.Reflection.PortableExecutable;
using System;
using DigitalGeniusTestAPI.DTO;
using DigitalGeniusTestAPI.DTO.TicketMessage;
using System.Text.Json;
using System.Text;

namespace DigitalGeniusTestAPI.Services.GorgiasApi
{
    public class GorgiasApiService : IGorgiasApiService
    {
        private IHttpClientFactory httpClientFactory;
        private readonly Configuration.GorgiasApi apiConfig;
        private readonly JsonSerializerOptions jsonSerializerSettings;

        public GorgiasApiService(IHttpClientFactory httpClientFactory, IOptions<Configuration.GorgiasApi> gorgiasApiOptions, JsonSerializerOptions jsonSerializerSettings)
        {
            this.httpClientFactory = httpClientFactory;
            this.apiConfig = gorgiasApiOptions.Value;
            this.jsonSerializerSettings = jsonSerializerSettings;
        }

        public async Task<HttpResponseMessage> SendApiRequest(string apiMethod, HttpMethod httpMethod, HttpContent jsonRequestBody, string gorgiasQuery, List<Header> headers)
        {
            var gorgiaUriBuilder = new UriBuilder(this.apiConfig.BaseUri + apiMethod);
            gorgiaUriBuilder.Query = gorgiasQuery;

            HttpRequestMessage gorgiasRequest = new HttpRequestMessage
            {
                Content = jsonRequestBody,
                RequestUri = gorgiaUriBuilder.Uri,
                Method = httpMethod,

            };
            foreach (Header header in headers)
            {
                gorgiasRequest.Headers.Add(header.HeaderDescriptor, header.HeaderValue);
            }

            using var client = this.httpClientFactory.CreateClient("GorgiasAPI");
            return await client.SendAsync(gorgiasRequest).ConfigureAwait(continueOnCapturedContext: false);
        }
        private static string Base64Encode(string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        public async Task<TicketMessageResponse?> SendTicketMessageUpdate(TicketMessageRequest ticketMessage, string apiKey, int ticketId)
        {
            string authString = Base64Encode(this.apiConfig.AuthUsername + ":" + apiKey);

            List<Header> authHeader = new List<Header>()
                {
                    new Header("Authorization", $"Basic {authString}"),
                };
            var serialisedTicketMessage = JsonSerializer.Serialize(ticketMessage, this.jsonSerializerSettings);
            var httpContent = new StringContent(serialisedTicketMessage, Encoding.UTF8, "application/json");

            var apiResponse = await this.SendApiRequest(
                $"api/tickets/{ticketId}/messages",
                HttpMethod.Post,
                httpContent,
                string.Empty,
                authHeader
                );
            if (apiResponse.IsSuccessStatusCode)
            {
                return await apiResponse.Content.ReadFromJsonAsync<TicketMessageResponse>();
            }
            return null;
        }
    }
}
