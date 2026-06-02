using System.ComponentModel;
using ModelContextProtocol.Server;
using KangarooMCP.Services;

namespace KangarooMCP;

[McpServerToolType]
public class KangarooMCPTool
{
    private readonly KangarooMCPService _service;

    public KangarooMCPTool(KangarooMCPService service)
    {
        _service = service;
    }

    [McpServerTool, Description("TODO: Replace with your tool description")]
    public async Task<string> ExampleTool(
        [Description("TODO: Replace with your parameter description")] string input)
    {
        return await _service.GetDataAsync(input);
    }
}
