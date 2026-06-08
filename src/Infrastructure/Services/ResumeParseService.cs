using Application.Common.Interfaces.Services;
using Domain.Entities.Resumes;
using Infrastructure.AI.Claude;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace Infrastructure.Services;

public sealed class ResumeParseService : IResumeParseService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ClaudeOptions _options;

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public ResumeParseService(IHttpClientFactory httpClientFactory, IOptions<ClaudeOptions> options)
    {
        _httpClientFactory = httpClientFactory;
        _options = options.Value;
    }

    public async Task<ResumeParseData> ParseAsync(Stream fileContent, string fileName, CancellationToken cancellationToken)
    {
        var text = ExtractText(fileContent, fileName);

        if (string.IsNullOrWhiteSpace(text))
            return new ResumeParseData();

        return await CallClaudeAsync(text, cancellationToken);
    }

    private static string ExtractText(Stream stream, string fileName)
    {
        var ext = Path.GetExtension(fileName).ToLowerInvariant();

        if (ext == ".pdf")
            return ExtractPdfText(stream);

        if (ext == ".docx")
            return ExtractDocxText(stream);

        return string.Empty;
    }

    private static string ExtractPdfText(Stream stream)
    {
        var ms = new MemoryStream();
        stream.CopyTo(ms);
        ms.Position = 0;
        var sb = new StringBuilder();
        using var doc = PdfDocument.Open(ms);
        foreach (Page page in doc.GetPages())
            sb.AppendLine(page.Text);
        return sb.ToString();
    }

    private static string ExtractDocxText(Stream stream)
    {
        var ms = new MemoryStream();
        stream.CopyTo(ms);
        ms.Position = 0;
        var sb = new StringBuilder();
        using var doc = WordprocessingDocument.Open(ms, false);
        var body = doc.MainDocumentPart?.Document?.Body;
        if (body == null) return string.Empty;
        foreach (var para in body.Elements<Paragraph>())
            sb.AppendLine(para.InnerText);
        return sb.ToString();
    }

    private async Task<ResumeParseData> CallClaudeAsync(string resumeText, CancellationToken cancellationToken)
    {
        var requestBody = new
        {
            model = _options.Model,
            max_tokens = _options.MaxTokens,
            system = _options.ResumeParseSystemPrompt,
            messages = new[]
            {
                new { role = "user", content = resumeText }
            }
        };

        var client = _httpClientFactory.CreateClient("Claude");
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Add("x-api-key", _options.ApiKey);
        client.DefaultRequestHeaders.Add("anthropic-version", _options.AnthropicVersion);

        var json = JsonSerializer.Serialize(requestBody);
        using var request = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await client.PostAsync("v1/messages", request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);
        var node = JsonNode.Parse(responseJson);
        var text = node?["content"]?[0]?["text"]?.GetValue<string>();

        if (string.IsNullOrWhiteSpace(text))
            return new ResumeParseData();

        // Strip markdown code fences (```json ... ```) that Claude may wrap around the response
        var trimmed = text.Trim();
        if (trimmed.StartsWith("```"))
        {
            var firstNewline = trimmed.IndexOf('\n');
            var lastFence = trimmed.LastIndexOf("```");
            if (firstNewline >= 0 && lastFence > firstNewline)
                trimmed = trimmed[(firstNewline + 1)..lastFence].Trim();
        }

        return JsonSerializer.Deserialize<ResumeParseData>(trimmed, _jsonOptions)
               ?? new ResumeParseData();
    }
}

