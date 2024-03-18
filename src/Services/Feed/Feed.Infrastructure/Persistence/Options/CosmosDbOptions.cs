namespace Feed.Infrastructure.Persistence.Options;

public class CosmosDbOptions
{
    public string EndpointUrl { get; set; } = null!;
    public string DatabaseName { get; set; } = null!;
}