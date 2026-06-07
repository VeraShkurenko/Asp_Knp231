using AspKnP231.Data;
using AspKnP231.Data.Entities;
using AspKnP231.Middleware.Auth.Token;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static System.Net.Mime.MediaTypeNames;
using AspKnP231.Models.Api;
using AspKnP231.Services.Storage;

namespace AspKnP231.Controllers.Api
{

    [Route("api/product")]
    [ApiController]
    public class ProductController(DataContext dataContext, IStorageService storageService) : ControllerBase
    {
        private readonly DataContext _dataContext = dataContext;
        private readonly IStorageService _storageService = storageService;
        [HttpGet("{id}")]
        public RestResponse GetProduct(String id)
        {
            string authMessage;
            if (HttpContext.User.Identity?.IsAuthenticated ?? false)
            {
                authMessage = HttpContext.User.Claims.First(c => c.Type == ClaimTypes.Name).Value;
            }
            else
            {
                authMessage = HttpContext.Items[nameof(AuthTokenMiddleware)]?.ToString() ?? string.Empty;
            }
            var product = _dataContext.ShopProducts.FirstOrDefault(p => p.Slug == id || p.Id.ToString() == id);
            return new()
            {
                Meta = new()
                {
                    ServerTime = DateTime.Now.Ticks,
                    Cache = 3600,
                    ResourceId = id,
                    AuthStatus = authMessage,
                    DataType = product == null ? "null" : "object",
                    Path = Request.Path,
                    Service = "Asp-Shop API"
                },
                Data = product == null ? null : new ShopProductPage
                {
                    Product = new()
                    {
                        Id = product.Id.ToString(),
                        Title = product.Title,
                        Price = product.Price,
                        Stock = product.Stock,
                        Slug = product.Slug,
                        ImageUrl = _storageService.GetPathPrefix() + (product.ImageUrl ?? "no_image.jpg"),
                        Rating = null,
                        Discount = 0,
                    },
                    Recommended = [.._dataContext
    .ShopProducts
    .Where(p => p.Id != product.Id)
    .OrderBy(_ => Guid.NewGuid())
    .Take(3)
    .Select(p => new ShopProductModel{
        Id = p.Id.ToString(),
        Title = p.Title,
        Price = p.Price,
        Stock = p.Stock,
        Slug = p.Slug,
        ImageUrl = _storageService.GetPathPrefix() + (p.ImageUrl ?? "no_image.webp"),
        Rating = null,
        Discount = 0,
    })
]
                    // Три випадкові товари окрім даного
                },
            };
        }

    }
}
