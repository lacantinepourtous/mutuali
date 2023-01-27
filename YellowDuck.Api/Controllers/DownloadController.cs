using System.Threading.Tasks;
using YellowDuck.Api.Services.Crypto;
using YellowDuck.Api.Services.Files;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace YellowDuck.Api.Controllers
{
    [Route("[controller]")]
    public class DownloadController : Controller
    {
        [Route("file/{container}/{*fileId}")]
        public async Task<IActionResult> File(
            [FromServices] ISignatureService signature,
            [FromServices] IFileManager fileManager,
            string container,
            string fileId)
        {
            if (!signature.VerifySignedUrl(Request.GetEncodedPathAndQuery())) return Unauthorized();

            var file = await fileManager.DownloadFile(container, fileId);
            if (file == null) return NotFound();

            return File(file.Content, file.ContentType, file.FileName);
        }
    }
}