namespace ChatBox.Models
{
    public interface IMemberRepository
    {
        Member Signin(SigninMember obj);
        IEnumerable<Member> GetMemberName(string id);
        Member GetMemberById(string id);
        int SetTimeActive(string id, DateTime time);
        DateTime GetTimeActive(string id);
        UserStatus GetTimeAwayById(string id);

    }
}
