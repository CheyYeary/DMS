using Microsoft.AspNetCore.Mvc;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.IO;

namespace dead_switch_api.Controllers;

[ApiController]
[Route("api/storage")]
public class StorageController: ControllerBase
{
    const string AZURE_STORAGE_CONNECTION_STRING = "";
    BlobServiceClient blobServiceClient = new BlobServiceClient(AZURE_STORAGE_CONNECTION_STRING);
    // private readonly ObjectDataStore _objectDataStore;

    // public StorageController(ObjectDataStore objectDataStore)
    // {
    //    _objectDataStore = objectDataStore ?? throw new ArgumentNullException(nameof(objectDataStore));
    // }
    

    [HttpGet("containers")]
    public async Task<ActionResult> GetContainers()
    {
        List<string> containers = new List<string>();
        Console.WriteLine("Azure Blob Storage v12 - .NET quickstart sample\n");
        // Create a BlobServiceClient object which will be used to create a container client
        var results = blobServiceClient.GetBlobContainersAsync().AsPages();
        await foreach (Azure.Page<BlobContainerItem> containerPage in results)
        {
            foreach (BlobContainerItem containerItem in containerPage.Values)
            {
                containers.Add($"Container name: {containerItem.Name}");
            }
        }
        return Ok(containers);
    }

    [HttpPost("container")]
    public async Task<ActionResult> CreateContainer()
    {
        Console.WriteLine("Azure Blob Storage v12 - .NET quickstart sample\n");
        //Create a unique name for the container
        string containerName = "quickstartblobs" + Guid.NewGuid().ToString();

        // Create the container and return a container client object
        BlobContainerClient containerClient = await blobServiceClient.CreateBlobContainerAsync(containerName);
        return Ok();
    }

    [HttpDelete("container")]
    public ActionResult DeleteBlobContainer(string containerName)
    {
        Console.WriteLine("Delete Blob");
        var res = blobServiceClient.DeleteBlobContainer(containerName);
        // deleting does not immeditately delete the container. The above gets back a 202 which is accepted but not processed.
        return Ok();
    }

    // Possibly make another controller just for the blob
    // Make api to post file to blob store possibly database. Param will be for a specifc container.
    [HttpPost("blob")]
    public async Task<ActionResult> UploadObjectToBlob(List<IFormFile> files, string containerName)
    {
        BlobContainerClient containerClient = new BlobContainerClient(AZURE_STORAGE_CONNECTION_STRING, containerName);
        long size = files.Sum(f => f.Length);
                        
        var filePaths = new List<string>();

        foreach (var FormFile in files){
            var res = await containerClient.UploadBlobAsync(FormFile.FileName,FormFile.OpenReadStream());
        }
        
        // return Created instead of 200 at some point
        return Ok();
    }

    [HttpGet("blobs")]
    public async Task<ActionResult> GetBlobsFromContainer(string containerName){
        BlobContainerClient containerClient = new BlobContainerClient(AZURE_STORAGE_CONNECTION_STRING, containerName);
        
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
        return Ok(blobItems);
    }

    [HttpDelete("deleteblob")]
    public async Task<IActionResult> DeleteObjectInBlob(string fileName, string containerName)
    {
        BlobContainerClient containerClient = new BlobContainerClient(AZURE_STORAGE_CONNECTION_STRING, containerName);
        var res = await containerClient.DeleteBlobAsync(fileName);
        return Ok();
    }

    [HttpGet("downloadblob")]
    public async Task<ActionResult> DownloadBlob(string fileName, string containerName)
    {
        try 
        {
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            
            var blobs = containerClient.GetBlobs();

            foreach(var blob in blobs)
            {
                Console.WriteLine(blob.Name);
                if(blob.Name == fileName)
                {
                    BlobClient blobClient = containerClient.GetBlobClient(blob.Name);
                    var content = await blobClient.DownloadContentAsync();
                    
                    return File(content.Value.Content.ToArray(), "application/octet-stream", blob.Name);
                }
            }
        }
        catch (Azure.RequestFailedException e)
        {
            Console.WriteLine(e.Message);
            return StatusCode(500);
        }
        return NotFound();
    }
}



// Cleanup the methods I have tomorrow. 
// download a file from the blob. ? 
// Delete the blob altogether. This needs to be an action on an owner can take.