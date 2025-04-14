using Shared.Models.Dashboard;
using Shared.Models.Report;

namespace Shared.Services.Interfaces
{
    public interface IReportService
    {
        Task<SalesReport> GetDailySalesReportAsync(DateTime date);
        Task<SalesReport> GetWeeklySalesReportAsync(DateTime startDate);
        Task<SalesReport> GetMonthlySalesReportAsync(int year, int month);
        Task<List<MenuItemSales>> GetTopSellingItemsAsync(DateTime startDate, DateTime endDate, int limit = 10);
        Task<Dictionary<string, decimal>> GetSalesByCategoryAsync(DateTime startDate, DateTime endDate);
        Task<Dictionary<int, int>> GetOrdersByHourAsync(DateTime date);
        Task<Dictionary<string, decimal>> GetSalesByPaymentMethodAsync(DateTime startDate, DateTime endDate);
        Task<byte[]> ExportSalesReportAsync(DateTime startDate, DateTime endDate, string format = "csv");
        Task<DashboardSummary> GetDashboardSummaryAsync();
    }
}