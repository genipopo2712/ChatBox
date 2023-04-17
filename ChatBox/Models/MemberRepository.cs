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
            string sql = "SELECT Fullname, Avatar, LastTimeActive FROM Member WHERE UserId = @UserId";
            return connection.QueryFirstOrDefault<Member>(sql, new {UserId = id});
        }
        public DateTime GetLastTimeActive(string id)
        {
            string sql = "SELECT LastTimeActive FROM Member WHERE UserId = @UserId";
            return connection.QueryFirstOrDefault<DateTime>(sql, new {UserId = id});
        }

        //Note 01: Not use this function any more from 06/04/23 because this make heavy traffic of query to database 
        //Note 04: Re-use this function again for set last time active of user when you user click to sign out
        public int SetTimeActive(string id, DateTime time)
        {
            return connection.Execute("SetTimeActive", new { UserId = id, LastTimeActive = time }, commandType: CommandType.StoredProcedure);
        }

        public string CheckUserNameById(string name)
        {
            string sql = "SELECT Fullname FROM Member WHERE Username = @Username";
            return connection.QueryFirstOrDefault<string>(sql, new { Username = name });
        }

        public int Add(Member member)
        {
            return connection.Execute("AddMemberIfNotExists",member, commandType: CommandType.StoredProcedure); 
        }

        public IEnumerable<Member> GetMembersById(string id)
        {
            string sql = "SELECT Username, UserId, Fullname, Avatar, LastTimeActive FROM Member WHERE UserId <> @UserId ORDER BY Fullname ASC";
            return connection.Query<Member>(sql, new {UserId = id});
        }

        public int ChangePwd(string userid, string oldpwd, string newpwd)
        {
            return connection.Execute("ChangePassword", new { UserId = userid, Password = Helper.Hash(oldpwd), NewPassword = Helper.Hash(newpwd) }, commandType: CommandType.StoredProcedure);
        }

        public int ChangeAva(string userid, string ava)
        {
            return connection.Execute("ChangeAvatar", new { UserId = userid, Avatar = ava }, commandType: CommandType.StoredProcedure);
        }

        public int ChangeLastTimeActive(string userid, DateTime t)
        {
            return connection.Execute("ChangeTime", new { UserId = userid, LastTimeActive = t }, commandType: CommandType.StoredProcedure);
        }


        //Note 01: Not use this function any more from 06/04/23 because this make heavy traffic of query to database 
        /*
        public DateTime GetTimeActive(string id)
        {
            string sql = "SELECT LastTimeActive FROM Member WHERE UserId = @UserId";
            return connection.QueryFirstOrDefault<DateTime>(sql, new { UserId = id});
        }
        */


        //Note 01: Not use this function any more from 06/04/23 because this make heavy traffic of query to database 
        /*
        public UserStatus GetTimeAwayById(string id)
        {
            return connection.QueryFirstOrDefault<UserStatus>("CountTimeAway", new {UserId = id}, commandType: CommandType.StoredProcedure);
        }
        */
    }
}
