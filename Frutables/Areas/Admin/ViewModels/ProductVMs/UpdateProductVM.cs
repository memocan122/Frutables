using System.ComponentModel.DataAnnotations;

namespace Frutables.Areas.Admin.ViewModels.ProductVMs
{
    public class UpdateProductVM
    {
        


        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public int CategoryId { get; set; }

        public string? MainPhoto { get; set; }
        public IEnumerable<string>? AdditionalPhotos { get; set; }

        public IFormFile? MainImage { get; set; }
        public IEnumerable<IFormFile>? AdditionalImages { get; set; }
    }
}
