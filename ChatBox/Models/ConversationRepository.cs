using Dapper;
using System.Data;

namespace ChatBox.Models
{
    public class ConversationRepository : Repository, IConversationRepository
    {
        public ConversationRepository(IDbConnection connection) : base(connection)
        {
        }

        public string GetMembersIdInGroup(string userid, string convid)
        {
            return connection.QueryFirstOrDefault<string>("GetUserIdinGroup", new { UserId = userid, ConvId = convid }, commandType: CommandType.StoredProcedure);
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
    }
}
