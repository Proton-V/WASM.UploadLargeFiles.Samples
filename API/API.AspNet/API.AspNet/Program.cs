var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var startup = new Startup(builder.Environment);
startup.ConfigureServices(builder.Services);

var app = builder.Build();
startup.Configure(app);

app.Run();
