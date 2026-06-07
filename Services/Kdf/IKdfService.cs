namespace AspKnP231.Services.Kdf
{
    public interface IKdfService
    {
        String Dk(String salt, String password);
    }
}
