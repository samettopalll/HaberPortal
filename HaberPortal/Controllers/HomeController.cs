using AspNetCoreHero.ToastNotification.Abstractions;
using HaberPortal.Models;
using HaberPortal.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using NETCore.Encrypt.Extensions;
using System.Diagnostics;
using System.Security.Claims;


namespace HaberPortal.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;
        private readonly INotyfService _notify;
        private readonly IConfiguration _config;
        private readonly IFileProvider _fileProvider;
        public HomeController(ILogger<HomeController> logger, AppDbContext context, INotyfService notify, IConfiguration config, IFileProvider fileProvider)
        {
            _logger = logger;
            _context = context;
            _notify = notify;
            _config = config;
            _fileProvider = fileProvider;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginModel model)
        {
            var hashedpass = MD5Hash(model.Password);
            var user = _context.Users.Where(s => s.UserName == model.UserName && s.Password == hashedpass).SingleOrDefault();

            if (user == null)

            {
                _notify.Error("Kullanıcı Adı veya Parola Geçersizdir!");
                return View();
            }

            List<Claim> claims = new List<Claim>() {

                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role,user.Role),
                new Claim("UserName",user.UserName),
                new Claim("PhotoUrl",user.PhotoUrl),

                };

            ClaimsIdentity idetity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            ClaimsPrincipal principal = new ClaimsPrincipal(idetity);

            AuthenticationProperties properties = new AuthenticationProperties()
            {
                AllowRefresh = true,
                IsPersistent = model.KeepMe
            };
            HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, properties);

            return RedirectToAction("Scan", "News");

        }
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterModel model)
        {
            if (_context.Users.Count(s => s.UserName == model.UserName) > 0)
            {
                _notify.Error("Girilen Kullanıcı Adı Kayıtlıdır!");
                return View(model);
            }
            if (_context.Users.Count(s => s.Email == model.Email) > 0)
            {
                _notify.Error("Girilen E-Posta Adresi Kayıtlıdır!");
                return View(model);
            }

            var rootFolder = _fileProvider.GetDirectoryContents("wwwroot");
            var photoUrl = "-";
            if (model.PhotoFile.Length > 0 && model.PhotoFile != null)
            {
                var filename = Guid.NewGuid().ToString() + Path.GetExtension(model.PhotoFile.FileName);
                var photoPath = Path.Combine(rootFolder.First(x => x.Name == "Photos").PhysicalPath, filename);
                using var stream = new FileStream(photoPath, FileMode.Create);
                model.PhotoFile.CopyTo(stream);
                photoUrl = filename;

            }

            var hashedpass = MD5Hash(model.Password);
            var user = new User();
            user.FullName = model.FullName;
            user.UserName = model.UserName;
            user.Password = hashedpass;
            user.Email = model.Email;
            user.PhotoUrl = photoUrl;
            user.Role = "Uye";
            _context.Users.Add(user);
            _context.SaveChanges();

            _notify.Success("Üye Kaydı Yapılmıştır. Oturum Açınız");

            return RedirectToAction("Login");
        }


        public IActionResult Logout()
        {
            HttpContext.SignOutAsync();
            return RedirectToAction("Login");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }


        [Authorize]
        public IActionResult Privacy()
        {
            return View();
        }

        [Authorize]
        public IActionResult UserPage()
        {
            var id = new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var userModel = _context.Users.Where(x => x.Id == id).Select(x => new UserModel()
            {
                Id = x.Id,
                FullName = x.FullName,
                Email = x.Email,
                UserName = x.UserName,
                PhotoUrl = x.PhotoUrl,
                Role = x.Role

            }).SingleOrDefault();

            return View(userModel);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public string MD5Hash(string pass)
        {
            var salt = _config.GetValue<string>("AppSettings:MD5Salt");
            var password = pass + salt;
            var hashed = password.MD5();
            return hashed;
        }
    }
}