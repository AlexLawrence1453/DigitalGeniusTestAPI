namespace DigitalGeniusTestAPI.DTO.TicketMessage
{
    public class TicketMessageBase
    {
        public object?[]? attachments { get; set; }
        public string? body_html { get; set; }
        public string? body_text { get; set; }
        public string channel { get; set; }
        public DateTime? created_datetime { get; set; }
        public string? external_id { get; set; }
        public DateTime? failed_datetime { get; set; }
        public bool from_agent { get; set; }
        public string? message_id { get; set; }
        public object? receiver { get; set; }
        public object? sender { get; set; }
        public DateTime? sent_datetime { get; set; }
        public TicketMessageSource source { get; set; }
        public string? subject { get; set; }
        public string via { get; set; }
    }
}
