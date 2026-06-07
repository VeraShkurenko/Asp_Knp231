using AspKnP231.Data;
using AspKnP231.Models.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AspKnP231.Controllers.Api
{
    [Route("api/sections")]
    [ApiController]
    public class SectionsController(DataContext dataContext) : ControllerBase
    {
        private readonly DataContext _dataContext = dataContext;

        private String StorageUrl => $"{Request.Scheme}://{Request.Host}/Storage/Item/";

        [HttpGet]
        public RestResponse AllSections()
        {
            var sections = _dataContext
                .ShopSections
                .AsNoTracking()
                .Where(s => s.DeletedAt == null)
                .AsEnumerable()
                .Select(s => s with { ImageUrl = StorageUrl + s.ImageUrl });

            return new RestResponse
            {
                Meta = new RestMeta
                {
                    ServerTime = DateTime.Now.Ticks,
                    Cache = 3600,
                    DataType = "array",
                    Path = Request.Path,
                    Service = "Asp-Shop API"
                },
                Data = sections
            };
        }

        [HttpGet("{id}")]
        public RestResponse ProductsBySection(String id)
        {
            var section = _dataContext
                .ShopSections
                .Include(s => s.Products)
                .AsNoTracking()
                .FirstOrDefault(s => (s.DeletedAt == null && s.Id.ToString() == id) || s.Slug == id);

            if (section != null)
            {
                section = section with
                {
                    ImageUrl = StorageUrl + section.ImageUrl,
                    Products = [..section.Products.Select(p => p with
                    {
                        ImageUrl = p.ImageUrl == null ? null : StorageUrl + p.ImageUrl
                    })]
                };
            }

            return new RestResponse
            {
                Meta = new RestMeta
                {
                    ServerTime = DateTime.Now.Ticks,
                    Cache = 3600,
                    ResourceId = id,
                    DataType = section == null ? "null" : "object",
                    Path = Request.Path,
                    Service = "Asp-Shop API"
                },
                Data = section
            };
        }
    }
}