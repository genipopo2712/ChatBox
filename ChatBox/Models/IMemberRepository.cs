namespace ChatBox.Models
{
    public interface IMemberRepository
    {
        Member Signin(SigninMember obj);
        IEnumerable<Member> GetMemberName(string id);
        Member GetMemberById(string id);

        //Note 01: Not use this function any more from 06/04/23 because this make heavy traffic of query to database 
        /*
        DateTime GetTimeActive(string id);
        */

        //Note 01: Not use this function any more from 06/04/23 because this make heavy traffic of query to database 
        /*
        UserStatus GetTimeAwayById(string id);
        */

        //Note 01: Not use this function any more from 06/04/23 because this make heavy traffic of query to database 
        /*
        int SetTimeActive(string id, DateTime time);
        */
    }
}
