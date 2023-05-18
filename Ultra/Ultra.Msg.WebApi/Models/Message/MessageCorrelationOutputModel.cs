namespace Ultra.Msg.WebApi.Models.Message
{
    public class MessageCorrelationOutputModel : MessageOutputModel
    {
        public Guid CorrelationId { get; set; }
    }
}
