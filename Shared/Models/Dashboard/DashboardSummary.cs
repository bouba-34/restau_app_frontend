namespace Shared.Models.Dashboard
{
    public class SalesSummary
    {
        public decimal TotalSales { get; set; }
        public int TotalOrders { get; set; }
        public decimal AverageOrderValue { get; set; }
    }
    
    public class MenuItemSales
    {
        public string MenuItemId { get; set; }
        public string MenuItemName { get; set; }
        public int QuantitySold { get; set; }
        public decimal TotalSales { get; set; }
    }
    
    public class DashboardSummary
    {
        public SalesSummary Today { get; set; }
        public SalesSummary Yesterday { get; set; }
        public SalesSummary ThisWeek { get; set; }
        public SalesSummary LastWeek { get; set; }
        public SalesSummary ThisMonth { get; set; }
        public int PendingOrders { get; set; }
        public List<MenuItemSales> TopSellingItems { get; set; }
        public Dictionary<string, decimal> SalesByCategory { get; set; }
        public Dictionary<int, int> OrdersByHour { get; set; }
    }
}