using ChatBox.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace ChatBox
{
    public class ContactFilter : IActionFilter
    {
        IMemberRepository memberRepository;
        IMessageRepository messageRepository;
        IConversationRepository conversationMemberRepository;
        
        public ContactFilter(IMemberRepository memberRepository, IMessageRepository messageRepository, IConversationRepository conversationMemberRepository)
        {
            this.memberRepository = memberRepository;
            this.messageRepository = messageRepository;
            this.conversationMemberRepository = conversationMemberRepository;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Controller is Controller con)
            {                
                string id = con.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (id != null)
                {
                    IEnumerable<Conversation> list = messageRepository.GetGroups(id);
                    foreach (var item in list)
                    {
                        if (string.IsNullOrEmpty(item.Convname))
                        {
                            item.Convname = conversationMemberRepository.GetMembersInGroup(id, item.ConvId);
                        }
                    }
                    con.ViewData["contacts"]=list;
                    IEnumerable<Member> users = memberRepository.GetMembersById(id);
                    con.ViewData["users"] = users;
                }
                
            }
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            
        }
    }
}
