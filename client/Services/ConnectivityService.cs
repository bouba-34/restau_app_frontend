using Shared.Services.Interfaces;

namespace Shared.Services.Implementations
{
    public class ConnectivityService : IConnectivityService
    {
        private readonly ISettingsService _settingsService;
        private readonly Timer _monitoringTimer;
        private bool _isConnected = true;
        
        public event EventHandler<bool> ConnectivityChanged;
        
        public bool IsConnected 
        { 
            get => _isConnected;
            private set
            {
                if (_isConnected != value)
                {
                    _isConnected = value;
                    ConnectivityChanged?.Invoke(this, _isConnected);
                }
            }
        }
        
        public ConnectivityService(ISettingsService settingsService)
        {
            _settingsService = settingsService;
            
            // Setup monitoring timer (check every 30 seconds)
            _monitoringTimer = new Timer(async _ => await CheckConnectivityInternalAsync(), 
                null, Timeout.Infinite, Timeout.Infinite);
        }
        
        public async Task<bool> CheckConnectivityAsync()
        {
            try
            {
                var current = Connectivity.NetworkAccess;
                
                // Check if device has internet access
                bool isConnected = current == NetworkAccess.Internet;
                
                // If connected, also ping the server to make sure it's reachable
                if (isConnected)
                {
                    using var client = new HttpClient();
                    var baseUrl = _settingsService.ApiBaseUrl;
                    
                    if (!string.IsNullOrEmpty(baseUrl))
                    {
                        // Set a short timeout for the ping check
                        client.Timeout = TimeSpan.FromSeconds(5);
                        
                        // Try to connect to server
                        try
                        {
                            var response = await client.GetAsync(baseUrl);
                            isConnected = response.IsSuccessStatusCode;
                        }
                        catch
                        {
                            isConnected = false;
                        }
                    }
                }
                
                IsConnected = isConnected;
                return isConnected;
            }
            catch
            {
                IsConnected = false;
                return false;
            }
        }
        
        private async Task CheckConnectivityInternalAsync()
        {
            await CheckConnectivityAsync();
        }
        
        public void StartMonitoring()
        {
            // Start checking immediately and then every 30 seconds
            _monitoringTimer.Change(0, 30000);
            
            // Additionally, listen to connectivity changes
            Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;
        }
        
        public void StopMonitoring()
        {
            _monitoringTimer.Change(Timeout.Infinite, Timeout.Infinite);
            Connectivity.ConnectivityChanged -= Connectivity_ConnectivityChanged;
        }
        
        private void Connectivity_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            // When connectivity changes, check full connectivity
            CheckConnectivityAsync().ContinueWith(t => 
            {
                if (t.Exception != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Connectivity check error: {t.Exception.Message}");
                }
            });
        }
    }
}
