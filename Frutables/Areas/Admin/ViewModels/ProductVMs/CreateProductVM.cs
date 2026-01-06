using System.ComponentModel.DataAnnotations;

namespace Frutables.Areas.Admin.ViewModels.ProductVMs
{
    public class CreateProductVM
    {
        [Required]
        public string Name { get; set; }



        [Required]
        public decimal Price { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        public IFormFile MainImage { get; set; }


    }
}
