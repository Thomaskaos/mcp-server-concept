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

    /// <summary>
    /// Search for species in the ALA database by name or keyword.
    /// Returns matching species with taxonomic information.
    /// </summary>
    public async Task<string> SearchSpeciesAsync(string query, int limit = 10)
    {
        if (string.IsNullOrWhiteSpace(query))
            throw new ArgumentException("Query cannot be empty", nameof(query));

        var url = $"{_baseUrl}/ws/species/search?q={Uri.EscapeDataString(query)}&limit={limit}";
        return await GetAsync(url);
    }

    /// <summary>
    /// Get detailed information about a specific species using its GUID or name.
    /// </summary>
    public async Task<string> GetSpeciesDetailsAsync(string speciesGuid)
    {
        if (string.IsNullOrWhiteSpace(speciesGuid))
            throw new ArgumentException("Species GUID cannot be empty", nameof(speciesGuid));

        var url = $"{_baseUrl}/ws/species/show/{Uri.EscapeDataString(speciesGuid)}";
        return await GetAsync(url);
    }

    /// <summary>
    /// Search for species occurrences (records of species sightings/observations).
    /// Can filter by location, year range, and other criteria.
    /// </summary>
    public async Task<string> SearchOccurrencesAsync(string taxonName, int limit = 20)
    {
        if (string.IsNullOrWhiteSpace(taxonName))
            throw new ArgumentException("Taxon name cannot be empty", nameof(taxonName));

        var url = $"{_baseUrl}/ws/occurrences/search?q=taxon_name:{Uri.EscapeDataString(taxonName)}&limit={limit}&pageSize={limit}";
        return await GetAsync(url);
    }

    /// <summary>
    /// Search by common name and get all associated scientific names.
    /// </summary>
    public async Task<string> SearchCommonNameAsync(string commonName)
    {
        if (string.IsNullOrWhiteSpace(commonName))
            throw new ArgumentException("Common name cannot be empty", nameof(commonName));

        var url = $"{_baseUrl}/ws/species/search?q={Uri.EscapeDataString(commonName)}&limit=20";
        return await GetAsync(url);
    }

    /// <summary>
    /// Get conservation status and threat information for a species.
    /// </summary>
    public async Task<string> GetSpeciesStatisticsAsync(string taxonName)
    {
        if (string.IsNullOrWhiteSpace(taxonName))
            throw new ArgumentException("Taxon name cannot be empty", nameof(taxonName));

        var url = $"{_baseUrl}/ws/occurrences/search?q=taxon_name:{Uri.EscapeDataString(taxonName)}&pageSize=0&facets=conservation_status";
        return await GetAsync(url);
    }
}
