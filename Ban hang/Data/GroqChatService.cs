using System.Text;
using System.Text.Json;
using Ban_hang.Models;

namespace Ban_hang.Services;

public class GroqChatService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _model;

    public GroqChatService(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        _apiKey = config["GroqSettings:ApiKey"] ?? "";
        _model = config["GroqSettings:Model"] ?? "llama-3.3-70b-versatile";
    }

    public async Task<string> AskAsync(string userMessage, List<ChatMessage> history)
    {
        // System prompt: định hình AI là "bác sĩ thú cưng"
        var messages = new List<object>
        {
            new
            {
                role = "system",
                content = "Bạn là bác sĩ thú y AI thân thiện của Petlor, một phòng khám thú cưng. " +
                           "Nhiệm vụ: tư vấn sơ bộ về sức khỏe, dinh dưỡng, hành vi của chó mèo và thú cưng. " +
                           "QUAN TRỌNG: Luôn nhắc người dùng rằng đây chỉ là tư vấn tham khảo, " +
                           "không thay thế khám trực tiếp, và khuyến khích đặt lịch khám tại Petlor nếu " +
                           "thú cưng có dấu hiệu nghiêm trọng (bỏ ăn, nôn mửa, chảy máu, khó thở...). " +
                           "Trả lời ngắn gọn, dễ hiểu, gần gũi bằng tiếng Việt, có thể dùng emoji thú cưng nhẹ nhàng."
            }
        };

        // Thêm lịch sử hội thoại (giới hạn 10 tin gần nhất để tránh quá tải)
        foreach (var h in history.TakeLast(10))
        {
            messages.Add(new { role = h.Role, content = h.Content });
        }

        // Thêm tin nhắn mới nhất
        messages.Add(new { role = "user", content = userMessage });

        var requestBody = new
        {
            model = _model,
            messages = messages,
            temperature = 0.7,
            max_tokens = 500
        };

        var json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");

        var response = await _httpClient.PostAsync("https://api.groq.com/openai/v1/chat/completions", content);

        if (!response.IsSuccessStatusCode)
        {
            var errorText = await response.Content.ReadAsStringAsync();
            return $"Xin lỗi, hiện tại AI đang bận. (Lỗi: {response.StatusCode})";
        }

        var responseText = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(responseText);

        var reply = doc.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString();

        return reply ?? "Xin lỗi, mình chưa hiểu câu hỏi. Bạn có thể mô tả rõ hơn không?";
    }
}