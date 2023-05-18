namespace Ultra.Msg.WebApi.Models.Message
{
    public class SendMessageModel
    {
        public string Text { get; set; }
        public Guid CorrelationId { get; set; }
    }
}
