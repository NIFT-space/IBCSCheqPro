namespace NIBC.Models
{
    public class Comment
    {
        public string Name { get; set; }
        public string CommentDetails { get; set; }
        public string Bankname { get; set; }
        public string Ticket { get; set; }
        public DateTime Dated { get; set; }
    }

    public class StatusComment
    {
        public string Ticket { get; set; }
        public string Bankcode { get; set; }
        public DateTime Dated { get; set; }

    }
    public class ViewComment
    {
        public string Name { get; set; }
        public string Bankname { get; set; }
        public string CommentDetails { get; set; }
        public DateTime Dated { get; set; }
    }
}
