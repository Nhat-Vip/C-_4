using Microsoft.AspNetCore.Mvc;

namespace ASM_C_4.Controllers
{
    public class RegisterController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(string username, string password, string confirmPassword)
        {
            if (password != confirmPassword)
            {
                ViewBag.Error = "Mật khẩu xác nhận không đúng.";
                return View();
            }

            // Giả sử đăng ký thành công (chưa lưu DB)
            ViewBag.Success = "Đăng ký thành công! Bạn có thể đăng nhập.";
            return RedirectToAction("Index", "Login");
        }
    }
}
