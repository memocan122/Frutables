namespace Frutables.ViewModel
{
    public class HomeVM
    {
        public IEnumerable<Models.Slider> Sliders { get; set; }
        public IEnumerable<Models.Product> Products { get; set; }
        public IEnumerable<Models.Category> Categories { get; set; }

    }
}
