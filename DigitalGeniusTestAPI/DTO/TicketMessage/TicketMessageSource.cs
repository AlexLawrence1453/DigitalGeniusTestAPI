namespace DigitalGeniusTestAPI.DTO.TicketMessage
{
    public class TicketMessageSource
    {
        public string type { get; set; }
        public EmailAddress?[]? to { get; set; }
        public EmailAddress?[]? cc { get; set; }
        public EmailAddress?[]? bcc { get; set; }
        public EmailAddress from { get; set; }
    }
}
