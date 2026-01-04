namespace Frutables.Models
{
    public class Product: BaseEntity
    {
        public string Name { get; set; }
        public double Price { get; set; }
        public int? CategoryId { get; set; }
        public Category? Category { get; set; }
        public ICollection<ProductImage> ProductImages { get; set; }
    }
}

