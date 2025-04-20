# Restaurant Management App

A cross-platform mobile application built with .NET MAUI that allows customers to browse menus, place orders, make reservations, and receive real-time notifications about their orders and reservations.

## Features

### Authentication & User Profile
- User registration and login functionality
- Profile management with personal information
- Secure storage of user credentials
- Password reset capability

![Authentication Screen](screenshots/auth_screen.png)

### Menu Management
- Browse menu categories and items
- View detailed item descriptions, ingredients, and allergen information
- Search functionality to find specific items
- Filter by dietary preferences (vegetarian, vegan, gluten-free)
- View special offers and discounts

![Menu Screen](screenshots/menu_screen.png)

### Order Management
- Add items to cart with quantity adjustment
- Customization options for menu items
- Special instructions for orders
- Different order types (Dine-in, Takeout, Delivery)
- Real-time order status updates
- Order history with reordering capability
- Tipping options

![Order Screen](screenshots/order_screen.png)

### Reservation System
- Select date, time, and party size
- View available tables
- Special requests for reservations
- Reservation confirmation and status updates
- Reservation history and management

![Reservation Screen](screenshots/reservation_screen.png)

### Notifications
- Real-time notifications for order status changes
- Reservation confirmations and reminders
- Special offers and promotions
- Push notifications with local device integration


### Settings
- Dark mode toggle
- Notification preferences
- Cache management
- App information and about
- Privacy policy


## Technical Details

### Architecture
The application follows the MVVM (Model-View-ViewModel) architectural pattern:
- **Models**: Data structures representing business entities
- **Views**: XAML UI for different screens and features
- **ViewModels**: Business logic and state management
- **Services**: API communication and other utilities

### Technologies & Libraries
- **.NET MAUI**: For cross-platform UI development
- **CommunityToolkit.Mvvm**: MVVM infrastructure
- **CommunityToolkit.Maui**: UI utilities and controls
- **SignalR**: Real-time communication with the server
- **Plugin.LocalNotification**: Device notifications
- **SecureStorage**: Secure credential storage
- **HttpClient**: API communication

### Key Components
- **AppShell**: Navigation and routing
- **ViewModelLocator**: Dependency injection for ViewModels
- **Custom Controls**: Reusable UI components
- **Converters**: Value transformation for UI bindings

## Getting Started

### Prerequisites
- Visual Studio 2022+ or Rider with Mobile Development workload
- .NET 9.0 SDK
- Android SDK (for Android builds)

### Setup
1. Clone the repository:
   ```
   git clone https://github.com/yourusername/restau_app_frontend.git
   ```

2. Open the solution in Visual Studio:
   ```
   cd restau_app_frontend
   ```

3. Restore NuGet packages:
   ```
   dotnet restore
   ```

4. Configure the API endpoint:
    - Open `client/Services/SettingsService.cs`
    - Update the `DefaultApiBaseUrl` constant with your API endpoint

5. Build and run the application:
   ```
   dotnet build
   ```

### API Integration
The application requires a backend API for full functionality. Make sure the API supports:
- User authentication and management
- Menu items and categories
- Order management
- Reservation system
- SignalR for real-time updates

## Project Structure

```
restaurant-app/
│
├── Client/
│   ├── Constants/           # App constants and routes
│   ├── Controls/            # Reusable UI components
│   ├── Converters/          # Value converters for UI
│   ├── Platforms/           # Platform-specific code
│   ├── Resources/           # Images, fonts, and styles
│   ├── Services/            # API and utility services
│   ├── ViewModels/          # Business logic components
│   │   ├── Auth/            # Authentication ViewModels
│   │   ├── Base/            # Base ViewModels and infrastructure
│   │   ├── Menu/            # Menu-related ViewModels
│   │   ├── Notification/    # Notification ViewModels
│   │   ├── Order/           # Order management ViewModels
│   │   ├── Reservation/     # Reservation ViewModels
│   │   └── Settings/        # Settings ViewModels
│   ├── Views/               # XAML UI pages
│   │   ├── Auth/            # Authentication pages
│   │   ├── Menu/            # Menu browsing pages
│   │   ├── Notification/    # Notification pages
│   │   ├── Order/           # Order management pages
│   │   ├── Reservation/     # Reservation pages
│   │   └── Settings/        # Settings pages
│   ├── App.xaml             # Application entry point
│   ├── AppShell.xaml        # Navigation shell
│   └── MauiProgram.cs       # App configuration and DI setup
│
└── Shared/                  # Shared models and services
    ├── Constants/           # Shared constants
    ├── Helpers/             # Utility helpers
    ├── Models/              # Data models
    │   ├── Auth/            # Authentication models
    │   ├── Menu/            # Menu-related models
    │   ├── Notification/    # Notification models
    │   ├── Order/           # Order models
    │   └── Reservation/     # Reservation models
    └── Services/            # Shared services and interfaces
```

## Key Features in Code

### Dynamic UI Binding
```csharp
[ObservableProperty]
private ObservableCollection<MenuItem> _menuItems;
```

### Command Pattern
```csharp
[RelayCommand]
private async Task PlaceOrderAsync()
{
    // Order placement logic
}
```

### Real-time Updates with SignalR
```csharp
private void OnOrderStatusChanged(object sender, (string OrderId, OrderStatus Status) e)
{
    if (e.OrderId == _orderId)
    {
        // Update order status
        MainThread.BeginInvokeOnMainThread(async () => {
            await LoadOrderAsync();
        });
    }
}
```

## Contribution Guidelines

1. Fork the repository
2. Create a feature branch: `git checkout -b feature/my-feature`
3. Commit your changes: `git commit -am 'Add new feature'`
4. Push to the branch: `git push origin feature/my-feature`
5. Submit a pull request

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Acknowledgements

- .NET MAUI team for the cross-platform framework
- Community Toolkit contributors
- All contributors to this project