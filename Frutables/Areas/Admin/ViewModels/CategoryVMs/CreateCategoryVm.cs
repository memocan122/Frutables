using System.ComponentModel.DataAnnotations;

namespace Frutables.Areas.Admin.ViewModels.CategoryVMs
{
    public class CreateCategoryVm
    {
        [Required(ErrorMessage = "Bos Olabilmez")]
        public string Name { get; set; }
    }
}
