using Shared.Services.Interfaces;

namespace Client.Services
{
    public class NavigationService : INavigationService
    {
        public async Task NavigateToAsync(string route, IDictionary<string, object> parameters = null)
        {
            Console.WriteLine($"Navigating to {route}");
            await InvokeOnMainThreadAsync(() => NavigateToRoute(route, parameters));
        }

        public async Task PushAsync(string route, IDictionary<string, object> parameters = null)
        {
            await InvokeOnMainThreadAsync(() => Shell.Current.GoToAsync(route, parameters));
        }

        public async Task PopAsync()
        {
            await InvokeOnMainThreadAsync(() => Shell.Current.GoToAsync(".."));
        }

        public async Task PopToRootAsync()
        {
            await InvokeOnMainThreadAsync(() => Shell.Current.GoToAsync("//"));
        }

        public async Task GoBackAsync()
        {
            await InvokeOnMainThreadAsync(() => Shell.Current.GoToAsync(".."));
        }
        
        private static void NavigateToRoute(string route, IDictionary<string, object> parameters = null)
        {
            var safeParams = parameters ?? new Dictionary<string, object>();

            if (route.StartsWith("//"))
            {
                Shell.Current.GoToAsync(route, safeParams);
            }
            else
            {
                Shell.Current.GoToAsync(route, safeParams);
            }
        }
        
        private static async Task InvokeOnMainThreadAsync(Action action)
        {
            if (MainThread.IsMainThread)
            {
                action();
            }
            else
            {
                await MainThread.InvokeOnMainThreadAsync(action);
            }
        }
    }
}