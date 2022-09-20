

using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace DMS.DataProviders;


public interface IBlobService
{
    Task<List<string>> GetContainers();
    Task<Azure.Response<BlobContainerClient>> CreateContainer(string accountId);
    Task<Azure.Response> DeleteBlobContainer(string accountId);
    Task<List<Azure.Response<BlobContentInfo>>> UploadObjectsToBlob(List<IFormFile> files, string accountId);
    Task<List<string>> GetBlobsFromContainer(string accountId);
    Task<Azure.Response> DeleteObjectInBlob(string fileName, string containerName);
    Task<Azure.Response<BlobDownloadResult>> DownloadBlob(string fileName, string accountId);
}
