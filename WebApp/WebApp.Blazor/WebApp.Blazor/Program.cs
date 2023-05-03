using API.AspNet.Contracts;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using WebApp.Blazor;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var apiAddress = "http://localhost:5209/";
var httpClient = new HttpClient() { BaseAddress = new Uri(apiAddress) };
builder.Services.AddScoped(x => new ApiClient(apiAddress, httpClient));

await builder.Build().RunAsync();
