using Shared.Constants;
using Shared.Models.Common;
using Shared.Models.Dashboard;
using Shared.Models.Report;
using Shared.Services.Helpers;
using Shared.Services.Interfaces;

namespace Shared.Services.Implementations
{
    public class ReportService : ServiceBase, IReportService
    {
        private readonly ICacheService _cacheService;

        public ReportService(
            HttpClient httpClient,
            ISettingsService settingsService,
            ICacheService cacheService)
            : base(httpClient, settingsService)
        {
            _cacheService = cacheService;
        }

        public async Task<SalesReport> GetDailySalesReportAsync(DateTime date)
        {
            var endpoint = string.Format(ApiEndpoints.GetDailySalesReport, date.ToString("yyyy-MM-dd"));
            var response = await GetAsync<ApiResponse<SalesReport>>(endpoint);
            
            if (response.Success && response.Data != null)
            {
                return response.Data;
            }
            
            return new SalesReport();
        }

        public async Task<SalesReport> GetWeeklySalesReportAsync(DateTime startDate)
        {
            var endpoint = string.Format(ApiEndpoints.GetWeeklySalesReport, startDate.ToString("yyyy-MM-dd"));
            var response = await GetAsync<ApiResponse<SalesReport>>(endpoint);
            
            if (response.Success && response.Data != null)
            {
                return response.Data;
            }
            
            return new SalesReport();
        }

        public async Task<SalesReport> GetMonthlySalesReportAsync(int year, int month)
        {
            var endpoint = string.Format(ApiEndpoints.GetMonthlySalesReport, year, month);
            var response = await GetAsync<ApiResponse<SalesReport>>(endpoint);
            
            if (response.Success && response.Data != null)
            {
                return response.Data;
            }
            
            return new SalesReport();
        }

        public async Task<List<MenuItemSales>> GetTopSellingItemsAsync(DateTime startDate, DateTime endDate, int limit = 10)
        {
            var endpoint = string.Format(ApiEndpoints.GetTopSellingItems, 
                startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"), limit);
            
            var response = await GetAsync<ApiResponse<List<MenuItemSales>>>(endpoint);
            
            if (response.Success && response.Data != null)
            {
                return response.Data;
            }
            
            return new List<MenuItemSales>();
        }

        public async Task<Dictionary<string, decimal>> GetSalesByCategoryAsync(DateTime startDate, DateTime endDate)
        {
            var endpoint = string.Format(ApiEndpoints.GetSalesByCategory, 
                startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"));
            
            var response = await GetAsync<ApiResponse<Dictionary<string, decimal>>>(endpoint);
            
            if (response.Success && response.Data != null)
            {
                return response.Data;
            }
            
            return new Dictionary<string, decimal>();
        }

        public async Task<Dictionary<int, int>> GetOrdersByHourAsync(DateTime date)
        {
            var endpoint = string.Format(ApiEndpoints.GetOrdersByHour, date.ToString("yyyy-MM-dd"));
            var response = await GetAsync<ApiResponse<Dictionary<int, int>>>(endpoint);
            
            if (response.Success && response.Data != null)
            {
                return response.Data;
            }
            
            return new Dictionary<int, int>();
        }

        public async Task<Dictionary<string, decimal>> GetSalesByPaymentMethodAsync(DateTime startDate, DateTime endDate)
        {
            var endpoint = string.Format(ApiEndpoints.GetSalesByPaymentMethod, 
                startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"));
            
            var response = await GetAsync<ApiResponse<Dictionary<string, decimal>>>(endpoint);
            
            if (response.Success && response.Data != null)
            {
                return response.Data;
            }
            
            return new Dictionary<string, decimal>();
        }

        public async Task<byte[]> ExportSalesReportAsync(DateTime startDate, DateTime endDate, string format = "csv")
        {
            var endpoint = string.Format(ApiEndpoints.ExportSalesReport, 
                startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"), format);
            
            return await GetFileAsync(endpoint);
        }

        public async Task<DashboardSummary> GetDashboardSummaryAsync()
        {
            var cacheKey = "DashboardSummary";
            
            // Check cache first
            var cachedSummary = _cacheService.Get<DashboardSummary>(cacheKey);
            if (cachedSummary != null)
            {
                return cachedSummary;
            }
            
            // If not in cache, get from API
            var response = await GetAsync<ApiResponse<DashboardSummary>>(ApiEndpoints.GetDashboardSummary);
            
            if (response.Success && response.Data != null)
            {
                // Cache for 5 minutes
                _cacheService.Set(cacheKey, response.Data, TimeSpan.FromMinutes(5));
                return response.Data;
            }
            
            return new DashboardSummary
            {
                Today = new SalesSummary(),
                Yesterday = new SalesSummary(),
                ThisWeek = new SalesSummary(),
                LastWeek = new SalesSummary(),
                ThisMonth = new SalesSummary(),
                OrdersByHour = new Dictionary<int, int>(),
                SalesByCategory = new Dictionary<string, decimal>(),
                TopSellingItems = new List<MenuItemSales>()
            };
        }
    }
}