
namespace AspKnP231.Services.Storage
{
    public class LocalStorageService : IStorageService
    {
        private readonly String _path;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LocalStorageService(IHttpContextAccessor httpContextAccessor)
        {
            _path = Path.Combine(Directory.GetCurrentDirectory(), "LocalStorage/");
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetPathPrefix()
        {
            var request = _httpContextAccessor.HttpContext!.Request;
            return $"{request.Scheme}://{request.Host}/Storage/Item/";
        }

        public byte[] Load(string filename)
        {
            String savedName = Path.Combine(_path, filename);
            if (File.Exists(savedName))
            {
                return File.ReadAllBytes(savedName);
            }
            throw new FileNotFoundException(savedName);
        }

        public string Save(IFormFile formFile)
        {
            String ext = Path.GetExtension(formFile.FileName);
            String savedName = Guid.NewGuid().ToString() + ext;
            using Stream stream = File.OpenWrite(Path.Combine(_path, savedName));
            formFile.CopyTo(stream);
            return savedName;
        }
    }
}
