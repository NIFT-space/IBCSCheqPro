using System.Security.Cryptography.X509Certificates;

namespace IBCS_Core_Web_Portal
{
    public class CertificateValidationService
    {
        public bool ValidateCertifcate(X509Certificate2 clientCertificate)
        {
            var cert = new X509Certificate2(Path.Combine("dev_certt.pfx"), "1234");
            if (clientCertificate.Thumbprint == cert.Thumbprint)
            {
                return true;
            }
            return false;
        }
    }
}
