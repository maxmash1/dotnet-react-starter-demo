using System.Reflection;
using Api.Data;
using Api.Extensions;
using Api.Middleware;

var applicationBuilder = WebApplication.CreateBuilder(args);

// Register application services
applicationBuilder.Services.AddApplicationServices();

// Configure API controllers with JSON options
applicationBuilder.Services.AddControllers()
    .AddJsonOptions(jsonOptions =>
    {
        jsonOptions.JsonSerializerOptions.PropertyNamingPolicy = null;
    });

// Configure Swagger/OpenAPI documentation
applicationBuilder.Services.AddEndpointsApiExplorer();
applicationBuilder.Services.AddSwaggerGen(swaggerOptions =>
{
    swaggerOptions.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Organization API",
        Version = "v1",
        Description = "RESTful API following organization development standards"
    });

    // Include XML documentation comments in Swagger
    var assemblyXmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var assemblyXmlPath = Path.Combine(AppContext.BaseDirectory, assemblyXmlFile);
    if (File.Exists(assemblyXmlPath))
    {
        swaggerOptions.IncludeXmlComments(assemblyXmlPath);
    }
});

// Configure CORS for frontend development
applicationBuilder.Services.AddCors(corsOptions =>
{
    corsOptions.AddDefaultPolicy(corsPolicy =>
    {
        corsPolicy
            .WithOrigins(
                "http://localhost:5173",
                "http://localhost:3000"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .WithExposedHeaders("X-Transaction-Id");
    });
});

var webApplication = applicationBuilder.Build();

// Ensure InMemory database is created and seeded
using (var scope = webApplication.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.EnsureCreated();
}

// Enable Swagger in development
if (webApplication.Environment.IsDevelopment())
{
    webApplication.UseSwagger();
    webApplication.UseSwaggerUI(swaggerUiOptions =>
    {
        swaggerUiOptions.SwaggerEndpoint("/swagger/v1/swagger.json", "Organization API v1");
    });
}

// Configure middleware pipeline
webApplication.UseCors();
webApplication.UseTransactionId();
webApplication.UseAuthorization();
webApplication.MapControllers();

// Redirect root URL to Swagger UI for developer convenience
webApplication.MapGet("/", () => Results.Redirect("/swagger"));

webApplication.Run();

// Make Program class accessible for integration tests
public partial class Program { }
