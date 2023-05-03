using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;

namespace Api.AspNet.Tests
{
    public class TestClientProvider : IDisposable
    {
        public HttpClient Client { get; private set; }

        private TestServer _server;

        public TestClientProvider()
        {
            _server = new TestServer(new WebHostBuilder().UseStartup<Startup>());

            var httpClient = _server.CreateClient();
            Client = httpClient;
        }

        public void Dispose()
        {
            _server?.Dispose();
        }
    }
}