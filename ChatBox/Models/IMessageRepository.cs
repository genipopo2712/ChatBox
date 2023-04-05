namespace ChatBox.Models
{
    public interface IMessageRepository
    {
        IEnumerable<Conversation> GetGroups(string userid);
        IEnumerable<Message> GetMessages(string convid);
        int Add(Message obj);

    }
}
