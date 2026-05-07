using ASVS_Security_Auditor.Core.Interfaces;
using ASVS_Security_Auditor.Models;
using ASVS_Security_Auditor.Core.Entities;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace ASVS_Security_Auditor.Core.Services;

public class GroqAiService : IGroqAiService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public GroqAiService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey = configuration["Groq:ApiKey"] ?? string.Empty;
    }

    public async Task<string> GetRequirementExplanationAsync(string requirementDescription)
    {
        var messages = new[]
        {
            new { role = "system", content = "Tu es un expert en cybersécurité francophone. Explique l'exigence ASVS suivante de manière simple, en donnant un exemple concret de vérification. Réponds toujours en français." },
            new { role = "user", content = requirementDescription }
        };
        return await ContinueChatAsync(messages);
    }

    public async Task<string> ContinueChatAsync(IEnumerable<object> messages)
    {
        var requestBody = new
        {
            model = "llama-3.1-8b-instant",
            messages = messages
        };

        var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

        var request = new HttpRequestMessage(HttpMethod.Post, "https://api.groq.com/openai/v1/chat/completions")
        {
            Content = content
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

        try
        {
            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(responseString);
                var result = doc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();
                return result ?? "Aucune explication fournie.";
            }
            
            var errorContent = await response.Content.ReadAsStringAsync();
            return $"Erreur de l'API Groq ({response.StatusCode}): {errorContent}";
        }
        catch (System.Exception ex)
        {
            return $"Erreur lors de la connexion à l'API : {ex.Message}";
        }
    }

    public async IAsyncEnumerable<string> StreamChatAsync(IEnumerable<object> messages)
    {
        var requestBody = new
        {
            model = "llama-3.1-8b-instant",
            messages = messages,
            stream = true
        };

        var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

        var request = new HttpRequestMessage(HttpMethod.Post, "https://api.groq.com/openai/v1/chat/completions")
        {
            Content = content
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

        var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
        
        if (!response.IsSuccessStatusCode)
        {
            yield return $"[ERREUR] Impossible de joindre l'IA ({response.StatusCode})";
            yield break;
        }

        using var stream = await response.Content.ReadAsStreamAsync();
        using var reader = new System.IO.StreamReader(stream);

        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync();
            if (string.IsNullOrWhiteSpace(line)) continue;
            
            if (line.StartsWith("data: "))
            {
                var data = line.Substring(6);
                if (data == "[DONE]") break;

                JsonDocument doc;
                try
                {
                    doc = JsonDocument.Parse(data);
                }
                catch
                {
                    continue;
                }

                using (doc)
                {
                    var choices = doc.RootElement.GetProperty("choices");
                    if (choices.GetArrayLength() > 0)
                    {
                        var delta = choices[0].GetProperty("delta");
                        if (delta.TryGetProperty("content", out var contentProp))
                        {
                            var chunk = contentProp.GetString();
                            if (!string.IsNullOrEmpty(chunk))
                            {
                                yield return chunk;
                            }
                        }
                    }
                }
            }
        }
    }
}


