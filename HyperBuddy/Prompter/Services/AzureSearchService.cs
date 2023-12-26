using Azure.Search.Documents.Indexes;
using Azure;

namespace HyperBuddy.Prompter.Services;

public interface IAzureSearchService
{
    Task RunRecipeIndexer();
}

public class AzureSearchService(IConfiguration configs): IAzureSearchService
{
    private readonly SearchIndexerClient _searchIndexerClient = CreateSearchIndexClient(configs);
    private readonly string _indexerName = configs.GetValue<string>("SearchIndexerName") ?? string.Empty;

    public async Task RunRecipeIndexer()
    {
        await _searchIndexerClient.RunIndexerAsync(_indexerName);
    }


    private static SearchIndexerClient CreateSearchIndexClient(IConfiguration configuration)
    {
        string searchServiceEndPoint = configuration.GetValue<string>("SearchIndexerEndpoint") ?? string.Empty;
        string adminApiKey = configuration.GetValue<string>("SearchIndexerKey") ?? string.Empty;

        SearchIndexerClient indexClient = new SearchIndexerClient(new Uri(searchServiceEndPoint), new AzureKeyCredential(adminApiKey));
        return indexClient;
    }

}

