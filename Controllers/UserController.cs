using Microsoft.AspNetCore.Mvc;

public class UserController : Controller
{
    private readonly IUserService _user;
    public UserController(IUserService user)
    {
        _user = user;
    }
    public IActionResult Index()
    {
        return View();
    }
}