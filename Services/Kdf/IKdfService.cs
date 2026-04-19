namespace AspKnP231.Services.Kdf
{
    internal interface IKdfService
    {
        String Dk(String salt, String password);
    }
}
