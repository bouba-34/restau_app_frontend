using Microsoft.AspNetCore.SignalR.Client;
using Shared.Constants;
using Shared.Models.Order;
using Shared.Models.Reservation;
using Shared.Services.Interfaces;

namespace Shared.Services.SignalR
{
    public class SignalRService : ISignalRService, IDisposable
    {
        private HubConnection _hubConnection;
        private readonly ISettingsService _settingsService;
        private bool _isConnecting = false;
        
        public event EventHandler<string> Connected;
        public event EventHandler<Exception> ConnectionFailed;
        public event EventHandler Disconnected;
        
        public event EventHandler<(string OrderId, OrderStatus Status)> OrderStatusChanged;
        public event EventHandler<string> NewOrder;
        
        public event EventHandler<(string ReservationId, ReservationStatus Status)> ReservationStatusChanged;
        public event EventHandler<string> NewReservation;
        
        public event EventHandler<(string MenuItemId, bool IsAvailable)> MenuItemAvailabilityChanged;
        
        public event EventHandler<(string Title, string Message)> NotificationReceived;
        
        public bool IsConnected => _hubConnection?.State == HubConnectionState.Connected;
        
        public SignalRService(ISettingsService settingsService)
        {
            _settingsService = settingsService;
            InitializeHub();
        }
        
        private void InitializeHub()
        {
            var baseUrl = _settingsService.ApiBaseUrl;
            if (string.IsNullOrEmpty(baseUrl))
                return;
                
            var hubUrl = $"{baseUrl.TrimEnd('/')}/{AppConstants.SignalRHubUrl}";
            
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(hubUrl, options => 
                {
                    options.AccessTokenProvider = () => 
                    {
                        return Task.FromResult(_settingsService.AuthToken);
                    };
                })
                .WithAutomaticReconnect()
                .Build();
                
            RegisterHandlers();
        }
        
        private void RegisterHandlers()
        {
            // Connection events
            _hubConnection.Reconnecting += error => 
            {
                System.Diagnostics.Debug.WriteLine($"SignalR reconnecting: {error?.Message}");
                return Task.CompletedTask;
            };
            
            _hubConnection.Reconnected += connectionId => 
            {
                System.Diagnostics.Debug.WriteLine($"SignalR reconnected with ID: {connectionId}");
                Connected?.Invoke(this, connectionId);
                return Task.CompletedTask;
            };
            
            _hubConnection.Closed += error => 
            {
                System.Diagnostics.Debug.WriteLine($"SignalR connection closed: {error?.Message}");
                Disconnected?.Invoke(this, EventArgs.Empty);
                return Task.CompletedTask;
            };
            
            // Order events
            _hubConnection.On<string, OrderStatus>("OrderStatusChanged", (orderId, status) => 
            {
                OrderStatusChanged?.Invoke(this, (orderId, status));
            });
            
            _hubConnection.On<string>("NewOrder", (orderId) => 
            {
                NewOrder?.Invoke(this, orderId);
            });
            
            // Reservation events
            _hubConnection.On<string, ReservationStatus>("ReservationStatusChanged", (reservationId, status) => 
            {
                ReservationStatusChanged?.Invoke(this, (reservationId, status));
            });
            
            _hubConnection.On<string>("NewReservation", (reservationId) => 
            {
                NewReservation?.Invoke(this, reservationId);
            });
            
            // Menu events
            _hubConnection.On<string, bool>("MenuItemAvailabilityChanged", (menuItemId, isAvailable) => 
            {
                MenuItemAvailabilityChanged?.Invoke(this, (menuItemId, isAvailable));
            });
            
            // Notification events
            _hubConnection.On<string, string>("Notification", (title, message) => 
            {
                NotificationReceived?.Invoke(this, (title, message));
            });
        }
        
        public async Task ConnectAsync()
        {
            if (IsConnected || _isConnecting)
                return;
                
            if (string.IsNullOrEmpty(_settingsService.AuthToken))
                return;
                
            try
            {
                _isConnecting = true;
                await _hubConnection.StartAsync();
                _isConnecting = false;
                
                var connectionId = _hubConnection.ConnectionId;
                System.Diagnostics.Debug.WriteLine($"SignalR connected with ID: {connectionId}");
                Connected?.Invoke(this, connectionId);
            }
            catch (Exception ex)
            {
                _isConnecting = false;
                System.Diagnostics.Debug.WriteLine($"SignalR connection failed: {ex.Message}");
                ConnectionFailed?.Invoke(this, ex);
                
                // Try to reconnect after a delay if auth token is available
                if (!string.IsNullOrEmpty(_settingsService.AuthToken))
                {
                    await Task.Delay(5000);
                    await ConnectAsync();
                }
            }
        }
        
        public async Task DisconnectAsync()
        {
            if (_hubConnection != null && IsConnected)
            {
                await _hubConnection.StopAsync();
                System.Diagnostics.Debug.WriteLine("SignalR disconnected");
                Disconnected?.Invoke(this, EventArgs.Empty);
            }
        }
        
        public void Dispose()
        {
            DisconnectAsync().Wait();
            _hubConnection?.DisposeAsync().AsTask().Wait();
        }
    }
}