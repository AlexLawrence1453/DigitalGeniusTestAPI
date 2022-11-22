using DigitalGeniusTestAPI.DTO.AutomaticEmailReply;
using Microsoft.AspNetCore.Mvc;
using DigitalGeniusTestAPI.Services.GorgiasApi;
using DigitalGeniusTestAPI.DTO.TicketMessage;

namespace DigitalGeniusTestAPI.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class GorgiasController : ControllerBase
    {

        private readonly ILogger<GorgiasController> _logger;
        private IGorgiasApiService gorgiasApiService;
        public GorgiasController(ILogger<GorgiasController> logger, IGorgiasApiService gorgiasApiService)
        {
            _logger = logger;
            this.gorgiasApiService = gorgiasApiService;
        }

        [HttpPost]
        public async Task<ActionResult<TicketMessageResponse>> AddEmailReply(EmailReplyRequest emailyReplyRequest)
        {
            TicketMessageRequest updateRequest = new TicketMessageRequest(emailyReplyRequest);

            TicketMessageResponse? response = await this.gorgiasApiService.SendTicketMessageUpdate(updateRequest, emailyReplyRequest.ApiKey, emailyReplyRequest.TicketID);
            return response == null ? NotFound() : Ok(response);
        }
    }
}