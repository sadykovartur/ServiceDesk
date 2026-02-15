using Microsoft.Extensions.DependencyInjection;
using ServiceDesk.Application.Interfaces.Services;
using ServiceDesk.Application.Services;

namespace ServiceDesk.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<ITicketService, TicketService>();
        return services;
    }
}
