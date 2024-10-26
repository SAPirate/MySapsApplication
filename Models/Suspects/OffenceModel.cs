using System.ComponentModel.DataAnnotations;

namespace MySapsApplication.Models.Suspects
{
    public class OffenceModel
    {
        [Key]
        public string? OffenceType { get; set; }
    }
}
