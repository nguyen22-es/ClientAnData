using Client.Services;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Text;
using WebchatSignalr.ViewModel;

namespace ClientChat.Controllers
{
    public class ChatRoomController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly HttpClient _httpClient;
        private readonly ITokenService _TokenService;
        private readonly IConfiguration _config;
        public ChatRoomController(ILogger<HomeController> logger, IHttpContextAccessor httpContextAccessor, HttpClient httpClient, ITokenService tokenService, IConfiguration config)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _httpClient = httpClient;
            _TokenService = tokenService;
            _config = config;
        }



        [HttpGet]
        public async Task<IActionResult> GetRooms()  // thông tin những phòng mình tham gia
        {

           
            var claim = HttpContext.User.Claims.ToList();
            var ID = claim.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier);
            var tokenResponse = await _TokenService.GetToken("CoffeeAPI.read");
            _httpClient.SetBearerToken(tokenResponse.AccessToken);
         
            //
            var uriBuilder = new UriBuilder(_config["apiUrl"] + "/api/ChatRoom");
            var query = System.Web.HttpUtility.ParseQueryString(uriBuilder.Query);
            query["UserID"] = ID.Value;
            uriBuilder.Query = query.ToString();
            var result = await _httpClient.GetAsync(uriBuilder.ToString());

            //
            if (result.IsSuccessStatusCode)
            {
                var messageViewModels = await result.Content.ReadFromJsonAsync<List<ChatRoomViewModel>>();

                return Json(messageViewModels);
            }
            return Json(result);
        }


        [HttpGet]
        public async Task<IActionResult> GetChatUsers()  // thông tin những người chat cùng mình
        {
            var tokenResponse = await _TokenService.GetToken("CoffeeAPI.read");
            _httpClient.SetBearerToken(tokenResponse.AccessToken);
            var claim = HttpContext.User.Claims.ToList();
            var ID = claim.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier);
            //
            var uriBuilder = new UriBuilder(_config["apiUrl"] + "/api/ChatUser");
            var query = System.Web.HttpUtility.ParseQueryString(uriBuilder.Query);
            query["UserID"] = ID.Value;
            uriBuilder.Query = query.ToString();
            var result = await _httpClient.GetAsync(uriBuilder.ToString());

            //
            if (result.IsSuccessStatusCode)
            {
                var messageViewModels = await result.Content.ReadFromJsonAsync<List<ChatUserViewModel>>();

                return Json(messageViewModels);
            }
            return Json(result);
        }




        [HttpGet]
        public async Task<IActionResult> GetRoomAdmin()  // thông tin những phòng mình làm admin
        {



            var tokenResponse = await _TokenService.GetToken("CoffeeAPI.read");
            _httpClient.SetBearerToken(tokenResponse.AccessToken);
            var user = _httpContextAccessor.HttpContext.User;
            var userIdClaim = user.FindFirst("sub")?.Value;
            //
            var uriBuilder = new UriBuilder(_config["apiUrl"] + "/api/ChatRoom/RoomAdmin");
            var query = System.Web.HttpUtility.ParseQueryString(uriBuilder.Query);
            query["UserID"] = userIdClaim;
            uriBuilder.Query = query.ToString();
            var result = await _httpClient.GetAsync(uriBuilder.ToString());

            //
            if (result.IsSuccessStatusCode)
            {
                var messageViewModels = await result.Content.ReadFromJsonAsync<List<ChatRoomViewModel>>();

                return Json(messageViewModels);
            }
            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> CreateRoom(string NameRoom) // tạo phòng chat
        {


            

            var tokenResponse = await _TokenService.GetToken("CoffeeAPI.read");
            _httpClient.SetBearerToken(tokenResponse.AccessToken);
            var claim = HttpContext.User.Claims.ToList();
            var ID = claim.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier);
            var room = new ChatRoomViewModel
            {
                Admin = ID.Value,
                NameRoom = NameRoom,
                RoomID = ""
                
            };          
            var apiEndpoint = _config["apiUrl"] + "/api/ChatRoom";
            var request = new HttpRequestMessage(HttpMethod.Post, apiEndpoint);

            request.Content = new StringContent(JsonConvert.SerializeObject(room), Encoding.UTF8, "application/json");


            var response = await _httpClient.SendAsync(request);

            //
            if (response.IsSuccessStatusCode)
            {
                var messageViewModels = await response.Content.ReadFromJsonAsync<ChatRoomViewModel>();

                return Json(messageViewModels);
            }
            var errorResponse = await response.Content.ReadAsStringAsync();
            return StatusCode((int)response.StatusCode, errorResponse);
        }

        [HttpGet]
        public async Task<IActionResult> CreateChatUser(string FriendID) 
        {

            var room = new ChatRoomViewModel();


             var tokenResponse = await _TokenService.GetToken("CoffeeAPI.read");
             _httpClient.SetBearerToken(tokenResponse.AccessToken);

            var claim = HttpContext.User.Claims.ToList();
            var ID = claim.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier);

            var apiEndpoint = _config["apiUrl"] + "/api/ChatUser?ID="+ FriendID;
             var request = new HttpRequestMessage(HttpMethod.Post, apiEndpoint);

             request.Content = new StringContent(JsonConvert.SerializeObject(ID.Value), Encoding.UTF8, "application/json");
     
            var response = await _httpClient.SendAsync(request);

            //
            if (response.IsSuccessStatusCode)
            {
                var messageViewModels = await response.Content.ReadFromJsonAsync<List<MessageViewModel>>();
                return Json(messageViewModels);
            }
            var errorResponse = await response.Content.ReadAsStringAsync();
            return StatusCode((int)response.StatusCode, errorResponse);
        }

     
        public async Task<IActionResult> AddUser(string RoomID,string userID) // tạo phòng chat
        {
           
            var requestAddUser = new RequestAddUser{

                userID = userID,
                RoomID = RoomID


            };
            var tokenResponse = await _TokenService.GetToken("CoffeeAPI.read");
            _httpClient.SetBearerToken(tokenResponse.AccessToken);
            var apiEndpoint = _config["apiUrl"] + "/api/ChatRoom/AddUser";
            var request = new HttpRequestMessage(HttpMethod.Post, apiEndpoint);

            request.Content = new StringContent(JsonConvert.SerializeObject(requestAddUser), Encoding.UTF8, "application/json");


            var response = await _httpClient.SendAsync(request);

            //
            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();
                return Json(responseData);
            }
            var errorResponse = await response.Content.ReadAsStringAsync();
            return StatusCode((int)response.StatusCode, errorResponse);
        }




        public async Task<IActionResult> DeleteUserRoom(RequestAddUser requestAddUser) // tạo phòng chat
        {

            var tokenResponse = await _TokenService.GetToken("CoffeeAPI.read");
            _httpClient.SetBearerToken(tokenResponse.AccessToken);
            var apiEndpoint = _config["apiUrl"] + "/api/Messages";
            var request = new HttpRequestMessage(HttpMethod.Delete, apiEndpoint);

            request.Content = new StringContent(JsonConvert.SerializeObject(requestAddUser), Encoding.UTF8, "application/json");


            var response = await _httpClient.SendAsync(request);

            //
            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();
                return Json(responseData);
            }
            var errorResponse = await response.Content.ReadAsStringAsync();
            return StatusCode((int)response.StatusCode, errorResponse);
        }


        public async Task<IActionResult> CreateMessageRoom(MessageViewModel messageViewModel)
        {

            var user = _httpContextAccessor.HttpContext.User;
            var userIdClaim = user.FindFirst("sub")?.Value;


            messageViewModel.UserID = userIdClaim;



            try
            {
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
            catch (Exception ex)
            {
                // Xử lý lỗi xảy ra trong quá trình gọi API
                return StatusCode(500, ex.Message);
            }
        }

    }
}
