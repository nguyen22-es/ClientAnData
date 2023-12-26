using Client.Services;
using ClientChat.Models;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using WebchatSignalr.ViewModel;

namespace ClientChat.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly HttpClient _httpClient;
        private readonly ITokenService _TokenService;
        private readonly IConfiguration _config;
        public HomeController(ILogger<HomeController> logger, IHttpContextAccessor httpContextAccessor, HttpClient httpClient, ITokenService tokenService, IConfiguration config)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _httpClient = httpClient;
            _TokenService = tokenService;
            _config = config;
        }
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var tokenResponse = await _TokenService.GetToken("CoffeeAPI.read");
            _httpClient.SetBearerToken(tokenResponse.AccessToken);
            var uriBuilder = new UriBuilder(_config["apiUrl"] + "/api/User/AllUser");
            var query = System.Web.HttpUtility.ParseQueryString(uriBuilder.Query);        
            uriBuilder.Query = query.ToString();
            var result = await _httpClient.GetAsync(uriBuilder.ToString());




            if (result.IsSuccessStatusCode)
            {
                var userViewModel = await result.Content.ReadFromJsonAsync<List<UserViewModel>>();
                return View(userViewModel);
            }

            return View(result);

        }
        public async Task<IActionResult> Update()
        {
            var user = _httpContextAccessor.HttpContext.User;
            var ID = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;


            var tokenResponse = await _TokenService.GetToken("CoffeeAPI.read");
            _httpClient.SetBearerToken(tokenResponse.AccessToken);

            var result = await _httpClient.GetAsync("https://localhost:7191/api/User?id=" + ID);



            if (result.IsSuccessStatusCode)
            {
                return  RedirectToAction("Index");
    
            }
            else
            {
                return View();
            }

         
          
        }
        public IActionResult UpdateA()
        {

            return RedirectToAction("Update");

        }


            [HttpPost]
        public async Task<IActionResult> Create(UserViewModel model)
        {
            var user = _httpContextAccessor.HttpContext.User;
            var ID = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            model.ID = ID;
            try
            {
                var tokenResponse = await _TokenService.GetToken("CoffeeAPI.read");
                _httpClient.SetBearerToken(tokenResponse.AccessToken);
                var apiEndpoint = "https://localhost:7191/api/User";

                var request = new HttpRequestMessage(HttpMethod.Post, apiEndpoint);

                request.Content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");


                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();
                    return RedirectToAction("Index");

                }
                else
                {
                    // Xử lý trường hợp lỗi từ API
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, errorResponse);
                }

            }
            catch (Exception ex)
            {
                // Xử lý lỗi xảy ra trong quá trình gọi API
                return StatusCode(500, ex.Message);
            }

           
        }



        public async Task<IActionResult> GetUser(string roomID)
         {

          


            var tokenResponse = await _TokenService.GetToken("CoffeeAPI.read");
            _httpClient.SetBearerToken(tokenResponse.AccessToken);
            var uriBuilder = new UriBuilder(_config["apiUrl"] + "/api/User/All");
            var query = System.Web.HttpUtility.ParseQueryString(uriBuilder.Query);
            query["roomID"] = roomID;
            uriBuilder.Query = query.ToString();
            var result = await _httpClient.GetAsync(uriBuilder.ToString());




            if (result.IsSuccessStatusCode)
            {
                var userViewModel = await result.Content.ReadFromJsonAsync<List<UserViewModel>>();
                return Json(userViewModel);
            }

            return Json(result);

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
    }
}
