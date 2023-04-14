namespace ChatBox.Models
{
    public interface IMessageRepository
    {
        IEnumerable<ConversationInfo> GetGroups(string userid);
        IEnumerable<Message> GetMessages(string convid);
        int Add(Message obj);
        int CountUnreadMessage(string userid, string convid);
        int ReadMessage(string convid, string userid);
    }
}
