// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;

namespace OauthServer
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> Ids =>
            new IdentityResource[]
            {
              new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Address(),
                new IdentityResource("country","this is the country they lived",new List<string>{"country"})
            };

        public static IEnumerable<ApiResource> Apis =>
            new ApiResource[]
            { 
                new ApiResource("MyApi","My api"){
                
                ApiSecrets={new Secret("api_secreat".Sha256()) }}
            };

        public static IEnumerable<Client> Clients =>
            new Client[]
            {
                new Client
                {
                    AccessTokenType=AccessTokenType.Reference,
                    AllowOfflineAccess=true,
                    RefreshTokenExpiration=TokenExpiration.Sliding,
                    UpdateAccessTokenClaimsOnRefresh=true,
                    ClientName="Client",
                    ClientId="client_id",
                    ClientSecrets={new Secret("client_secreat".Sha256()) },
                    AllowedGrantTypes=GrantTypes.Code,
                    RedirectUris=new List<string>()
                    {
                        "https://localhost:44353/signin-oidc"
                    },
                    PostLogoutRedirectUris = new List<string>()
                    {
                        "https://localhost:44353/signout-callback-oidc"
                    },
                    RequirePkce=true,
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Address,
                        "MyApi",
                        "country"
                    }
                }
            };

    }
}