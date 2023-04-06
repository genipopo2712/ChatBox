using ChatBox.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.JSInterop.Implementation;

namespace ChatBox.Controllers
{
    public class AuthController : Controller
    {
        IMemberRepository memberRepository;
        public AuthController(IMemberRepository memberRepository)
        {
            this.memberRepository = memberRepository;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Signin()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Signin(SigninMember obj)
        {
            if (ModelState.IsValid)
            {
                Member member = memberRepository.Signin(obj);
                if (member != null)
                {
                    List<Claim> claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, member.Username),
                        new Claim(ClaimTypes.Email, member.Email),
                        new Claim(ClaimTypes.NameIdentifier, member.UserId),
                        new Claim(ClaimTypes.GivenName, member.Fullname),
                        new Claim("Avatar", member.Avatar)
                    };
                    ClaimsIdentity identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    HttpContext.SignInAsync(new ClaimsPrincipal(identity), new AuthenticationProperties
                    {
                        IsPersistent = obj.Rem
                    });
                    return Redirect("/chat/index");
                }
            }
            return View(obj);
        }
        public async Task<IActionResult> Signout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("/");
        }



        //Note 01: Not use this function any more from 06/04/23 because this make heavy traffic of query to database 
        /*
        public IActionResult Settime()
        {
            DateTime act = DateTime.Now;
            string userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userid != null)
            {
                memberRepository.SetTimeActive(userid,act);
            }
            return Json(null);
        }
        */
    }
}
