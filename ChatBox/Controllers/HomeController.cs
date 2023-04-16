using ChatBox.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;

namespace ChatBox.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        IMemberRepository memberRepository;
        public HomeController(IMemberRepository memberRepository)
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
                    return Redirect("/success");
                }
            }
            return View(obj);
        }
        [HttpPost]
        public IActionResult Changepassword(string o, string n)
        {
            string u = User.FindFirstValue(ClaimTypes.NameIdentifier);
            string v = User.FindFirstValue(ClaimTypes.Name);
            o=v + "#$%!@Diep3007}{><" + o + "30071994!@#$%^&*()";
            n=v + "#$%!@Diep3007}{><" + n + "30071994!@#$%^&*()";
            int ret = memberRepository.ChangePwd(u, o, n);
            if (ret > 0)
            {
                return Json(ret);
            }
            return Json(null);
        }
    }
}
