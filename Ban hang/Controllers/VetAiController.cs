using Microsoft.AspNetCore.Mvc;
using Ban_hang.Models;
using Ban_hang.Services;

namespace Ban_hang.Controllers;

public class VetAiController : Controller
{
    private readonly GroqChatService _groqChatService;

    public VetAiController(GroqChatService groqChatService)
    {
        _groqChatService = groqChatService;
    }

    // GET /VetAi
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    // POST /VetAi/Ask
    [HttpPost]
    public async Task<IActionResult> Ask([FromBody] ChatRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Message))
        {
            return BadRequest(new { error = "Vui lòng nhập câu hỏi." });
        }


        var reply = await _groqChatService.AskAsync(request.Message, request.History);

        return Json(new ChatResponse { Reply = reply });
    }
}