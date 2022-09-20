using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using DMS.Models.Exceptions;

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
    public async Task<Azure.Response<BlobContainerClient>> CreateContainer(string accountId)
    {
        return await _blobServiceClient.CreateBlobContainerAsync(accountId);
    }

    public async Task<Azure.Response> DeleteBlobContainer(string accountId)
    {
        return await _blobServiceClient.DeleteBlobContainerAsync(accountId);
        
    }

    public async Task<List<Azure.Response<BlobContentInfo>>> UploadObjectsToBlob(List<IFormFile> files, string accountId)
    {
        BlobContainerClient containerClient = new BlobContainerClient(_azure_storage_connection_string, accountId);
        long size = files.Sum(f => f.Length);
                        
        var filePaths = new List<string>();
        List<Azure.Response<BlobContentInfo>> res = new List<Azure.Response<BlobContentInfo>>();
        foreach (var FormFile in files){
            res.Add(await containerClient.UploadBlobAsync(FormFile.FileName,FormFile.OpenReadStream()));
        }
        return res;
    }

    public async Task<List<string>> GetBlobsFromContainer(string accountId)
    {
        BlobContainerClient containerClient = new BlobContainerClient(_azure_storage_connection_string, accountId);
        
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

    public async Task<Azure.Response> DeleteObjectInBlob(string fileName, string accountId)
     {
        BlobContainerClient containerClient = new BlobContainerClient(_azure_storage_connection_string, accountId);
        return await containerClient.DeleteBlobAsync(fileName);
    }

    public async Task<Azure.Response<BlobDownloadResult>> DownloadBlob(string fileName, string accountId)
    {
        try 
        {
            BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(accountId);
            var blobClient = containerClient.GetBlobClient(fileName);
            return await blobClient.DownloadContentAsync();
        }
        catch (Azure.RequestFailedException e)
        {
            Console.WriteLine(e.Message);
            throw new KnownException(ErrorCategory.ResourceNotFound, ServiceErrorCode.File_NotFound, "blob not found");   
        }
    }
}