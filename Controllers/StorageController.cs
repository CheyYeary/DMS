using Azure.Storage.Blobs;
using DMS.DataProviders;
using Microsoft.AspNetCore.Mvc;


namespace dead_switch_api.Controllers;

[ApiController]
[Route("api/storage")]
public class StorageController: ControllerBase
{
    private readonly IBlobService _blobService;
    public StorageController(IConfiguration configuration, IBlobService blobService)
    {
        _blobService = blobService;
    }

    [HttpPost("objects")]
    public async Task<ActionResult> UploadObjectsToBlob(List<IFormFile> files, string accountId)
    {
        return Ok(await _blobService.UploadObjectsToBlob(files, accountId));
    }

    [HttpGet("objects")]
    public async Task<ActionResult> GetBlobsFromContainer(string accountId){
        return Ok(await _blobService.GetBlobsFromContainer(accountId));
    }

    [HttpGet("downloadObjects")]
    public async Task<ActionResult> DownloadBlob(string fileName, string accountId)
    {
        var content = await _blobService.DownloadBlob(fileName, accountId);
        return File(content.Value.Content.ToArray(), "application/octet-stream", fileName);
    }
}