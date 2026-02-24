using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI;
using System.ClientModel;
using System.Net.Http.Headers;
using System.Text.Json;

namespace CustomerManager.Services;

public interface IGitHubModelsAgentService
{
    Task<GitHubModelsChatResult> ChatAsync(string prompt);
}

public class GitHubModelsChatResult
{
    public string Response { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
}

public class GitHubModelsAgentService : IGitHubModelsAgentService
{
    private readonly OpenAIClient? _client;
    private readonly string? _configuredModel;
    private readonly string? _endpoint;
    private readonly string? _token;
    private string? _resolvedModel;
    private readonly string? _configurationError;

    public GitHubModelsAgentService(IConfiguration configuration)
    {
        var token = configuration["GITHUB_MODELS_TOKEN"] ?? configuration["GITHUB_TOKEN"];
        if (string.IsNullOrWhiteSpace(token))
        {
            _configurationError = "Set GITHUB_MODELS_TOKEN (or GITHUB_TOKEN) with models:read permission.";
            return;
        }

        var endpoint = configuration["GITHUB_MODELS_ENDPOINT"] ?? "https://models.inference.ai.azure.com";
        var model = configuration["GITHUB_MODELS_MODEL"] ?? "gpt-4o-mini";

        _token = token;
        _endpoint = endpoint;
        _configuredModel = model;

        var clientOptions = new OpenAIClientOptions
        {
            Endpoint = new Uri(endpoint)
        };

        _client = new OpenAIClient(new ApiKeyCredential(token), clientOptions);
    }

    public async Task<GitHubModelsChatResult> ChatAsync(string prompt)
    {
        if (!string.IsNullOrWhiteSpace(_configurationError))
        {
            throw new InvalidOperationException(_configurationError);
        }

        if (string.IsNullOrWhiteSpace(prompt))
        {
            throw new ArgumentException("Prompt is required.", nameof(prompt));
        }

        var model = await EnsureResolvedModelAsync();
        var chatClient = _client!.GetChatClient(model).AsIChatClient();

        var agent = new ChatClientAgent(
            chatClient,
            instructions: "You are a helpful assistant for customer management tasks.",
            name: "CustomerManagerAgent");

        var result = await agent.RunAsync(prompt);
        return new GitHubModelsChatResult
        {
            Response = result.Text,
            Model = model
        };
    }

    private async Task<string> EnsureResolvedModelAsync()
    {
        if (!string.IsNullOrWhiteSpace(_resolvedModel))
        {
            return _resolvedModel;
        }

        var availableModels = await FetchChatModelNamesAsync();
        var candidates = BuildCandidates(_configuredModel!);

        foreach (var candidate in candidates)
        {
            if (availableModels.Contains(candidate, StringComparer.OrdinalIgnoreCase))
            {
                _resolvedModel = candidate;
                return _resolvedModel;
            }
        }

        _resolvedModel = availableModels.FirstOrDefault(name =>
            name.Contains("gpt-4o-mini", StringComparison.OrdinalIgnoreCase))
            ?? availableModels.FirstOrDefault();

        if (string.IsNullOrWhiteSpace(_resolvedModel))
        {
            throw new InvalidOperationException("No chat-completion model is available for this token.");
        }

        return _resolvedModel;
    }

    private async Task<List<string>> FetchChatModelNamesAsync()
    {
        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);

        var response = await httpClient.GetAsync($"{_endpoint!.TrimEnd('/')}/models");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        using var json = JsonDocument.Parse(content);

        var names = new List<string>();
        foreach (var model in json.RootElement.EnumerateArray())
        {
            if (!model.TryGetProperty("task", out var taskProp))
            {
                continue;
            }

            var task = taskProp.GetString();
            if (!string.Equals(task, "chat-completion", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (model.TryGetProperty("name", out var nameProp))
            {
                var name = nameProp.GetString();
                if (!string.IsNullOrWhiteSpace(name))
                {
                    names.Add(name);
                }
            }
        }

        return names;
    }

    private static List<string> BuildCandidates(string configuredModel)
    {
        var candidates = new List<string>();

        if (!string.IsNullOrWhiteSpace(configuredModel))
        {
            candidates.Add(configuredModel);

            var lastSegment = configuredModel.Split('/').Last();
            if (!string.IsNullOrWhiteSpace(lastSegment))
            {
                candidates.Add(lastSegment);
            }

            if (!configuredModel.Contains('/'))
            {
                candidates.Add($"openai/{configuredModel}");
                candidates.Add($"azure-openai/{configuredModel}");
            }
        }

        return candidates.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
    }
}
