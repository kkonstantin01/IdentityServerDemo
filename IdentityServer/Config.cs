using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System.Collections.Generic;
using System.Security.Claims;

namespace IdentityServer
{
    internal static class Config
    {
        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                // Client Credentials Flow
                new Client
                {
                    ClientId = "oauthClient",
                    ClientName = "Example client application using client credentials",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = { new Secret("secret".Sha256()) }, // change me!
                    AllowedScopes = { "api1.read" },
                    Enabled = true
                },
                // Resource Owner and Password Flow
                new Client
                {
                    ClientId = "oauthClient2",
                    ClientSecrets = { new Secret("secret".Sha256()) }, // change me!
                    RequireClientSecret = true,
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    AllowedScopes = { "api1.read" },
                    Enabled = true
                },
                // Authorization Code + PKCE Flow
                new Client
                {
                    ClientId = "oidcClient",
                    ClientName = "Example Client Application",
                    ClientSecrets = { new Secret("secret".Sha256()) }, // change me!
    
                    RedirectUris = { "https://localhost:44304/signin-oidc" },
                    PostLogoutRedirectUris = { "https://localhost:44304/signout-callback-oidc" },

                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true,
                    RequireClientSecret = true,
                    AllowOfflineAccess = true, // refresh token
                    
                    AllowedScopes = 
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        IdentityServerConstants.StandardScopes.OfflineAccess, // refresh token
                        "roles",
                        "api1.read"
                    },

                    AllowPlainTextPkce = false,
                    // Include Claims in IdToken
                    //AlwaysIncludeUserClaimsInIdToken = true,

                    Enabled = true
                },
                // Authorization Code + PKCE Flow
                new Client {
                    ClientId = "oidcPostmanClient",
                    ClientName = "Example Postman OIDC client",
                    ClientSecrets = { new Secret("secret".Sha256()) }, // change me!
                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true,
                    RequireClientSecret = true,
                    RedirectUris = { "https://oauth.pstmn.io/v1/callback" },
                    AllowedScopes = 
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "api1.read"
                    },
                    AllowAccessTokensViaBrowser = true,
                    AlwaysIncludeUserClaimsInIdToken = true,
                    Enabled = true
                },
                // Authorization Code + PKCE Flow
                new Client
                {
                    ClientId = "oidcSwaggerClient",
                    ClientSecrets = { new Secret("secret".Sha256()) }, // change me!
                    AllowedGrantTypes = GrantTypes.Code,
                    RedirectUris = { "https://localhost:44326/signin-oidc" },
                    PostLogoutRedirectUris = { "https://localhost:44326/signout-callback-oidc" },
                    RequirePkce = true,
                    RequireClientSecret = true,
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "api1.read"
                    },
                    Enabled = true
                }
            };
        }

        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
                new IdentityResource
                {
                    Name = "roles",
                    UserClaims = new List<string> { ClaimTypes.Role }
                }
            };
        }

        public static IEnumerable<ApiScope> GetApiScopes()
        {
            return new[]
            {
                new ApiScope("test-claim2", "role test"),
                new ApiScope("api1.read", "Read Access to API #1"),
                new ApiScope("api1.write", "Write Access to API #1")
            };
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new[]
            {
                new ApiResource
                {
                    Name = "api1",
                    DisplayName = "API #1",
                    Description = "Allow the application to access API #1 on your behalf",
                    Scopes = { "api1.read", "api1.write" },
                    ApiSecrets = { new Secret("ScopeSecret".Sha256()) },
                    UserClaims = { ClaimTypes.Role }
                }
            };
        }

        public static List<TestUser> GetUsers()
        {
            return new List<TestUser> {
                new TestUser {
                    SubjectId = "5BE86359-073C-434B-AD2D-A3932222DABE",
                    Username = "admin",
                    Password = "password",
                    Claims = new List<Claim> {
                        new Claim(ClaimTypes.Email, "admin@test.com"),
                        new Claim(ClaimTypes.Role, "admin")
                    }
                }
            };
        }
    }
}