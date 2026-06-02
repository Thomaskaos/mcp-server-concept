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

    [McpServerTool, Description("Search for species in the Australian Bioscience (ALA) database by name or keyword")]
    public async Task<string> SearchSpecies(
        [Description("Species name, scientific name, or keyword to search for")] string query,
        [Description("Maximum number of results to return (default: 10)")] int limit = 10)
    {
        return await _service.SearchSpeciesAsync(query, limit);
    }

    [McpServerTool, Description("Get detailed taxonomic and biodiversity information about a specific species")]
    public async Task<string> GetSpeciesDetails(
        [Description("Species GUID or name from ALA database")] string speciesGuid)
    {
        return await _service.GetSpeciesDetailsAsync(speciesGuid);
    }

    [McpServerTool, Description("Search for occurrence records (sightings and observations) of a species")]
    public async Task<string> SearchOccurrences(
        [Description("Scientific or common name of the taxon")] string taxonName,
        [Description("Maximum number of occurrence records to return (default: 20)")] int limit = 20)
    {
        return await _service.SearchOccurrencesAsync(taxonName, limit);
    }

    [McpServerTool, Description("Search for species by common name and get all associated scientific names")]
    public async Task<string> SearchCommonName(
        [Description("Common name of the species (e.g., 'platypus', 'koala')")] string commonName)
    {
        return await _service.SearchCommonNameAsync(commonName);
    }

    [McpServerTool, Description("Get conservation status, threats, and population statistics for a species")]
    public async Task<string> GetSpeciesStatistics(
        [Description("Scientific name of the species")] string taxonName)
    {
        return await _service.GetSpeciesStatisticsAsync(taxonName);
    }
}
