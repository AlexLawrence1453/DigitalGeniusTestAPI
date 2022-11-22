using DigitalGeniusTestAPI.DTO;
using DigitalGeniusTestAPI.DTO.TicketMessage;

namespace DigitalGeniusTestAPI.Services.GorgiasApi
{
    public interface IGorgiasApiService
    {
        Task<HttpResponseMessage> SendApiRequest(string apiMethod, HttpMethod httpMethod, HttpContent jsonRequestBody, string gorgiasQuery, List<Header> headers);
        Task<TicketMessageResponse?> SendTicketMessageUpdate(TicketMessageRequest ticketMessage, string apiKey, int ticketId);
    }
}