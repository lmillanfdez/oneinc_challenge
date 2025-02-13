namespace OneIncChallenge.Configurations;

internal static class AppConfigurations
{
    internal static void LoadAppConfigurations(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.Configure<ConnectionsStrings>(configuration.GetSection("ConnectionStrings"));
    }
}