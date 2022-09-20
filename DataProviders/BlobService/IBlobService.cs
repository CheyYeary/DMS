

using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace DMS.DataProviders;


public interface IBlobService
{
    Task<List<string>> GetContainers();
    Task<Azure.Response<BlobContainerClient>> CreateContainer(string AccountId);
    Task<Azure.Response> DeleteBlobContainer(string AccountId);
    Task<List<Azure.Response<BlobContentInfo>>> UploadObjectsToBlob(List<IFormFile> files, string AccountId);
    Task<List<string>> GetBlobsFromContainer(string AccountId);
    Task<Azure.Response> DeleteObjectInBlob(string fileName, string containerName);
}
