using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

public class AccountController : Controller
{
    private readonly MyDbContext _context;
    public AccountController(MyDbContext _context)
    {
        
    }
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Login(string email, string password)
    {

        if (email == "admin" && password == "123")
        {
            // Đăng nhập thành công, chuyển hướng về Home
            return RedirectToAction("Index", "Home");
        }

        ViewBag.Error = "Sai tài khoản hoặc mật khẩu!";
        return View();
    }
    public IActionResult SignUp()
    {
        return View();
    }

    [HttpPost]
    public IActionResult SignUp(string username, string password, string confirmPassword)
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

    private async Task SignInUser(string email, string userName, string role, int userId)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Email,email),
            new Claim(ClaimTypes.Name,userName),
            new Claim(ClaimTypes.Role, role),
            new Claim(ClaimTypes.NameIdentifier,userId.ToString())
        };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
        new AuthenticationProperties { IsPersistent = true });

    }
    private async Task<IActionResult> LogOut()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }
}
