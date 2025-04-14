using Shared.Services.Interfaces;

namespace Client.Services
{
    public class NavigationService : INavigationService
    {
        public async Task NavigateToAsync(string route, IDictionary<string, object> parameters = null)
        {
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
            if (route.StartsWith("//"))
            {
                // Absolute route
                Shell.Current.GoToAsync(route, parameters);
            }
            else
            {
                // Relative route
                Shell.Current.GoToAsync(route, parameters);
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