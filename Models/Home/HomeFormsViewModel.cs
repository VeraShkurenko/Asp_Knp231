using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace AspKnP231.Models.Home
{
    public class HomeFormsViewModel
    {
        public HomeFormsFormModel? FormModel { get; set; }

        public ModelStateDictionary? FormModelState { get; set; }
    }
}
