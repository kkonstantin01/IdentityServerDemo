using EnsureThat;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServer.Helpers
{
    public static class ConfigureIdentityServer
    {
        public static IIdentityServerBuilder AddMyIdentityServer(this IServiceCollection services, MyConfiguration _myConfiguration)
        {
            EnsureArg.IsNotNull(services, nameof(services));

            IIdentityServerBuilder builder = services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
                options.EmitStaticAudienceClaim = true;
            })
                    .AddInMemoryClients(Config.GetClients())
                    .AddInMemoryIdentityResources(Config.GetIdentityResources())
                    .AddInMemoryApiResources(Config.GetApiResources())
                    .AddInMemoryApiScopes(Config.GetApiScopes())
                    .AddTestUsers(Config.GetUsers())
                    .AddProfileService<MyProfileService>();

            return builder;
        }
    }
}
