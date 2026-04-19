namespace AspKnP231.Services.Kdf
{
    public interface IKdfService
    {
        string Dk(string salt, string password);
    }
}
