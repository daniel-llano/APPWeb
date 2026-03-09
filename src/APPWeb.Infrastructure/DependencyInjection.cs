using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using APPWeb.Application.Interfaces;
using APPWeb.Domain.Ports;
using APPWeb.Infrastructure.Http;
using APPWeb.Infrastructure.Services;

namespace APPWeb.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        var apiBaseUrl = config["ApiSettings:BaseUrl"] ?? "https://localhost:7100";

        services.AddHttpClient<IAuthApiClient, AuthApiClient>(client =>
            client.BaseAddress = new Uri(apiBaseUrl));

        services.AddHttpClient<IProductApiClient, ProductApiClient>(client =>
            client.BaseAddress = new Uri(apiBaseUrl));

        services.AddHttpClient<IUserApiClient, UserApiClient>(client =>
            client.BaseAddress = new Uri(apiBaseUrl));

        services.AddScoped<IAuthService, AuthService>();

        return services;
    }
}
