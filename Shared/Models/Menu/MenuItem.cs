namespace Shared.Models.Menu
{
    public class MenuItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsVegetarian { get; set; }
        public bool IsVegan { get; set; }
        public bool IsGlutenFree { get; set; }
        public string CategoryId { get; set; }
        public string CategoryName { get; set; }
        public decimal DiscountPercentage { get; set; }
        public bool IsFeatured { get; set; }
    }
}