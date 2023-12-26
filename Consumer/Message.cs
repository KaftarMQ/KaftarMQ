namespace Consumer;

public class Message
{
    public string Content { get; set; }
    public string MessageId { get; set; } // Unique identifier for acknowledgment
    // Other properties
}
