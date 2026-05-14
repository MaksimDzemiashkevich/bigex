namespace bigexapi.ModelDTO;

public class SendMessageRequest
{
    public string ChatId { get; set; }
    public int SenderId { get; set; }
    public string Text { get; set; }
}
