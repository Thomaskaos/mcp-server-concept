using MCPServers.Shared;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace KangarooMCP.Services;

public class KangarooMCPService : BaseHttpService
{
    private readonly string _baseUrl;

    public KangarooMCPService(
        IConfiguration configuration,
        HttpClient client,
        ILogger<KangarooMCPService> logger)
        : base(configuration, client, logger)
    {
        _baseUrl = configuration["KangarooMCPApi:BaseUrl"]
            ?? throw new InvalidOperationException("KangarooMCPApi:BaseUrl is not configured");
    }

    public async Task<string> GetDataAsync(string input)
    {
        var url = $"{_baseUrl}/TODO-replace-with-endpoint/{Uri.EscapeDataString(input)}";
        return await GetAsync(url);
    }
}
