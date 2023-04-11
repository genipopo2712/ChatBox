namespace ChatBox.Models
{
    public class Message
    {
        public int MessageId { get; set; }
        public string UserId { get; set; }
        public string ConvId { get; set; }
        public string Content { get; set; }
        public DateTime MessageDate { get; set; }
        public string Avatar { get; set; }
        public string Fullname { get; set; }
        public bool IsRead { get; set; }
    }
}
