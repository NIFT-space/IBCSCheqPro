namespace NIBC.Models
{
    public class Ticket
    {
        public string TicketNo { get; set; }
        public string status { get; set; }

        public string comment { get; set; }
    }

    public class ViewTicket
    {
        public string? Date { get; set; }
        public string? Name { get; set; }
        public string? Mobile { get; set; }
        public string? Email { get; set; }
        public string? cityofincident { get; set; }
        public string? sbankname { get; set; }
        public string? sbankcode { get; set; }
        public string? recieverbank { get; set; }
        public string? recieverbranch { get; set; }
        public string? chqno { get; set; }
        public string? accno { get; set; }
        public string? amt { get; set; }
        public string? dinno { get; set; }
        public string? trancode { get; set; }
        public string? processdate { get; set; }
        public string? subject { get; set; }
        public string? details { get; set; }
        public string? type { get; set; }
        public string? city { get; set; }
        public string? remail { get; set; }
        public string? status { get; set; }
        public string? RBank { get; set; }
        public string? RBranch { get; set; }

    }

    public class ViewTicketResponse
    {
        public List<ViewTicket> Ticket { get; set; }
        public string ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
    }
}
