using Azure.Identity;
using FileUpload.API.Options;
using FileUpload.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Azure;

namespace FileUpload.API;

internal class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        var fileUploadSection = Configuration.GetSection("FileUpload");
        services.Configure<FileUploadOptions>(fileUploadSection);

        var fileUploadOptions = new FileUploadOptions();
        fileUploadSection.Bind(fileUploadOptions);

        services.AddControllers(options =>
        {
            options.Filters.Add(new RequestSizeLimitAttribute(fileUploadOptions.ChunkSizeInBytes));
        });
        
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
            {
                builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });
        
        services.AddScoped<IChunkedFileUploadService, ChunkedFileUploadService>();
        
        var blobStorageSection = Configuration.GetSection("BlobStorage");
        var blobStorageOptions = new BlobStorageOptions();
        blobStorageSection.Bind(blobStorageOptions);
        
        services.Configure<BlobStorageOptions>(blobStorageSection);

        services.AddAzureClients(clientBuilder =>
        {
            clientBuilder.AddBlobServiceClient(new Uri(blobStorageOptions.Uri));
            clientBuilder.UseCredential(new DefaultAzureCredential());
        });
    }

    public void Configure(WebApplication app, IWebHostEnvironment environment)
    {
        if (environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseCors();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();
    }
}
