using DigitalGeniusTestAPI.DTO.AutomaticEmailReply;
using System.Security.AccessControl;

namespace DigitalGeniusTestAPI.DTO.TicketMessage
{
    public class TicketMessageRequest : TicketMessageBase
    {
        public TicketMessageRequest(EmailReplyRequest emailReplyRequest)
        {
            this.body_text = emailReplyRequest.EmailText;
            this.channel = "email";
            this.from_agent = true;
            this.source = new TicketMessageSource()
            {
                type = "email",
                to = emailReplyRequest.ToAddresses.ToArray(),
                from = emailReplyRequest.FromUser,
            };
            this.via = "api";

        }
    }
}
