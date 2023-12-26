
using Client.Services;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Client.Controllers
{
    [Authorize]
    public class TokenController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITokenService _TokenService;
        public TokenController(HttpClient httpClient, IConfiguration config, IHttpContextAccessor httpContextAccessor, ITokenService tokenService)
        {
          
            _httpClient = httpClient;
            _config = config;
            _httpContextAccessor = httpContextAccessor;

            _TokenService = tokenService;

        }
   
        public async Task<ActionResult> GetsToken()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var user = _httpContextAccessor.HttpContext.User;
            var tokenResponse = await _TokenService.GetToken("CoffeeAPI.read");
            _httpClient.SetBearerToken(tokenResponse.AccessToken);


            return Json(accessToken);
        }
    }
}
