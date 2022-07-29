namespace PuppetMaster.WebApi.Helpers
{
    public static class Base64Helper
    {
        public static string? Encode(string? plainText)
        {
            if (string.IsNullOrEmpty(plainText))
            {
                return null;
            }

            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }
    }
}
