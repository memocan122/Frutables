using System.ComponentModel.DataAnnotations;

namespace EntityFrameworkProject.Areas.Admin.ViewModels.SliderVMs
{
    public class CreateSliderVM
    {
        [Required(ErrorMessage ="Bos ola bilmez!")]
        public IFormFile Photo { get; set; }
    }
}
