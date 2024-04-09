namespace IBCS_Core_Web_Portal.Models
{
    public class ViewRequest
    {
        public string? DateOpened { get; set; }
        public string? name { get; set; }
        public string? city { get; set; }
        public string? reqbkcode { get; set; }
        public string? reqbrcode { get; set; }
        public string? title { get; set; }
        public string? reqtype { get; set; }
        public string? status { get; set; }
        public string? details { get; set; }
        public string? Date2 { get; set; }
        public string? refcode { get; set; }
    }

    public class ViewRequestResponse
    {
        public List<ViewRequest> ViewRequests { get; set; }
        public string ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
    }

    public class Requestinfo
    {
        public int RequestNo { get; set; }
    }
}
