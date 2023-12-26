using Client.Services;
using IdentityModel.Client;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Text;
using WebchatSignalr.ViewModel;

namespace ClientChat.Controllers
{
    public class MessageController : Controller
    {


        private readonly ILogger<HomeController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly HttpClient _httpClient;
        private readonly ITokenService _TokenService;
        private readonly IConfiguration _config;
        public MessageController(ILogger<HomeController> logger, IHttpContextAccessor httpContextAccessor, HttpClient httpClient, ITokenService tokenService, IConfiguration config)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _httpClient = httpClient;
            _TokenService = tokenService;
            _config = config;
        }



        [HttpGet]
        public async Task<IActionResult> GetMessageRoom(string RoomID) // lấy ra những tin nhắn có trong phòng chat
        {
          


            var tokenResponse = await _TokenService.GetToken("CoffeeAPI.read");
            _httpClient.SetBearerToken(tokenResponse.AccessToken);

            //
            var uriBuilder = new UriBuilder(_config["apiUrl"] + "/api/Messages/Room");
            var query = System.Web.HttpUtility.ParseQueryString(uriBuilder.Query);
            query["id"] = RoomID;
            uriBuilder.Query = query.ToString();
            var result = await _httpClient.GetAsync(uriBuilder.ToString());

            //
            if (result.IsSuccessStatusCode)
            {
                var messageViewModels = await result.Content.ReadFromJsonAsync<List<MessageViewModel>>();

                return Json(messageViewModels);
            }
            return View(null);
        }

        [HttpGet]
        public async Task<IActionResult> GetChatUser(string id)
        {



            var tokenResponse = await _TokenService.GetToken("CoffeeAPI.read");
            _httpClient.SetBearerToken(tokenResponse.AccessToken);
      
      
            var uriBuilder = new UriBuilder(_config["apiUrl"] + "/api/Messages/id");
            var query = System.Web.HttpUtility.ParseQueryString(uriBuilder.Query);
            query["id"] = id;
            uriBuilder.Query = query.ToString();
            var result = await _httpClient.GetAsync(uriBuilder.ToString());

            //
            if (result.IsSuccessStatusCode)
            {
                var messageViewModels = await result.Content.ReadFromJsonAsync<List<MessageViewModel>>();

                return Json(messageViewModels);
            }
            return View(result);
        }
        public async Task<IActionResult> CreateMessageUser(string message, string chatUSerID)
        {

            var user = _httpContextAccessor.HttpContext.User;
            var ID = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

           
          
         
            var messageViewModel = new MessageViewModel
            {
                Content = message,
                NameSend = "",
                ChatUSerID = chatUSerID,
                RoomID = "",
                UserID = ID,
                Timestamp = "",
                Avatar = "",
                Room = "",

            };


            try
            {
                var tokenResponse = await _TokenService.GetToken("CoffeeAPI.read");
                _httpClient.SetBearerToken(tokenResponse.AccessToken);
                var apiEndpoint = _config["apiUrl"] + "/api/Messages";

                var request = new HttpRequestMessage(HttpMethod.Post, apiEndpoint);

                request.Content = new StringContent(JsonConvert.SerializeObject(messageViewModel), Encoding.UTF8, "application/json");


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



        public async Task<IActionResult> CreateMessageRoom(string message, string RoomID )
        {

            var user = _httpContextAccessor.HttpContext.User;
            var ID = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;


            var messageViewModel = new MessageViewModel
            {
                Content = message,
                NameSend = "",
                ChatUSerID = "",
                RoomID = RoomID,
                UserID = ID,
                Timestamp = "",
                Avatar = "",
                Room = "",
                
            };




          
                var tokenResponse = await _TokenService.GetToken("CoffeeAPI.read");
                _httpClient.SetBearerToken(tokenResponse.AccessToken);
                var apiEndpoint = _config["apiUrl"] + "/api/Messages/chatRoom";

                var request = new HttpRequestMessage(HttpMethod.Post, apiEndpoint);

                request.Content = new StringContent(JsonConvert.SerializeObject(messageViewModel), Encoding.UTF8, "application/json");


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



        public IActionResult Index()
        {
            return View();
        }
    }
}
