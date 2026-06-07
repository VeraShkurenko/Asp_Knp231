using AspKnP231.Data.Entities;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace AspKnP231.Models.Shop.Admin
{
    public class ShopAdminViewModel
    {
        public ShopSectionFormModel? ShopSectionFormModel { get; set; }
        public ModelStateDictionary? ShopSectionModelState { get; set; }


        public ShopProductFormModel? ShopProductFormModel { get; set; }
        public ModelStateDictionary? ShopProductModelState { get; set; }

        public List<ShopSection> ShopSections { get; set; } = [];
    }
}
