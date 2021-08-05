using IdentityServer4.Configuration;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace IdentityServer.Helpers
{
    public class MyTokenService : DefaultTokenService
    {
        public MyTokenService(IClaimsService claimsProvider, IReferenceTokenStore referenceTokenStore, ITokenCreationService creationService, IHttpContextAccessor contextAccessor, ISystemClock clock, IKeyMaterialService keyMaterialService, IdentityServerOptions options, ILogger<DefaultTokenService> logger) : base(claimsProvider, referenceTokenStore, creationService, contextAccessor, clock, keyMaterialService, options, logger)
        {
        }

        public override async Task<Token> CreateAccessTokenAsync(TokenCreationRequest request)
        {
            Token result = await base.CreateAccessTokenAsync(request);
            // Your logging goes here...
            return result;
        }
    }
}
