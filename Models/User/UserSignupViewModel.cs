using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace AspKnP231.Models.User
{
    public class UserSignupViewModel
    {
        public UserSignupFormModel? FormModel { get; set; }

        public ModelStateDictionary? FormModelState { get; set; }

        public bool IsSignupSuccessfull { get; set; }
    }
}
