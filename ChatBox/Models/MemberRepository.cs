using Dapper;
using System.Data;
using Chatbox;

namespace ChatBox.Models
{
    public class MemberRepository : Repository, IMemberRepository
    {
        
        public MemberRepository(IDbConnection connection) : base(connection)
        {
        }
        public Member Signin(SigninMember obj)
        {
            string sql = "SELECT UserId, Username, Email, Fullname, Avatar FROM Member WHERE Password = @Pwd AND (Username = @Usr OR Email = @Eml)";
            return connection.QueryFirstOrDefault<Member>(sql, new {Eml=obj.Usr, Usr = obj.Usr, Pwd = Helper.Hash(obj.Pwd) });
            //return connection.QueryFirstOrDefault<Member>("SignIn", new { Pwd = Helper.Hash(obj.Pwd), Usr = obj.Usr }, commandType: CommandType.StoredProcedure);
        }
        public IEnumerable<Member> GetMemberName(string id)
        {
            string sql = "SELECT UserId, Username, Email, Fullname, Avatar FROM Member WHERE UserId <> @Id ";
            return connection.Query<Member>(sql, new {Id = id});
        }

        public Member GetMemberById(string id)
        {
            string sql = "SELECT Fullname, Avatar FROM Member WHERE UserId = @UserId";
            return connection.QueryFirstOrDefault<Member>(sql, new {UserId = id});
        }

        public int SetTimeActive(string id, DateTime time)
        {
            return connection.Execute("SetTimeActive", new { UserId = id, LastTimeActive = time }, commandType: CommandType.StoredProcedure);
        }
        public DateTime GetTimeActive(string id)
        {
            string sql = "SELECT LastTimeActive FROM Member WHERE UserId = @UserId";
            return connection.QueryFirstOrDefault<DateTime>(sql, new { UserId = id});
        }

        public UserStatus GetTimeAwayById(string id)
        {
            return connection.QueryFirstOrDefault<UserStatus>("CountTimeAway", new {UserId = id}, commandType: CommandType.StoredProcedure);
        }
    }
}
