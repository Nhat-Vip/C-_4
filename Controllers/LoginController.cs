using Microsoft.AspNetCore.Mvc;

namespace ASM_C_4.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(string username, string password)
        {
            if (username == "admin" && password == "123")
            {
                // Đăng nhập thành công, chuyển hướng về Home
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Sai tài khoản hoặc mật khẩu!";
            return View();
        }
    }
}
