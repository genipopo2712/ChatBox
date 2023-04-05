using ChatBox.Models;
using Microsoft.AspNetCore.Mvc;

namespace ChatBox.Controllers
{
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
    }
}
