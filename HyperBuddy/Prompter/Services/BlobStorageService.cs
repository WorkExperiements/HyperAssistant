using Azure.Storage.Blobs;
using System.Configuration;
using System.Reflection.Metadata;

namespace HyperBuddy.Prompter.Services;

public interface IBlobStorageService
{
    Task<BlobServiceResponse> UploadFile(string blobContents, string fileName);
}
public class BlobStorageService : IBlobStorageService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly BlobContainerClient _containerClient;
    private readonly string _blobContainerName;
    public BlobStorageService(IConfiguration configs)
    {
        // Retrieve the connection string for use with the application. 
        string connectionString = configs.GetConnectionString("BlobStorage") ?? string.Empty;

        // Create a BlobServiceClient object 
        _blobContainerName = configs.GetValue<string>("BlobContainerName") ?? string.Empty;
        if (connectionString == string.Empty || _blobContainerName == string.Empty)
        {
            throw new ConfigurationErrorsException("Blob Storage Configurations are missing.");
        }

        _blobServiceClient = new BlobServiceClient(connectionString);
        _containerClient = _blobServiceClient.GetBlobContainerClient(_blobContainerName);
    }
    public async Task<BlobServiceResponse> UploadFile(string blobContents, string fileName)
    {
        var blobClient = _containerClient.GetBlobClient(fileName);
        await blobClient.UploadAsync(BinaryData.FromString(blobContents), true);
        return new BlobServiceResponse() { Success = true};
    }
}

public class BlobServiceResponse
{
    public bool Success { get; set; }
}
