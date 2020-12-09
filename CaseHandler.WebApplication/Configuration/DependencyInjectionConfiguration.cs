using CaseHandler.WebApplication.Services;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CaseHandler.WebApplication.Configuration
{
    public static class DependencyInjectionConfiguration
    {
        public static IServiceCollection ConfigureDependencyInjection(this IServiceCollection services)
        {                        
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IHistoryService, HistoryService>();
            services.AddTransient<IEmailSender, EmailSender>();

            return services;
        }
    }
}