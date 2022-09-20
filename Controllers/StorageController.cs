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

    // TODO Use login repo to check if they have an account before doing an action. Safegaurd. 
    [HttpPost("objects")]
    public async Task<ActionResult> UploadObjectsToBlob(List<IFormFile> files, [FromHeader] Guid accountId)
    {
        return Ok(await _blobService.UploadObjectsToBlob(files, accountId.ToString()));
    }

    [HttpGet("objects")]
    public async Task<ActionResult> GetBlobsFromContainer([FromHeader] Guid accountId){
        return Ok(await _blobService.GetBlobsFromContainer(accountId.ToString()));
    }

    [HttpGet("downloadObjects")]
    public async Task<ActionResult> DownloadBlob(string fileName, [FromHeader] Guid accountId)
    {
        var content = await _blobService.DownloadBlob(fileName, accountId.ToString());
        return File(content.Value.Content.ToArray(), "application/octet-stream", fileName);
    }
}