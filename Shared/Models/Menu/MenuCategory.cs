namespace Shared.Models.Menu
{
    public class MenuCategory
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int DisplayOrder { get; set; }
        public string ImageUrl { get; set; }
        public bool IsActive { get; set; }
    }
}