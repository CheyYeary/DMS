using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;

namespace DMS.DataProviders;

public class AzureBlobService: IBlobService 
{
    private string _azure_storage_connection_string = string.Empty;
    private readonly BlobServiceClient _blobServiceClient;
    public AzureBlobService(IConfiguration configuration)
    {
        _azure_storage_connection_string = configuration["ConnectionStrings:dev"];
        _blobServiceClient = new BlobServiceClient(_azure_storage_connection_string);
    }


    public async Task<List<string>> GetContainers()
        {
            List<string> containers = new List<string>();
            Console.WriteLine("Azure Blob Storage v12 - .NET quickstart sample\n");
            // Create a BlobServiceClient object which will be used to create a container client
            var results = _blobServiceClient.GetBlobContainersAsync().AsPages();
            await foreach (Azure.Page<BlobContainerItem> containerPage in results)
            {
                foreach (BlobContainerItem containerItem in containerPage.Values)
                {
                    containers.Add($"Container name: {containerItem.Name}");
                }
            }
            return containers;
        }
    // TODO : ACCOUNT ID IS EQUAL TO CONTAINER NAME
    public async Task<Azure.Response<BlobContainerClient>> CreateContainer(string AccountId)
    {
        return await _blobServiceClient.CreateBlobContainerAsync(AccountId);
    }

    public async Task<Azure.Response> DeleteBlobContainer(string AccountId)
    {
        return await _blobServiceClient.DeleteBlobContainerAsync(AccountId);
        
    }

    public async Task<List<Azure.Response<BlobContentInfo>>> UploadObjectsToBlob(List<IFormFile> files, string AccountId)
    {
        BlobContainerClient containerClient = new BlobContainerClient(_azure_storage_connection_string, AccountId);
        long size = files.Sum(f => f.Length);
                        
        var filePaths = new List<string>();
        List<Azure.Response<BlobContentInfo>> res = new List<Azure.Response<BlobContentInfo>>();
        foreach (var FormFile in files){
            res.Add(await containerClient.UploadBlobAsync(FormFile.FileName,FormFile.OpenReadStream()));
        }
        return res;
    }

    public async Task<List<string>> GetBlobsFromContainer(string AccountId)
    {
        BlobContainerClient containerClient = new BlobContainerClient(_azure_storage_connection_string, AccountId);
        
        List<string> blobItems = new List<string>();
        try
        {
            // Call the listing operation and return pages of the specified size.
            var resultSegment = containerClient.GetBlobsAsync()
                .AsPages(default);

            // Enumerate the blobs returned for each page.
            await foreach (Azure.Page<BlobItem> blobPage in resultSegment)
            {
                foreach (BlobItem blobItem in blobPage.Values)
                {
                    blobItems.Add($"Blob name: { blobItem.Name}");
                }

                Console.WriteLine();
            }
        }
        catch (Azure.RequestFailedException e)
        {
            Console.WriteLine(e.Message);
            Console.ReadLine();
            throw;
        }
        return blobItems;
    }

    public async Task<Azure.Response> DeleteObjectInBlob(string fileName, string AccountId)
     {
        BlobContainerClient containerClient = new BlobContainerClient(_azure_storage_connection_string, AccountId);
        return await containerClient.DeleteBlobAsync(fileName);
    }
}