using System;
using System.IO;
using System.Threading.Tasks;
using YellowDuck.Api.Constants;
using YellowDuck.Api.Services.Files;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System.Text.RegularExpressions;

namespace YellowDuck.Api.Controllers
{
    [Route("upload")]
    [Authorize] // Seulement les utilisateurs identifiés peuvent uploader.
    public class UploadController : Controller
    {
        private const int MaxImageWidth = 1920;
        private const int MaxImageHeight = 1080;

        private readonly IFileManager fileManager;
        private readonly ILogger<UploadController> logger;

        public UploadController(IFileManager fileManager, ILogger<UploadController> logger)
        {
            this.fileManager = fileManager;
            this.logger = logger;
        }

        [HttpPost("image")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file == null) return BadRequest();

            FileInfos fileInfos;
            
            try
            {
                fileInfos = await GetImageFileInfos(file);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error pre-processing uploaded image");
                return BadRequest();
            }

            var fileId = await fileManager.UploadFile(FileContainers.Images, fileInfos);
            logger.LogInformation($"Image uploaded ({fileId})");

            return Ok(new 
            {
                fileId
            });
        }

        [HttpPost("file")]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null) return BadRequest();

            FileInfos fileInfos;

            try
            {
                fileInfos = GetFileInfos(file);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error pre-processing uploaded file");
                return BadRequest();
            }

            var fileId = await fileManager.UploadFile(FileContainers.Files, fileInfos);
            logger.LogInformation($"File uploaded ({fileId})");

            return Ok(new
            {
                fileId
            });
        }

        private static async Task<FileInfos> GetImageFileInfos(IFormFile file)
        {
            using (var stream = file.OpenReadStream())
            using (var image = await Image.LoadAsync(stream))
            {
                if (image.Width > MaxImageWidth || image.Height > MaxImageHeight)
                {
                    image.Mutate(x =>
                        x.Resize(new ResizeOptions
                        {
                            Mode = ResizeMode.Max,
                            Size = new Size(MaxImageWidth, MaxImageHeight)
                        }));
                }

                var fileInfos = new FileInfos {Content = new MemoryStream()};
                
                if (file.FileName.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
                {
                    await image.SaveAsPngAsync(fileInfos.Content);
                    fileInfos.ContentType = "image/png";
                    fileInfos.FileName = EnsureSafeFileName(file.FileName);
                }
                else
                {
                    await image.SaveAsJpegAsync(fileInfos.Content);
                    fileInfos.ContentType = "image/jpeg";
                    fileInfos.FileName = EnsureSafeFileName(file.FileName);
                }

                fileInfos.Content.Position = 0;

                return fileInfos;
            }
        }

        private static string EnsureSafeFileName(string fileName)
        {
            return Regex.Replace(fileName, "[^a-zA-Z0-9-_.]", "");
        }

        private static FileInfos GetFileInfos(IFormFile file)
        {
            var stream = file.OpenReadStream();
            var fileInfos = new FileInfos { Content = stream, ContentType = file.ContentType, FileName = EnsureSafeFileName(file.FileName) };
            return fileInfos;
        }
    }
}