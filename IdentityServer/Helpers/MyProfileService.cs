using IdentityServer4.Models;
using IdentityServer4.Services;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityServer.Helpers
{
    public class MyProfileService : IProfileService
    {
        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var claims = new List<Claim>();

            claims.Add(new Claim(ClaimTypes.Role, "admin"));
            claims.Add(new Claim("test-claim", "test-value"));

            context.IssuedClaims = claims;

            await Task.CompletedTask;
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            context.IsActive = true;

            await Task.CompletedTask;
        }
    }
}
