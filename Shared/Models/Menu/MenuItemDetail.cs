namespace Shared.Models.Menu
{
    public class MenuItemDetail : MenuItem
    {
        public int PreparationTimeMinutes { get; set; }
        public List<string> Ingredients { get; set; } = new List<string>();
        public List<string> Allergens { get; set; } = new List<string>();
        public int Calories { get; set; }
        public int DisplayOrder { get; set; }
    }
}