using IdentityServer.Helpers;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;
using System;

namespace IdentityServer
{
    public class Startup
    {
        private readonly MyConfiguration _myConfiguration;

        private IWebHostEnvironment Environment { get; }
        private IConfiguration Configuration { get; }

        public Startup(IWebHostEnvironment environment, IConfiguration configuration)
        {
            Environment = environment;
            Configuration = configuration;

            _myConfiguration = Configuration.GetSection("MyConfiguration").Get<MyConfiguration>();    
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // Services
            services.AddTransient<IProfileService, MyProfileService>();
            services.AddTransient<ITokenService, MyTokenService>();

            // CORS
            services.AddCors();

            // Add IdentityServer
            //var builder = services.AddIdentityServer(options => {
            //    options.Events.RaiseErrorEvents = true;
            //    options.Events.RaiseInformationEvents = true;
            //    options.Events.RaiseFailureEvents = true;
            //    options.Events.RaiseSuccessEvents = true;
            //    options.EmitStaticAudienceClaim = true;
            //})
            //        .AddInMemoryClients(Config.GetClients())
            //        .AddInMemoryIdentityResources(Config.GetIdentityResources())
            //        .AddInMemoryApiResources(Config.GetApiResources())
            //        .AddInMemoryApiScopes(Config.GetApiScopes())
            //        .AddTestUsers(Config.GetUsers())
            //        .AddProfileService<MyProfileService>();

            var builder = services.AddMyIdentityServer(_myConfiguration);

            // Signing Key
            if (Environment.IsDevelopment())
            {
                builder.AddDeveloperSigningCredential();
            }
            else
            {
                throw new Exception("Configure key material");
            }

            // Authorization & Authentication
            services.AddAuthorization();

            // Facebook Authentication
            //services.AddAuthentication()
            //    .AddFacebook("Facebook", options => {

            //        options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;

            //        options.AppId = "";
            //        options.AppSecret = "";
            //    });
            services.AddMyAuthentication(_myConfiguration);

            // HTTP Only Cookies
            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = true;
            });
            
            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            // Security Headers
            app.UseSecurityHeaders(GetHeaderPolicyCollection());

            if (Environment.IsDevelopment())
            {
                // Extended IdentityServer Logging
                IdentityModelEventSource.ShowPII = true;

                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("Home/Error")
                    .UseHsts();
            }

            // CORS - Sepcify origins if possible
            app.UseCors(options => options.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            
            app.UseIdentityServer();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => endpoints.MapDefaultControllerRoute());
        }

        private static HeaderPolicyCollection GetHeaderPolicyCollection()
        {
            return new HeaderPolicyCollection().AddPermissionsPolicy(
                                                                     builder =>
                                                                     {
                                                                         builder.AddAccelerometer().None();
                                                                         builder.AddAutoplay().None();
                                                                         builder.AddCamera().None();
                                                                         builder.AddFullscreen().All();
                                                                         builder.AddGeolocation().None();
                                                                         builder.AddGyroscope().None();
                                                                         builder.AddMagnetometer().None();
                                                                         builder.AddMicrophone().None();
                                                                         builder.AddMidi().None();
                                                                         builder.AddPayment().None();
                                                                         builder.AddUsb().None();
                                                                     });
        }
    }
}
