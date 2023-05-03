using Microsoft.AspNetCore.Http;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Xunit.Abstractions;

namespace Api.AspNet.Tests
{
    public class FileUploaderControllerTests
    {
        private readonly ITestOutputHelper _testOutputHelper;
        public FileUploaderControllerTests(ITestOutputHelper outputHelper)
        {
            _testOutputHelper = outputHelper;
        }

        [Fact]
        public async Task UploadFilesToServerAsync_WithRandomData_RetunrsOk()
        {
            // random data
            var files = Enumerable.Range(0, 100)
                .Select(x => 
            {
                byte[] filebytes = Encoding.UTF8.GetBytes("dummy file");
                IFormFile file = new FormFile(new MemoryStream(filebytes), 0, filebytes.Length, $"Data{x}", $"image{x}.data");
                return file;
            });

            try
            {
                using (var multipartFormContent = new MultipartFormDataContent())
                {
                    foreach (var file in files)
                    {
                        var fullFileName = file.Name;
                        var fileName = Path.GetFileNameWithoutExtension(fullFileName);

                        var fileStreamContent = new PushStreamContent(async (stream, httpContent, transportContext) =>
                        {
                            using (var fileStream = file.OpenReadStream())
                            {
                                await fileStream.CopyToAsync(stream);
                            }
                            stream.Close();
                        });


                        fileStreamContent.Headers.ContentType = new MediaTypeWithQualityHeaderValue("multipart/form-data");
                        multipartFormContent.Add(fileStreamContent, name: fileName, fileName: fullFileName);
                    }

                    using (var request = new HttpRequestMessage())
                    {
                        request.Content = multipartFormContent;
                        request.Method = new HttpMethod("POST");
                        request.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json"));
                        request.RequestUri = new Uri("FileUploader/UploadFiles", UriKind.RelativeOrAbsolute);

                        using (var httpClient = new TestClientProvider().Client)
                        {
                            var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead);

                            var msg = await response.Content.ReadAsStringAsync();
                            _testOutputHelper.WriteLine(msg);

                            Assert.StrictEqual(HttpStatusCode.OK, response.StatusCode);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _testOutputHelper.WriteLine(ex.Message);
                Assert.Fail(ex.Message);
            }
        }
    }
}
