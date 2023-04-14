using ChatBox.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.JSInterop.Implementation;
using Chatbox;

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
                string pwd = obj.Usr + "#$%!@Diep3007}{><" + obj.Pwd + "30071994!@#$%^&*()";
                obj.Pwd = pwd;
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
                    DateTime act = DateTime.Now;
                    string id = member.UserId;
                    memberRepository.SetTimeActive(id, act);
                    return Redirect("/chat/index");
                }
            }
            return View(obj);
        }
        public async Task<IActionResult> Signout()
        {
            DateTime act = DateTime.Now;
            string id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            memberRepository.SetTimeActive(id, act);
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("/");
        }
        public IActionResult Signup()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Signup(Member obj)
        {
            obj.UserId = Helper.RandomString(32).ToString().ToUpper();
            obj.Avatar = "no-image.jpg";
            if (ModelState.IsValid)
            {
                if (obj != null)
                {

                    string pwd = obj.Username + "#$%!@Diep3007}{><" + obj.Password+ "30071994!@#$%^&*()";
                    obj.Password = pwd;
                    string info = memberRepository.CheckUserNameById(obj.Username);
                    if (info != null)
                    {
                        ViewBag.userMessage = "User name already existed";
                    }
                    else
                    {
                        int ret = memberRepository.Add(obj);
                    }
                }
            }
            return View(obj);

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
