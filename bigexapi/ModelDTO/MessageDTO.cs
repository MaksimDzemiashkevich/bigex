using System.Text.Json.Serialization;

namespace bigexapi.ModelDTO;

public class MessageDTO
{
    [JsonPropertyName("text")]
    public string Text { get; set; } = string.Empty;

    [JsonPropertyName("senderId")]
    public int SenderId { get; set; }

    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; }
    [JsonPropertyName("senderName")]
    public string? SenderName { get; set; }
}
