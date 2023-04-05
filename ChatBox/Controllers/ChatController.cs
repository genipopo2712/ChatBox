using ChatBox.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;

namespace ChatBox.Controllers
{
    public class ChatController : Controller
    {
        IMemberRepository memberRepository;
        IMessageRepository messageRepository;
        IConversationRepository conversationRepository;
        public ChatController(IMemberRepository memberRepository, IMessageRepository messageRepository, IConversationRepository conversationRepository)
        {
            this.memberRepository = memberRepository;
            this.messageRepository = messageRepository;
            this.conversationRepository = conversationRepository;
        }
        [ServiceFilter(typeof(ContactFilter))]
        public IActionResult Index()
        {
            return View();
        }
        [ServiceFilter(typeof(ContactFilter))]
        public IActionResult Chat(string t="")
        {
            string id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var name = conversationRepository.GetNameById(t);
            if(name == null)
            {
                name = conversationRepository.GetMembersInGroup(id, t).ToString();
                memberRepository.SetTimeActive(name, DateTime.Now);
            }
            ViewBag.conv = t;
            ViewBag.chatname = name;
            ViewBag.messages = messageRepository.GetMessages(t);
            return View();
        }
        [HttpPost]
        public IActionResult AddMessage(Message obj,string a="")
        {
            obj.ConvId = a;
            obj.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            messageRepository.Add(obj);
            return Redirect($"/Chat/Chat?t={a}");
        }
    }
}
