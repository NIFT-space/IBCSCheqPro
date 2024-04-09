using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IBCS_Web_Portal.Models
{
    public class Bank
    {
        public string InstId { get; set; }
        public int BankId { get; set; }
        public string InstName { get; set; }
    }
    public class BankRequest
    {
        public int InstId { get; set; }
        public string InstName { get; set; }
    }

    public class BankResponse
    {
        public string responseCode { get; set; }
        public string responseMessage { get; set; }
        public List<Bank> banks { get; set; }
    }
    public class InsertBankResponse
    {
        public string responseCode { get; set; }
        public string responseMessage { get; set; }
        public bool success { get; set; }
    }
}