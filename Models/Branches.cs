namespace NIBC.Models
{
    public class Branches
    {
        public List<Brname> Branchname { get; set; }
        public string responseCode { get; set; }
        public string responseMessage { get; set; }
    }
    public class Brname
    {
        public string BrID { get; set; }
        public string Branchname { get; set; }
    }
    //public class DDL_SelectedBranch
    //{
    //    public string Branchname { get; set; }
    //}
}
