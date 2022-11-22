namespace DigitalGeniusTestAPI.DTO.TicketMessage
{
    public class TicketMessageResponse : TicketMessageBase
    {
        public int? id { get; set; }
        public int? integration_id { get; set; }
        public object? last_sending_error { get; set; }
        public int? rule_id { get; set; }
        public string? stripped_html { get; set; }
        public string? stripped_text { get; set; }
        public string? uri { get; set; }

    }
}
