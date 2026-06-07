using AspKnP231.Services.Hash;

namespace AspKnP231.Services.Kdf
{
    internal class PbKdfService(IHashService hashService) : IKdfService
    {
        private readonly IHashService _hashService = hashService;

        public string Dk(string salt, string password)
        {
            String t = _hashService.Digest(salt + password);
            for (int i = 0; i < 100000; i++)
            {
                t = _hashService.Digest(t);
            }
            return t;
        }

    }
}
