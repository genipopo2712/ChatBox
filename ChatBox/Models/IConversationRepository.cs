namespace ChatBox.Models
{
    public interface IConversationRepository
    {
        string GetMembersInGroup(string userid, string convid);
        IEnumerable<string> GetMembersIdInGroup(string userid, string convid);
        string GetNameById(string id);
        int Add(Conversation obj);
        int Insert(string conv, string id);
        int CountNewMessage(string userid, string convid);

    }
}
