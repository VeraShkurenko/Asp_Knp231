namespace AspKnP231.Services.Storage
{
    public static class StorageExtension
    {
        public static IServiceCollection AddStorage(this IServiceCollection services)
        {
            return services.AddScoped<IStorageService, LocalStorageService>();
        }
    }
}
