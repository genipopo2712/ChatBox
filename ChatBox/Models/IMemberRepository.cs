namespace ChatBox.Models
{
    public interface IMemberRepository
    {
        Member Signin(SigninMember obj);
        IEnumerable<Member> GetMemberName(string id);
        Member GetMemberById(string id);
        DateTime GetLastTimeActive(string id);
        string CheckUserNameById(string name);
        int Add(Member member);
        IEnumerable<Member> GetMembersById(string id);  
        int ChangePwd(string userid, string oldpwd, string newpwd);

        //Note 01: Not use this function any more from 06/04/23 because this make heavy traffic of query to database 
        /*
        DateTime GetTimeActive(string id);
        */

        //Note 01: Not use this function any more from 06/04/23 because this make heavy traffic of query to database 
        /*
        UserStatus GetTimeAwayById(string id);
        */

        //Note 01: Not use this function any more from 06/04/23 because this make heavy traffic of query to database 
        //Note 04: Re-use this function again for set last time active of user when you user click to sign out
        int SetTimeActive(string id, DateTime time);
        
    }
}
