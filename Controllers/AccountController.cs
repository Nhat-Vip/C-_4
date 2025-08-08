using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

public class AccountController : Controller
{
    private readonly IAccountService _account;
    private readonly MyDbContext _context;
    public AccountController(MyDbContext context, IAccountService account)
    {
        _account = account;
        _context = context;
    }
    public IActionResult Login()
    {
        return View();
    }
 
    [HttpPost]
    public async Task<IActionResult> Login(string email, string password)
    {
        var user = await _account.Login(email, password);
        if (user!=null)
        {
            await LoginUser(user.Email!, user.UserName!, user.Role.ToString(), user.UserId);
            // Đăng nhập thành công, chuyển hướng về Home
            return RedirectToAction("Index", "Home");
        }

        ViewBag.Error = "Sai tài khoản hoặc mật khẩu!";
        return View();
    }
    [HttpGet]
    public IActionResult SignUp()
    {
        return View(new User());
    }

    [HttpPost]
    public async Task<IActionResult> SignUp(User user)
    {
        user.Role = Role.User;
        var findUser = _context.Users.FirstOrDefault(us => us.Email == user.Email);
        if (findUser != null)
        {
            ViewBag.Error = "Email đã tồn tại vui lòng sử dụng email khác";
            return View(user);

        }
        var signUp = await _account.SignUp(user);
        if (!signUp)
        {
            ViewBag.Error = "Có lỗi xảy ra vui lòng thử lại";
            return View(user);
        }
        ViewBag.Success = "Đăng ký thành công!";
        await LoginUser(user.Email!, user.UserName!, user.Role.ToString(), user.UserId);
        return RedirectToAction("Index", "Home");
    }

    private async Task LoginUser(string email, string userName, string role, int userId)
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
    public async Task<IActionResult> LogOut()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }
}
