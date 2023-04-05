namespace ChatBox.Models
{
    public interface IConversationRepository
    {
        string GetMembersInGroup(string userid, string convid);
        string GetMembersIdInGroup(string userid, string convid);
        string GetNameById(string id);
    }
}
