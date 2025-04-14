namespace Shared.Models.Order
{
    public class CartItem
    {
        public string MenuItemId { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public List<string> Customizations { get; set; } = new List<string>();
        public string SpecialInstructions { get; set; }
        public decimal Subtotal => Price * Quantity;
    }

    public class Cart
    {
        public List<CartItem> Items { get; set; } = new List<CartItem>();
        public string SpecialInstructions { get; set; }
        public OrderType OrderType { get; set; } = OrderType.DineIn;
        public string TableNumber { get; set; }
        public decimal TipAmount { get; set; }
        
        public decimal Subtotal => Items.Sum(i => i.Subtotal);
        public decimal Tax => Math.Round(Subtotal * 0.1m, 2); // Assuming 10% tax
        public decimal Total => Subtotal + Tax + TipAmount;
        
        public int ItemCount => Items.Sum(i => i.Quantity);
        
        public void AddItem(CartItem item)
        {
            var existingItem = Items.FirstOrDefault(i => 
                i.MenuItemId == item.MenuItemId && 
                string.Equals(i.SpecialInstructions, item.SpecialInstructions) &&
                Enumerable.SequenceEqual(i.Customizations.OrderBy(c => c), item.Customizations.OrderBy(c => c)));
            
            if (existingItem != null)
            {
                existingItem.Quantity += item.Quantity;
            }
            else
            {
                Items.Add(item);
            }
        }
        
        public void RemoveItem(string menuItemId)
        {
            var item = Items.FirstOrDefault(i => i.MenuItemId == menuItemId);
            if (item != null)
            {
                Items.Remove(item);
            }
        }
        
        public void UpdateItemQuantity(string menuItemId, int quantity)
        {
            var item = Items.FirstOrDefault(i => i.MenuItemId == menuItemId);
            if (item != null)
            {
                if (quantity <= 0)
                {
                    Items.Remove(item);
                }
                else
                {
                    item.Quantity = quantity;
                }
            }
        }
        
        public void Clear()
        {
            Items.Clear();
            SpecialInstructions = null;
            TipAmount = 0;
        }
    }
}
