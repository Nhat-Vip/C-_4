using Microsoft.AspNetCore.Mvc;

public class AdminController : Controller
{
    private readonly IAdminService _admin;
    private readonly MyDbContext _context;
    public AdminController(IAdminService admin, MyDbContext context)
    {
        _admin = admin;
        _context = context;
    }
    public IActionResult Index()
    {
        return View();
    }
}