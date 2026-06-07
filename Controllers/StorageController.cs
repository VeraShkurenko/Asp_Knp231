using AspKnP231.Data;
using AspKnP231.Services.Storage;
using Microsoft.AspNetCore.Mvc;

namespace AspKnP231.Controllers
{
    public class StorageController(IStorageService storageService) : Controller
    {
        private readonly IStorageService _storageService = storageService;

        [HttpGet]
        public IActionResult Item([FromRoute] String id)
        {
            String ext = Path.GetExtension(id);
            String mimeType = ext switch
            {
                ".png" => "image/png",
                ".jpg" => "image/jpeg",
                ".webp" => "image/webp",
                _ => "application/octet-stream"
            };
            try
            {
                return File(_storageService.Load(id), mimeType);
            }
            catch
            {
                return NotFound();
            }
        }
    }
}
