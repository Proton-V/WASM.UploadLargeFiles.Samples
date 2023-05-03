using API.AspNet.Shared.Models;
using API.AspNet.Utils;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;
using System.Net;
using System.Net.Http.Headers;

namespace API.AspNet.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FileUploaderController : ControllerBase
    {
        private readonly ILogger<FileUploaderController> _logger;
        private const string _storageFolderPath = "Files";

        public FileUploaderController(ILogger<FileUploaderController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("GetFileDatas")]
        public IEnumerable<FileData> Get()
        {
            _logger.LogInformation(nameof(Get));

            try
            {
                var files = Directory
                    .GetFiles(_storageFolderPath)
                    .Select(x => new FileInfo(x));

                var fileDatas = files.Select(file => new FileData
                {
                    Name = file.Name,
                    CreatedAt = file.CreationTime,
                    SizeInBytes = file.Length
                });
                return fileDatas;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return Enumerable.Empty<FileData>();
            }
        }

        [HttpPost]
        [Route("UploadFiles")]
        [DisableFormValueModelBinding]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> UploadFiles(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation(nameof(UploadFiles));

            try
            {
                var boundary = HttpContext.Request.GetMultipartBoundary();
                var stream = HttpContext.Request.Body;

                var reader = new MultipartReader(boundary, stream);
                await StoreFiles(reader, _storageFolderPath, cancellationToken);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ex);
            }
        }

        private async Task StoreFiles(MultipartReader reader, string storageFolderPath, CancellationToken cancellationToken)
        {
            while (await reader.ReadNextSectionAsync(cancellationToken) is { } section)
            {
                if (!ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out var contentDisposition))
                    continue;

                if (IsFileDisposition(contentDisposition))
                {
                    var trustedFileName = WebUtility.HtmlEncode(contentDisposition.FileName);
                    if (string.IsNullOrEmpty(trustedFileName))
                    {
                        trustedFileName = Path.GetRandomFileName();
                    }

                    var filePath = Path.Combine(storageFolderPath, trustedFileName);

                    FileInfo file = new FileInfo(filePath);
                    file.Directory?.Create();

                    using (var fs = file.Open(FileMode.OpenOrCreate))
                    {
                        await section.Body.CopyToAsync(fs);
                    }
                }
            }
        }

        private bool IsFileDisposition(ContentDispositionHeaderValue header)
        {
            if (header == null)
            {
                throw new ArgumentNullException("header");
            }

            if (header.DispositionType.Equals("form-data"))
            {
                if (StringSegment.IsNullOrEmpty(header.FileName))
                {
                    return !StringSegment.IsNullOrEmpty(header.FileNameStar);
                }

                return true;
            }

            return false;
        }
    }
}
