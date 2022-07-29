using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace PuppetMaster.WebApi.Certificates
{
    public static class KeyHelper
    {
        public static byte[] GenerateEncryptionCertificate(string subject, string password)
        {
            using var algorithm = RSA.Create(keySizeInBits: 2048);

            var request = new CertificateRequest(subject, algorithm, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            request.CertificateExtensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.KeyEncipherment, critical: true));

            var certificate = request.CreateSelfSigned(DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddYears(2));

            // Note: setting the friendly name is not supported on Unix machines (including Linux and macOS). 
            // To ensure an exception is not thrown by the property setter, an OS runtime check is used here. 
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                certificate.FriendlyName = "PuppetMaster Encryption Certificate";
            }

            return certificate.Export(X509ContentType.Pfx, password);
        }

        public static byte[] GenerateSigningCertificate(string subject, string password)
        {
            using var algorithm = RSA.Create(keySizeInBits: 2048);

            var request = new CertificateRequest(subject, algorithm, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            request.CertificateExtensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.DigitalSignature, critical: true));

            var certificate = request.CreateSelfSigned(DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddYears(2));

            // Note: setting the friendly name is not supported on Unix machines (including Linux and macOS). 
            // To ensure an exception is not thrown by the property setter, an OS runtime check is used here. 
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                certificate.FriendlyName = "PuppetMaster Signing Certificate";
            }

            return certificate.Export(X509ContentType.Pfx, password);
        }
    }
}
