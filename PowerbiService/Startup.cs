using PowerbiService.Models;
using PowerbiService.Services;

namespace PowerbiService;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container
    public void ConfigureServices(IServiceCollection services)
    {
        // Loading appsettings.json in C# Model classes
        services.Configure<AzureAd>(Configuration.GetSection("AzureAd"))
                .Configure<PowerBI>(Configuration.GetSection("PowerBI"));
        // Register AadService and PbiEmbedService for dependency injection
        services.AddScoped(typeof(AadService))
                .AddScoped(typeof(PbiEmbedService));

        services.AddControllers();

        services.AddCors(options =>
           options.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyMethod()));

    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseHttpsRedirection();

        // Needed for the lambda function with common api gateway to work properly with the api
        app.UsePathBase(new PathString("/PowerbiService"));

        app.UseRouting();

        app.UseCors();

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapGet("/", async context =>
            {
                await context.Response.WriteAsync("Welcome to running ASP.NET Core on AWS Lambda");
            });
        });
    }
}