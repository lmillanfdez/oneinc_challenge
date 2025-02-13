using OneIncChallenge.Repositories.Contracts;
using OneIncChallenge.Repositories.Implementations;
using OneIncChallenge.Services.Contracts;
using OneIncChallenge.Services.Implementations;

namespace OneIncChallenge.Configurations;

internal static class DependencyInjections
{
    internal static void ConfigureDependencyInjections(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IUsersService, UsersService>();
        serviceCollection.AddScoped<IUsersRepository, UsersRepository>();
    } 
}