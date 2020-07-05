using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client.HttpHandlers
{
    public class BearerTokenHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor httpContext;
        private readonly IHttpClientFactory httpClient;

        public BearerTokenHandler(IHttpContextAccessor httpContext,IHttpClientFactory httpClient)
        {
            this.httpContext = httpContext;
            this.httpClient = httpClient;
        }
        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var accessToken = await GetTokenAsync();
            if (!string.IsNullOrWhiteSpace(accessToken))
            {
                request.SetBearerToken(accessToken);
            }

            return await base.SendAsync(request, cancellationToken);
        }

        public async Task<string> GetTokenAsync()
        {
            //get expire at value and parse it
            var expireAt = await httpContext.HttpContext.GetTokenAsync("expires_at");

            var expireatDateTime = DateTimeOffset.Parse(expireAt, CultureInfo.InvariantCulture);

            if (expireatDateTime.AddSeconds(-60).ToUniversalTime()>DateTimeOffset.UtcNow)
            {
                //token not expired. return the access token
                return await httpContext.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
            }
            var client = httpClient.CreateClient("server");

            var discoveryDocument = await client.GetDiscoveryDocumentAsync();

            //refresh the token
            var refreshToken = await httpContext.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);

            var refreshResponse = await client.RequestRefreshTokenAsync(new RefreshTokenRequest
            {
                Address = discoveryDocument.TokenEndpoint,
                ClientId = "client_id",
                ClientSecret = "client_secreat",
                RefreshToken = refreshToken
            });

            //store the token
            var updatedToken = new List<AuthenticationToken>();
            updatedToken.Add(new AuthenticationToken
            {
                Name = OpenIdConnectParameterNames.IdToken,
                Value = refreshResponse.IdentityToken
            });
            updatedToken.Add(new AuthenticationToken
            {
                Name = OpenIdConnectParameterNames.AccessToken,
                Value = refreshResponse.AccessToken
            });
            updatedToken.Add(new AuthenticationToken
            {
                Name = OpenIdConnectParameterNames.RefreshToken,
                Value = refreshResponse.RefreshToken
            });
            updatedToken.Add(new AuthenticationToken
            {
                Name = "expires_at",
                Value = (DateTime.UtcNow + TimeSpan.FromSeconds(refreshResponse.ExpiresIn)).
                ToString("o", CultureInfo.InvariantCulture)
            });

            var currentAuthenticationResult = await httpContext.HttpContext.
                AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            //store the updated token
            currentAuthenticationResult.Properties.StoreTokens(updatedToken);

            //sign in
            await httpContext.HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                currentAuthenticationResult.Principal,
                currentAuthenticationResult.Properties
                );
            return refreshResponse.AccessToken;

        }
    }
}
