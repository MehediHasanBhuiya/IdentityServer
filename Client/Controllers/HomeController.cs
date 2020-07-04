using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Client.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using System.Net.Http;
using IdentityModel.Client;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace Client.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpClientFactory http;

        public HomeController(ILogger<HomeController> logger,IHttpClientFactory http)
        {
            _logger = logger;
            this.http = http;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public async Task LogOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
        }
        [Authorize(Policy = "HaveCountry")]
        public async Task<string> Address()
        {
            var httpclientreq = http.CreateClient("server");
            var metadata = await httpclientreq.GetDiscoveryDocumentAsync();

            var accessToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);

            var userInforresponse = await httpclientreq.GetUserInfoAsync(
                new UserInfoRequest
                {
                    Address = metadata.UserInfoEndpoint,
                    Token = accessToken
                });
            var address = userInforresponse.Claims.FirstOrDefault(c => c.Type == "address").Value;

            return address;
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
        public async Task<IActionResult> WeatherInfo()
        {
            var response = http.CreateClient("api");
            var x = await response.SendAsync(new HttpRequestMessage(HttpMethod.Get,response.BaseAddress));
            x.EnsureSuccessStatusCode();
            var content = await x.Content.ReadAsStringAsync();
            return Json(content);
        }
        
    }
}
