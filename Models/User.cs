namespace NIBC.Models
{
    public class User_tick
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Complainee_Bankcode { get; set; }
        public string Complainee_Branchcode { get; set; }
        public string Mobnumber { get; set; }
        public string DDL_Cycle { get; set; }
        public string DDL_ClearingDate { get; set; }
        public string City { get; set; }
        public string BankCode { get; set; }

        public string BranchCode { get; set; }

        public string Date { get; set; }

        public string Chequenumber { get; set; }

        public string Amount { get; set; }
        public string Complaint { get; set; }
        public string CompDetails { get; set; }
        public DateTime date1 { get; set; }
        public DateTime date2 { get; set; }
        
    }

    public class UserBank
    {
        public string instid { get; set; }
        public string instname { get; set; }
    }

    public class UserBranch
    {
        public string BrID { get; set; }
        public string Branchname { get; set; }
    }

    public class NewTicket
    {
       public string Name { get; set; }
       public string? Mobnumber { get; set; }
       public string? Email { get; set; }
       public string Complainee_Bankcode { get; set; }
       public string Complainee_Branchcode { get; set; }
       public string Complaint { get; set; }
       public string DDL_Cycle { get; set; }
        public string City { get; set; }
        public string CompDetails { get; set; }
        public DateTime date1 { get; set; }
        public string? Date { get; set; }
        public string BankCode { get; set; }
        public string BranchCode { get; set; }
        public string? Chequenumber { get; set; }
        public string? Amount { get; set; }
        public DateTime date2 { get; set; }
    }

    
}
