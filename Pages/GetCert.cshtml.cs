using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Cryptography.X509Certificates;

namespace IBCS_Core_Web_Portal.Pages
{
    public class GetCertModel : PageModel
    {
        public X509Certificate2 SelectedCertificate { get; set; }

        public void OnGet()
        {
        }

        public void OnPost()
        {
            X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadOnly);

            X509Certificate2Collection certificates = store.Certificates;
            X509Certificate2Collection selectedCertificates = X509Certificate2UI.SelectFromCollection(
                certificates,
                "Certificate Selection",
                "Select the certificate you want to use.",
                X509SelectionFlag.SingleSelection);

            if (selectedCertificates.Count > 0)
            {
                SelectedCertificate = selectedCertificates[0];
            }

            store.Close();
        }
    }
}
