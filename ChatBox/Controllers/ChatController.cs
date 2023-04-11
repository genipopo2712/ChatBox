using ChatBox.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;

namespace ChatBox.Controllers
{
    [Authorize]
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
            string ids = conversationRepository.GetMembersIdInGroup(id, t);
            DateTime lT = memberRepository.GetLastTimeActive(ids);
            ViewBag.lastActive = (int)(DateTime.Now - lT).TotalMinutes;
            if(name == null)
            {
                name = conversationRepository.GetMembersInGroup(id, t).ToString();
                //Note 01: Not use this function any more from 06/04/23 because this make heavy traffic of query to database
                /*
                memberRepository.SetTimeActive(name, DateTime.Now);
                */
            }
            ViewBag.id = id;
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
