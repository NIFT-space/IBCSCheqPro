using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IBCS_Web_Portal.Models
{
    public class Domain
    {
        public int domainID { get; set; }
        public string bankCode { get; set; }
        public string emailDomain { get; set; }
    }

    public class DomainResponse
    {
        public string responseCode { get; set; }
        public string responseMessage { get; set; }
        public List<Domain> domains { get; set; }
    }

    public class DomainRequest
    {
        public string bankCode { get; set; }
    }
}