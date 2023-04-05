using Dapper;
using System.Data;

namespace ChatBox.Models
{
    public class MessageRepository : Repository, IMessageRepository
    {
        public MessageRepository(IDbConnection connection) : base(connection)
        {
        }

        public int Add(Message obj)
        {
            //string sql = "INSERT INTO Message(ConvId, UserId, Content) VALUES (@ConvId, @UserId, @Content)";
            return connection.Execute("AddMessage", new { ConvId = obj.ConvId, UserId = obj.UserId, Content = obj.Content },commandType:CommandType.StoredProcedure);
        }

        public IEnumerable<Conversation> GetGroups(string userid)
        {
            return connection.Query<Conversation>("GetGroupName", new { UserId = userid}, commandType: CommandType.StoredProcedure);

        }

        public IEnumerable<Message> GetMessages(string convid)
        {
            return connection.Query<Message>("GetMessages", new { ConvId = convid }, commandType: CommandType.StoredProcedure);
        }
    }
}
