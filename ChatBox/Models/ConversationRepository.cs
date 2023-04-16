using Dapper;
using System.Data;

namespace ChatBox.Models
{
    public class ConversationRepository : Repository, IConversationRepository
    {
        public ConversationRepository(IDbConnection connection) : base(connection)
        {
        }

        public int Add(Conversation obj)
        {
            return connection.Execute("CreateGroup", new { ConvId = obj.ConvId, Convname = obj.Convname, ConvDescrip = obj.ConvDescrip, Avatar = obj.Avatar },commandType: CommandType.StoredProcedure);
        }

        public int CountNewMessage(string userid, string convid)
        {
            return connection.QueryFirstOrDefault<int>("CountNewMessage", new { UserId = userid, ConvId = convid},commandType: CommandType.StoredProcedure);
        }

        public IEnumerable<string> GetMembersIdInGroup(string userid, string convid)
        {
            return connection.Query<string>("GetUserIdinGroup", new { UserId = userid, ConvId = convid }, commandType: CommandType.StoredProcedure);
        }

        public string GetMembersInGroup(string userid, string convid)
        {
            return connection.QueryFirstOrDefault<string>("GetUserinGroup", new { UserId = userid, ConvId = convid }, commandType: CommandType.StoredProcedure);
        }

        public string GetNameById(string id)
        {
            string sql = "SELECT Convname FROM Conversation WHERE ConvId = @ConvId";
            return connection.QueryFirstOrDefault<string>(sql, new { ConvId = id });
        }

        public int Insert(string conv, string id)
        {
            return connection.Execute("AddMemberToGroup", new { ConvId = conv, UserId = id }, commandType: CommandType.StoredProcedure);
        }
    }
}
