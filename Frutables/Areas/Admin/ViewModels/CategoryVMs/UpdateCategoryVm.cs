using System.ComponentModel.DataAnnotations;

namespace Frutables.Areas.Admin.ViewModels.CategoryVMs
{
    public class UpdateCategoryVm
    {
        [Required(ErrorMessage = "Bos ola bilmez!")]
        public string Name { get; set; }
    }
}
