using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IBCS_Core_Web_Portal.Pages.Cycleinfo
{
    public class OuttestModel : PageModel
    {
        public class branchboarddetails
        {
            public string? bkcode { get; set; }
            public string? citycode { get; set; }
            public string? cycle { get; set; }
            public string? brcode { get; set; }
            public string? bkType { get; set; }
        }
        public JsonResult OnPostFetchFiles(string data1)
        {
            return new JsonResult(true);
        }
    }
}
