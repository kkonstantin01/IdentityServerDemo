using IdentityServer4;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServer.Helpers
{
    public static class ConfigureAuthentication 
    {
        public static IServiceCollection AddMyAuthentication(this IServiceCollection services, MyConfiguration myConfiguration)
        {
            services.AddAuthentication()
                .AddFacebook("Facebook", options => {

                    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;

                    options.AppId = "";
                    options.AppSecret = "";
                });

            return services;
        } 
    }
}
