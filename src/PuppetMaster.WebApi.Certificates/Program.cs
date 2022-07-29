namespace PuppetMaster.WebApi.Certificates
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var path = Path.GetDirectoryName(typeof(Program).Assembly.Location);
            var subject = "CN=puppetmaster";
            var password = "<your password>";
            File.WriteAllBytes(Path.Combine(path!, "Encryption.pfx"), KeyHelper.GenerateEncryptionCertificate(subject, password));
            File.WriteAllBytes(Path.Combine(path!, "Signing.pfx"), KeyHelper.GenerateSigningCertificate(subject, password));
        }
    }
}