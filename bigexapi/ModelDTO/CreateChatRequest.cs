namespace bigexapi.ModelDTO;

public class CreateChatRequest
{
    public int SenderId { get; set; }
    public string RecipientEmail { get; set; }
    public string MessageText { get; set; }
}
