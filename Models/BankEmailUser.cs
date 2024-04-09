using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IBCS_Web_Portal.Models
{

    public class BankUserEmail
    {
        public int regID { get; set; }
        public int userID { get; set; }
        public string userPWD { get; set; }
        public string userName1 { get; set; }
        public string email1 { get; set; }
        public string domain1 { get; set; }
        public string mobile1 { get; set; }
        public string phone1 { get; set; }
        public string userName2 { get; set; }
        public string email2 { get; set; }
        public string domain2 { get; set; }
        public string mobile2 { get; set; }
        public string phone2 { get; set; }
        public string bankCode { get; set; }
        public string branchCode { get; set; }
        public int cityID { get; set; }
        public string updatedate { get; set; }
        public int isUpdate { get; set; }
        public string cityName { get; set; }
    }
    public class BankUserEmailRequest
    {
        public string bankCode { get; set; }
        public string branchCode { get; set; }

    }

    public class BankUserEmailResponse
    {
        public string responseCode { get; set; }
        public string responseMessage { get; set; }
        public List<BankUserEmail> bankUserDetail { get; set; }
    }

    public class RegBankUserEmailReqest
    {
        public string UserID { get; set; }
        public string UserPWD { get; set; }
        public string UserName1 { get; set; }
        public string Email1 { get; set; }
        public string Domain1 { get; set; }
        public string Mobile1 { get; set; }
        public string Phone1 { get; set; }
        public string UserName2 { get; set; }
        public string Email2 { get; set; }
        public string Domain2 { get; set; }
        public string Mobile2 { get; set; }
        public string Phone2 { get; set; }
        public string BankCode { get; set; }
        public string BranchCode { get; set; }
        public string CityID { get; set; }
        public string Updatedate { get; set; }
        public string IsUpdate { get; set; }
    }

    public class RegBankUserEmailResponse
    {
        public string responseCode { get; set; }
        public string responseMessage { get; set; }
    }

    public class UpdBankUserEmailReqest
    {
        public string UserName1 { get; set; }
        public string Email1 { get; set; }
        public string Domain1 { get; set; }
        public string Mobile1 { get; set; }
        public string UserName2 { get; set; }
        public string Email2 { get; set; }
        public string Domain2 { get; set; }
        public string Mobile2 { get; set; }
        public int Regid { get; set; }
    }
}