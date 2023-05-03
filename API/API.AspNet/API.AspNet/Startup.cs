public class Startup
{
    const string DevCorsPolicy = "DevelopmentCorsPolicy";

    private readonly IWebHostEnvironment _environment;

    public Startup(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddCors(options =>
        {
            options.AddPolicy(DevCorsPolicy, builder => {
                builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
            });
        });
    }

    public void Configure(IApplicationBuilder app)
    {
        // Configure the HTTP request pipeline.
        if (_environment.IsDevelopment())
        {
            app.UseCors(DevCorsPolicy);

            app.UseSwagger();
            app.UseSwaggerUI();
        }

        //app.UseAuthorization();
        app.UseRouting();
        app.UseEndpoints(x => x.MapControllers());
    }
}