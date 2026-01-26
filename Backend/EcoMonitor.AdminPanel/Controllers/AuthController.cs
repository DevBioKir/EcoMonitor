// using EcoMonitor.Contracts.Contracts.Auth;
// using Microsoft.AspNetCore.Mvc;
//
// namespace EcoMonitor.AdminPanel.Controllers;
//
// [Route("auth")]
// public class AuthController : Controller
// {
//     private readonly IHttpClientFactory _clientFactory;
//     private HttpClient Client => _clientFactory.CreateClient("PublicApi");
//
//     public AuthController(IHttpClientFactory clientFactory)
//     {
//         _clientFactory = clientFactory;
//     }
//
//     [HttpGet("login")]
//     public IActionResult Login(string? returnUrl = null)
//     {
//         ViewData["ReturnUrl"] = returnUrl ?? "/admin/dashboard";
//         return View();
//     }
//
//     [HttpPost("mvc-login")]
//     public async Task<IActionResult> LoginAsync(
//         [FromForm]AuthRequest request,
//         // string email,
//         // string password,
//         string? returnUrl = null)
//     {
//         returnUrl ??= "/admin/dashboard";
//
//         //var client = _clientFactory.CreateClient("PublicApi");
//
//         var response = await Client.PostAsJsonAsync(
//             "api/admin/v1/AdminAuthorization/admin-login", 
//             request);
//
//         if (!response.IsSuccessStatusCode)
//         {
//             ModelState.AddModelError(string.Empty, "Неверный логин или пароль");
//             ViewData["ReturnUrl"] = returnUrl;
//             return View();
//         }
//
//         var loginResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
//         
//         var accessToken = loginResponse?.AccessToken;
//         var refreshToken = loginResponse?.RefreshToken;
//         
//
//         if (string.IsNullOrWhiteSpace(accessToken) || string.IsNullOrWhiteSpace(refreshToken))
//         {
//             ModelState.AddModelError(string.Empty, "Пустой токен от сервера");
//             ViewData["ReturnUrl"] = returnUrl;
//             return View();
//         }
//         
//         var cookieOptions = new CookieOptions
//         {
//             HttpOnly = true,
//             Secure = false, // prod: true
//             SameSite = SameSiteMode.Lax,
//             Path = "/"
//         };
//         
//         Response.Cookies.Append(
//             "accessToken",
//             accessToken,
//             cookieOptions);
//
//         Response.Cookies.Append(
//             "refreshToken",
//             refreshToken,
//             cookieOptions);
//         
//         return Redirect(returnUrl ?? "/admin/dashboard");
//     }
//
//     [HttpPost("logout")]
//     public async Task<IActionResult> LogoutAsync()
//     {
//         var refreshToken = Request.Cookies["refreshToken"];
//         var logoutRequest = new RefreshTokenRequest(refreshToken);
//         
//         if (!string.IsNullOrWhiteSpace(refreshToken))
//         {
//             await Client.PostAsJsonAsync("api/admin/v1/AdminAuthorization/logout", logoutRequest);
//         }
//         
//         Response.Cookies.Delete("accessToken");
//         Response.Cookies.Delete("refreshToken");
//         
//         return Redirect("/auth/login");
//     }
// }