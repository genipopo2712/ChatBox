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

        public int CountUnreadMessage(string userid, string convid)
        {
            return connection.QueryFirstOrDefault<int>("CountMessage", new {UserId = userid, Convid = convid},commandType:CommandType.StoredProcedure);
        }

        public IEnumerable<ConversationInfo> GetDirects(string userid)
        {
            return connection.Query<ConversationInfo>("GetBoxMessageDirect", new { UserId = userid}, commandType: CommandType.StoredProcedure);

        }

        public IEnumerable<GroupInfo> GetGroups(string userid)
        {
            return connection.Query<GroupInfo>("GetBoxMessageGroup", new { UserId = userid }, commandType: CommandType.StoredProcedure);
        }

        public IEnumerable<Message> GetMessages(string convid)
        {
            return connection.Query<Message>("GetMessages", new { ConvId = convid }, commandType: CommandType.StoredProcedure);
        }

        public int ReadMessage(string convid, string userid)
        {
            return connection.Execute("ReadMessage", new {ConvId = convid, UserId = userid},commandType: CommandType.StoredProcedure);
        }
    }
}
