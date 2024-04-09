using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IBCS_Web_Portal.Models
{
    public class ICCycleTiming
    {
        public int CycleNo { get; set; }
        public string Cycle_desc { get; set; }
        public int Sorder { get; set; }
        public string CityName { get; set; }
        public string CycleStartTime { get; set; }
        public string CycleEndTime { get; set; }
        public int mindiff {  get; set; }
    }

    public class ICCycleTimingRequest
    {
        public string SBankCode { get; set; }
        public int IBankCode { get; set; }
    }

    public class ICCycleTimingResponse
    {
        public string responseCode { get; set; }
        public string responseMessage { get; set; }
        public List<ICCycleTiming> iCCycleTiming { get; set; }
    }

    public class ICWeeklyCycleTiming
    {
        public int Cycle_no { get; set; }
        public string Cycle_desc { get; set; }
        public int Sorder { get; set; }
        public string Monday { get; set; }
        public string Tuesday { get; set; }
        public string Wednesday { get; set; }
        public string Thursday { get; set; }
        public string Friday { get; set; }
        public string Saturday { get; set; }
        public string Sunday { get; set; }
    }

    public class ICWeeklyCycleTimingResponse
    {
        public string responseCode { get; set; }
        public string responseMessage { get; set; }
        public List<ICWeeklyCycleTiming> iCCycleTiming { get; set; }
    }
}