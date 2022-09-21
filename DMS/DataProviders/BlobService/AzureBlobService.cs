using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Threading;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System.Net;
using Azure;
using System.ComponentModel;
using System.Reflection.Metadata;

namespace DMS.DataProviders;

public class AzureBlobService: IBlobService 
{
    private string _azure_storage_connection_string = string.Empty;
    private readonly BlobServiceClient _blobServiceClient;
    private static readonly JsonSerializer Serializer;

    static AzureBlobService()
    {
        Serializer = JsonSerializer.Create(new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            DateTimeZoneHandling = DateTimeZoneHandling.Utc,
            Converters = new List<JsonConverter>
            {
                new IsoDateTimeConverter(),
                new StringEnumConverter { CamelCaseText = true }
            }
        });
    }

    public AzureBlobService(IConfiguration configuration)
    {
        _azure_storage_connection_string = Environment.GetEnvironmentVariable("ConnectionString") ?? configuration["ConnectionStrings:dev"] ?? throw new ArgumentNullException("ConnectionString");
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
    public async Task<Response<BlobContainerClient>> CreateContainer(string accountId)
    {
        return await _blobServiceClient.CreateBlobContainerAsync(accountId);
    }

    public async Task<Response> DeleteBlobContainer(string AccountId)
    {
        return await _blobServiceClient.DeleteBlobContainerAsync(AccountId);
    }

    public async Task<T> UploadObjectToBlob<T>(string blobName, string accountId, T value, CancellationToken cancellationToken)
    {
        BlobContainerClient containerClient = new(_azure_storage_connection_string, accountId);

        BlobClient blobClient = containerClient.GetBlobClient(blobName);
        using MemoryStream memStream = new();
        using (StreamWriter streamWriter = new(memStream, leaveOpen: true))
        using (JsonTextWriter jsonWriter = new(streamWriter) { CloseOutput = false })
        {
            Serializer.Serialize(jsonWriter, value);
        }

        memStream.Seek(0, SeekOrigin.Begin);

        await blobClient.UploadAsync(memStream, overwrite: true, cancellationToken);

        memStream.Seek(0, SeekOrigin.Begin);
        using StreamReader textReader = new(memStream, leaveOpen: true);
        using JsonTextReader jsonReader = new(textReader) { CloseInput = false };
        
        return Serializer.Deserialize<T>(jsonReader);
    }


    public async Task<T> DownloadBlobAsync<T>(string blobName, string accountId, CancellationToken cancellationToken)
    {
        BlobContainerClient containerClient = new(_azure_storage_connection_string, accountId);
        using MemoryStream memStream = new();
        try
        {
            BlobClient blobClient = containerClient.GetBlobClient(blobName);
            await blobClient.DownloadToAsync(memStream, cancellationToken);
        }
        catch (RequestFailedException ex) when (ex.ErrorCode == BlobErrorCode.ContainerBeingDeleted || ex.ErrorCode == BlobErrorCode.ContainerNotFound)
        {
            // Ignore any errors if the container being deleted or if it has already been deleted
            return default;
        }

        memStream.Seek(0, SeekOrigin.Begin);
        using StreamReader textReader = new(memStream, leaveOpen: true);
        using JsonTextReader jsonReader = new(textReader) { CloseInput = false };
        var entity = Serializer.Deserialize<T>(jsonReader);

        return entity;
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

    public async Task<Response> DeleteObjectInBlob(string fileName, string accountId)
     {
        BlobContainerClient containerClient = new BlobContainerClient(_azure_storage_connection_string, accountId);
        return await containerClient.DeleteBlobAsync(fileName);
    }
}