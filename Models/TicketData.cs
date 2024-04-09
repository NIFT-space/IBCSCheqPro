namespace NIBC.Models
{
    public class TicketData
    {

        public string status { get; set; }
        public string rstatus { get; set; }
        public string sstatus { get; set; }
        public DateTime updatedt { get; set; }
        public int ticketno { get; set; }
        public string roleofcomplaint { get; set; }
        public DateTime complaintdate { get; set; }
        public string subject { get; set; }
        public string type { get; set; }
        public string details { get; set; }
        public string recieverbank { get; set; }
        public string recieverbranch { get; set; }
        public string sbankname { get; set; }
        public string sbankcode { get; set; }
        public string responseCode { get; set; }
        public string responseMessage { get; set; }
    }

    public class CPUTicketData
    {
        public int CityId { get; set; }
        public string Cityname { get; set; }
        public string responseCode { get; set; }
        public string responseMessage { get; set; }

    }
}
