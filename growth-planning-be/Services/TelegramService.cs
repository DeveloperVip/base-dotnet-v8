using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace growth_planning_be.Services;

public class TelegramService
{
    private readonly string? _chatId;
    private readonly string _apiUrl;
    private readonly string _fileUrl;
    
    public TelegramService(IConfiguration configuration)
    {
        var botToken = configuration["Telegram:BotToken"];
        _chatId = configuration["Telegram:ChatId"];
        _apiUrl = $"https://api.telegram.org/bot{botToken}/sendMessage";
        _fileUrl = $"https://api.telegram.org/bot{botToken}/sendDocument";
    }

    public  async Task SendMessageAsync(string message)
    {
        using HttpClient client = new HttpClient();
        var data = new { chat_id = _chatId, text = message, parse_mode = "HTML" };
        string json = JsonSerializer.Serialize(data);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        HttpResponseMessage response = await client.PostAsync(_apiUrl, content);
        string result = await response.Content.ReadAsStringAsync();
        Console.WriteLine(result);
    }
}