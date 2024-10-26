using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MySapsApplication.Models.ViewModel
{
    public class EditUserViewModel
    {
        public IdentityUser User { get; set; }
    }
}
