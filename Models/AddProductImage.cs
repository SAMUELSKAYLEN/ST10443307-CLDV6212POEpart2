using System.ComponentModel.DataAnnotations;

namespace ABCRetail.Models
{
    public class AddProductImage
    {
        [Required]
        public string? Name { get; set; }

        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; }

        [Required]
        [Display(Name = "Product Image")]
        public IFormFile? ProductImage { get; set; }


    }
}
