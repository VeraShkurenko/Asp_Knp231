namespace AspKnP231.Services.Hash
{
    public class ShaHashService : IHashService
    {
        public string Digest(string input)
        {
            return Convert.ToHexString(
                System.Security.Cryptography.SHA1.HashData(
                    System.Text.Encoding.UTF8.GetBytes(input)
                )
            );
        }
    }
}
