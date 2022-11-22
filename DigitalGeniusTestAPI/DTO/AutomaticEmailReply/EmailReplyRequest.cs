using DigitalGeniusTestAPI.DTO.TicketMessage;

namespace DigitalGeniusTestAPI.DTO.AutomaticEmailReply
{
    public class EmailReplyRequest
    {
        public int TicketID { get; set; }
        public EmailAddress FromUser { get; set; }
        public string EmailText { get; set; }
        public string ApiKey { get; set; }
        public List<EmailAddress> ToAddresses { get; set; }
    }
}
