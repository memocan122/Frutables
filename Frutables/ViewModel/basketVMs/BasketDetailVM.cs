namespace Frutables.ViewModel.basketVMs
{
    public class BasketDetailVM
    {
        public int Id { get; set; }
        public string Image { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public double Price { get; set; }
        public int Count { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
