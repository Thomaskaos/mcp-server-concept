using {{ServerName}};
using {{ServerName}}.Services;
using MCPServers.Shared.Extensions;
using MCPServers.Shared.Services;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    builder.Logging.AddConsole();
    builder.Logging.AddDebug();
}

var configuration = ConfigurationExtensions.BuildMcpConfiguration(includeEnvironmentVariables: true);
builder.Services.AddSingleton<IConfiguration>(configuration);

builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient());

builder.Services.AddScoped<{{ServerName}}Service>();

builder.Services.AddMcpOpenTelemetry(builder.Logging);

builder.Services.AddScoped<TokenContextAccessor>();
builder.Services.AddScoped<TokenExchangeService>();
builder.Services.AddMcpAuthentication(
    configuration,
    onTokenValidated: context =>
    {
        var tokenContextAccessor = context.HttpContext.RequestServices
            .GetRequiredService<TokenContextAccessor>();
        tokenContextAccessor.SetTokenValidatedContext(context);
        return Task.CompletedTask;
    },
    validateIssuer: true
);

var isTransportStateless = bool.Parse(configuration["IsTransportStateless"] ?? "true");
builder.Services.AddMcpServer()
    .WithHttpTransport(options => options.Stateless = isTransportStateless)
    .WithTools<{{ServerName}}Tool>();

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

var serverUrl = configuration["ServerUrl"] ?? "http://0.0.0.0:4547";

Console.WriteLine($"Starting MCP server with OBO token exchange at {serverUrl}");
Console.WriteLine($"Public URL: {publicUrl}");

app.MapMcp().RequireAuthorization();

app.Run(serverUrl);
