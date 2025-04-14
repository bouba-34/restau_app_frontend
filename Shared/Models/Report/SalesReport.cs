using Shared.Models.Dashboard;
using Shared.Models.Order;

namespace Shared.Models.Report
{
    public class SalesReport
    {
        public DateTime Date { get; set; }
        public decimal TotalSales { get; set; }
        public int TotalOrders { get; set; }
        public decimal AverageOrderValue { get; set; }
        public Dictionary<string, decimal> SalesByCategory { get; set; } = new Dictionary<string, decimal>();
        public List<MenuItemSales> TopSellingItems { get; set; } = new List<MenuItemSales>();
        public Dictionary<int, int> OrdersByHour { get; set; } = new Dictionary<int, int>();
        public Dictionary<PaymentMethod, decimal> SalesByPaymentMethod { get; set; } = new Dictionary<PaymentMethod, decimal>();
    }
}