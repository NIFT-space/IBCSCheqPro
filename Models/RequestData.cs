namespace IBCS_Core_Web_Portal.Models
{
    public class RequestData
    {
        public string status { get; set; }
        public int? RequestNo { get; set; }
        public DateTime? DateOpened { get; set; }
        public DateTime? RequestDate { get; set; }
        public string? title { get; set; }
        public string? city { get; set; }
        public string? reqtype { get; set; }
        public string? reqbkcode { get; set; }
        public string? reqbrcode { get; set; }
        public string responseCode { get; set; }
        public string responseMessage { get; set; }

    }

    public class SearchRequestData
    {
        public string status { get; set; }
        public DateTime? DateOpened { get; set; }
        public int? RequestNo { get; set; }
        public string? RequateDate { get; set; }
        public string? title { get; set; }
        public string? reqtype { get; set; }
        public string? reqbkcode { get; set; }
        public string? reqbrcode { get; set; }
        public string responseCode { get; set; }
        public string responseMessage { get; set; }


    }
}
