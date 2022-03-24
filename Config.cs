// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace IdentityServerAspNetIdentity
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
                new IdentityResource
                {
                    Name = "roles",
                    DisplayName = "Roles",
                    Description = "User roles",
                    UserClaims = { JwtClaimTypes.Role }
                }
            };

         public static IEnumerable<ApiScope> ApiScopes =>
            new List<ApiScope>
            {
                new ApiScope("ProductService.Write","Ürün servisi yazma izni."),
                new ApiScope("ProductService.Read","Ürün servisi okuma izni.")
            };
            
        public static IEnumerable<ApiResource> ApiResources =>
            new List<ApiResource>
            {
                new ApiResource("ProductService")
                { 
                    Scopes = {"ProductService.Write", "ProductService.Read" },
                    ApiSecrets = { new Secret("product-service".Sha256()) },
                    UserClaims = new []{ JwtClaimTypes.Role, JwtClaimTypes.Email } 
                }
            };
       
        public static IEnumerable<Client> Clients =>
            new List<Client>
            {
                // SPA
                new Client
                {
                    ClientId = "AngularClient",
                    ClientName = "Angular Client",
                    RequireClientSecret = false,
                    AllowedScopes = {
                        "ProductService.Write",
                        "ProductService.Read",
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Email,
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        "roles"
                    },
                    RedirectUris = {"http://localhost:4200/signin-callback", "http://localhost:4200/silent-callback"},
                    AllowedCorsOrigins = {"http://localhost:4200"},
                    PostLogoutRedirectUris = {"http://localhost:4200"},
                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true,
                    //RequireConsent = true
                    AllowOfflineAccess = true,
                    RefreshTokenUsage = TokenUsage.ReUse,
                    RefreshTokenExpiration = TokenExpiration.Absolute,
                    AbsoluteRefreshTokenLifetime = 96000,
                    AccessTokenLifetime = 3600
                },
                // Client Credentals
                new Client
                {
                    ClientId = "client",

                    // no interactive user, use the clientid/secret for authentication
                    AllowedGrantTypes = GrantTypes.ClientCredentials,

                    // secret for authentication
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },

                    // scopes that client has access to
                    AllowedScopes = {
                        "ProductService.Write",
                        "ProductService.Read",
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Email,
                        "roles"
                    },
                },
                // resource owner password grant client
                new Client
                {
                    ClientId = "ResourceOwnerClient",
                    ClientName = "Resource Owner Client",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,

                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    AllowedScopes = {
                        "ProductService.Write",
                        "ProductService.Read",
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Email,
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        "roles"
                    },
                    AllowOfflineAccess = true,
                    AccessTokenLifetime = 60 * 60,
                    RefreshTokenUsage = TokenUsage.ReUse,
                    RefreshTokenExpiration = TokenExpiration.Absolute,
                    AbsoluteRefreshTokenLifetime = (60 * 60) + 900,
                    RequirePkce = false,                    
                },
                // OpenID Connect hybrid flow client (MVC)
                new Client
                {
                    ClientId = "mvc",
                    ClientName = "MVC Client",
                    AllowedGrantTypes = GrantTypes.Hybrid,

                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },

                    RedirectUris           = { "http://localhost:5002/signin-oidc" },
                    PostLogoutRedirectUris = { "http://localhost:5002/signout-callback-oidc" },

                    AllowedScopes = {
                        "ProductService.Write",
                        "ProductService.Read",
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Email,
                        "roles"
                    },

                    AllowOfflineAccess = true
                },
                // JavaScript Client
                new Client
                {
                    ClientId = "js",
                    ClientName = "JavaScript Client",
                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true,
                    RequireClientSecret = false,

                    RedirectUris =           { "http://localhost:5003/callback.html" },
                    PostLogoutRedirectUris = { "http://localhost:5003/index.html" },
                    AllowedCorsOrigins =     { "http://localhost:5003" },

                    AllowedScopes =
                    {
                        "ProductService.Write",
                        "ProductService.Read",
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Email,
                        "roles"
                    }
                }
            };

        public static IEnumerable<TestUser> Users =>
            new List<TestUser>
            {
                new TestUser
                {
                    SubjectId = "test-user1",
                    Username = "test-user1",
                    Password = "12345",
                    Claims =
                    {
                        new Claim(JwtRegisteredClaimNames.Email, "test-user@gmail.com"),
                        new Claim(JwtRegisteredClaimNames.FamilyName, "User"),
                        new Claim(JwtRegisteredClaimNames.GivenName, "Test"),
                        new Claim("role", "admin"),
                        new Claim("authority","true")
                    }
                }
            };
    }
}