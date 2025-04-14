namespace Shared.Services.Interfaces
{
    public interface INavigationService
    {
        Task NavigateToAsync(string route, IDictionary<string, object> parameters = null);
        Task PushAsync(string route, IDictionary<string, object> parameters = null);
        Task PopAsync();
        Task PopToRootAsync();
        Task GoBackAsync();
    }
}