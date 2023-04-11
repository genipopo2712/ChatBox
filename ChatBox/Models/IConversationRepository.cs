namespace ChatBox.Models
{
    public interface IConversationRepository
    {
        string GetMembersInGroup(string userid, string convid);
        string GetMembersIdInGroup(string userid, string convid);
        string GetNameById(string id);
        int Add(string id, string name, string descrip);
        int Insert(string conv, string id);
    }
}
