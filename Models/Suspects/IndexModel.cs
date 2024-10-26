using System.ComponentModel.DataAnnotations;
using System;
using System.Web;

namespace MySapsApplication.Models.Suspects
{
    public class IndexModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Enter Suspect key Number.")]
        public string? SuspectNo { get; set; }

        [Required(ErrorMessage = "Enter Suspect IdentityNumber.")]
        [Display(Name = "SuspectIdentity", Order = 2)]
        [RegularExpression(@"(((\d{2}((0[013578]|1[02])(0[1-9]|[12]\d|3[01])|(0[13456789]|1[012])(0[1-9]|[12]\d|30)|02(0[1-9]|1\d|2[0-8])))|([02468][048]|[13579][26])0229))(( |-)(\d{4})( |-)([01]8((( |-)\d{1})|\d{1}))|(\d{4}[01]8\d{1}))", ErrorMessage = "Invalid ID Number")]
        [StringLength(13)]
        public string? SuspectIdentity { get; set; }

        [Required(ErrorMessage = "FirstName is required.")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "LastName is required.")]
        public string? LastName { get; set; }
    }
}

