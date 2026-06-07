namespace AspKnP231.Services.Storage
{
    // Сервіс збереження та доступу до файлів, завантажених формами
    public interface IStorageService
    {
        public String Save(IFormFile formFile);

        public byte[] Load(String filename);

        public String GetPathPrefix();
    }
}
