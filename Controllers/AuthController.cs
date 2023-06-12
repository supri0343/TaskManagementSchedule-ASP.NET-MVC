using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestAplication.Data;
using TestAplication.Models;
using TestAplication.Util;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace TestAplication.Controllers
{
    [AllowAnonymous]
    public class AuthController : Controller
    {
        private readonly ScheduleContext context;
        public AuthController(ScheduleContext _context)
        {
            context = _context;
        }
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Login(UserModel model)
        {
            using (context)
            {
                var IsValidUser = context.Users.Where(user => user.UserName.ToUpper() ==
                     model.UserName.ToUpper() && user.UserPassword.Equals(SHA1Encrypt.Hash(model.UserPassword))).FirstOrDefault();
                if (IsValidUser != null)
                {
                    var Role = context.Roles.Where(x => x.UserId == IsValidUser.ID).Select(x => x).First();

                    var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, model.UserName),
                            new Claim(ClaimTypes.Role, Role.RoleName.ToUpper()),
                    };

                    var claimsIdentity = new ClaimsIdentity(
                        claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var authProperties = new AuthenticationProperties
                    {
                     
                    };
                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),authProperties);

                    return RedirectToAction("Index", "Home");

                   
                }
                ModelState.AddModelError("", "invalid Username or Password");
                return View();
            }
        }
    }
}
