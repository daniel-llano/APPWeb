using Microsoft.Extensions.DependencyInjection;
using APPWeb.Application.Interfaces;
using APPWeb.Application.Services;
using APPWeb.Domain.Ports;

namespace APPWeb.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IProductService, ProductAppService>();
        return services;
    }
}
