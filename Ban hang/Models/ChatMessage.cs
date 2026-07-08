namespace Ban_hang.Models;

public class ChatMessage
{
    public string Role { get; set; } = ""; // "user" hoặc "assistant"
    public string Content { get; set; } = "";
}

public class ChatRequest
{
    public string Message { get; set; } = "";
    public List<ChatMessage> History { get; set; } = new();
}

public class ChatResponse
{
    public string Reply { get; set; } = "";
}