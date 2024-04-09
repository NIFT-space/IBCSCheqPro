using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace IBCS_Web_Portal.Models
{
    public class Branch
    {
        public int branchID { get; set; }
        public string branchName { get; set; }
    }

    public class BranchResponse
    {
        public string responseCode { get; set; }
        public string responseMessage { get; set; }
        public List<Branch> branches { get; set; }
    }

    public class BranchRequest
    {
        public string instID { get; set; }
        public string cityID { get; set; }
    }

}