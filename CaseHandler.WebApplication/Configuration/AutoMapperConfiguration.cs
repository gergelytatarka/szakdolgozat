using AutoMapper;
using CaseHandler.WebApplication.AutoMapperProfiles;
using Microsoft.Extensions.DependencyInjection;

namespace CaseHandler.WebApplication.Configuration
{
    public static class AutoMapperConfiguration
    {
        public static IServiceCollection ConfigureAutoMapper(this IServiceCollection services)
        {
            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new CaseProfile());
                mc.AddProfile(new CommentProfile());
                mc.AddProfile(new HistoryProfile());
                mc.AddProfile(new UserProfile());
            });
            var mapper = mapperConfig.CreateMapper();
            services.AddSingleton(mapper);

            return services;
        }
    }
}
