

using Azure.Storage.Blobs;

namespace DMS.DataProviders;


public interface IBlobService
{
    Task<List<string>> GetContainers();
    
    Task<Azure.Response<BlobContainerClient>> CreateContainer(string accountId);
    
    Task<Azure.Response> DeleteBlobContainer(string accountId);
    
    Task<T> UploadObjectToBlob<T>(string blobName, string accountId, T value, CancellationToken cancellationToken);

    Task<T> DownloadBlobAsync<T>(string blobName, string accountId, CancellationToken cancellationToken);

    Task<List<string>> GetBlobsFromContainer(string accountId);
    
    Task<Azure.Response> DeleteObjectInBlob(string fileName, string containerName);
}
