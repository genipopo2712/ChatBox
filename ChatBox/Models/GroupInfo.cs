namespace ChatBox.Models
{
    public class GroupInfo
    {
        public string Convname { get; set; }
        public string ConvId { get; set; }
        public string Avatar { get; set; }
        public DateTime MessageDate { get; set; }
        public int CountMessage { get; set; }
        public string Content { get; set; }
        public DateTime? LastTimeActive { get; set; }
        public int CountMember { get; set; }
    }
}
