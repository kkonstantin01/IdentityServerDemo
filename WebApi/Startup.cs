using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using WebApi.Models;

namespace WebApi
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddAuthentication(options =>
                {
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                })
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
                {
                    ConfigureOpenIdConnect(options);
                })
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    ConfigureJwtBearer(options);
                });

            services.AddDbContext<CustomerContext>(options => options.UseInMemoryDatabase("CustomerDb"));

            services.AddControllers();

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "WebApi.API", Version = "v1" });
            });
        }

        private static void ConfigureOpenIdConnect(OpenIdConnectOptions options)
        {
            options.Authority = "https://localhost:5001/";

            options.ClientId = "oidcSwaggerClient";
            options.ClientSecret = "secret";
            options.SaveTokens = true;
            options.ResponseType = "code";
            options.UseTokenLifetime = true;
            options.UsePkce = true;
        }

        private static void ConfigureJwtBearer(JwtBearerOptions options)
        {
            options.Authority = "https://localhost:5001";
            options.Audience = "api1";

            options.TokenValidationParameters = new TokenValidationParameters
            {
                RequireAudience = true,
                RequireExpirationTime = true,
                ValidateAudience = true,
                ValidateActor = true,
                ValidateLifetime = true,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                RoleClaimType = "role",
                NameClaimType = "name"
            };
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthentication(); // 1!!
            app.UseAuthorization(); // 2!!

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseEndpoints(endpoints =>
            {
                ConfigureEndpoints(endpoints);
            });
        }

        private static void ConfigureEndpoints(Microsoft.AspNetCore.Routing.IEndpointRouteBuilder endpoints)
        {
            endpoints.MapControllers()
                                     .RequireAuthorization(new AuthorizeAttribute { AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme }); ;

            var pipeline = endpoints.CreateApplicationBuilder().Build();
            var oidcAuthAttr = new AuthorizeAttribute { AuthenticationSchemes = OpenIdConnectDefaults.AuthenticationScheme };
            endpoints
                .Map("/swagger/{documentName}/swagger.json", pipeline)
                .RequireAuthorization(oidcAuthAttr);
            endpoints
                .Map("/swagger/index.html", pipeline)
                .RequireAuthorization(oidcAuthAttr);
        }
    }
}
