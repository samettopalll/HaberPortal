using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol;
using HaberPortal.Models;
using HaberPortal.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace HaberPortal.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }


        public IActionResult UserListAjax()
        {
            var userModels = _context.Users.Select(x => new UserModel()
            {
                Id = x.Id,
                FullName = x.FullName,
                Email = x.Email,
                UserName = x.UserName,
                PhotoUrl = x.PhotoUrl,
                Role = x.Role,
                Password = x.Password,
            }).ToList();

            return Json(userModels);
        }


    }
}

