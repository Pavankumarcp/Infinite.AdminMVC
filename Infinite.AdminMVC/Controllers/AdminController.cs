using Infinite.AdminMVC.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Threading.Tasks;

namespace Infinite.AdminMVC.Controllers
{
    public class AdminController : Controller
    {
        private readonly IConfiguration _configuration;
        public AdminController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(AdminViewModel admin)
        {
            if(ModelState.IsValid)
            {
                using(var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    client.BaseAddress = new System.Uri(_configuration["ApiUrl:api"]);
                    var result = await client.PostAsJsonAsync("Admin/Login", admin);
                    if (result.IsSuccessStatusCode)
                    {
                        string token = await result.Content.ReadAsAsync<string>();
                        HttpContext.Session.SetString("token", token);
                        return RedirectToAction("Privacy", "Home");
                        
                    }
                    ModelState.AddModelError("", "Invalid Username or Password");
                }
            }
            return View(admin);
        }

        [HttpPost]
        public IActionResult Logout ()
        {
            HttpContext.Session.Remove("token");
            return RedirectToAction("Index", "Home");
        }
    }
        
}