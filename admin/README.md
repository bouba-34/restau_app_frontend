# Restaurant Admin System

A comprehensive restaurant management system built with .NET MAUI that provides restaurant owners and managers with a powerful, cross-platform administration tool.

![Dashboard Overview](screenshots/dashboard.png)

## Features

### Dashboard
- Real-time sales analytics and visualizations
- Current day's order summary
- Active orders and upcoming reservations
- Best-selling items tracking
- Charts for sales by category and orders by hour

![Dashboard Analytics](screenshots/dashboard_analytics.png)

### Order Management
- View and manage all orders
- Filter by status (Placed, Preparing, Ready, Served, Completed, Cancelled)
- Filter by date range
- Real-time status updates
- Detailed order information
- Process payments
- Update order status with a single tap

![Orders Page](screenshots/orders.png)
![Order Details](screenshots/order_details.png)

### Reservations
- Manage table reservations
- Filter by status (Pending, Confirmed, Completed, Cancelled, No-Show)
- Filter by date
- Confirm reservations
- Mark reservations as completed or no-show
- View customer contact information

![Reservations Page](screenshots/reservations.png)
![Reservation Details](screenshots/reservation_details.png)

### Menu Management
- Comprehensive menu item management
- Add, edit, and delete menu items
- Upload item images
- Set prices, descriptions, and preparation times
- Mark items as featured
- Toggle item availability
- Organize items by categories
- Add dietary information (vegetarian, vegan, gluten-free)
- List ingredients and allergens

![Menu Management](screenshots/menu.png)
![Menu Item Detail](screenshots/menu_item_detail.png)

### Staff Management
- Add and manage staff accounts
- Assign different roles (Admin, Staff)
- Reset passwords
- Enable/disable accounts
- View staff contact information

![Staff Management](screenshots/staff.png)

### System Preferences
- Theme customization (Light/Dark mode)
- API configuration
- Real-time notifications settings
- Account management


## Technical Details

### Architecture
- **MVVM Pattern**: Clear separation of concerns with Models, Views, and ViewModels
- **Dependency Injection**: Services registered and injected for better testability
- **Repository Pattern**: Consistent data access layer

### Technology Stack
- **.NET MAUI** (Multi-platform App UI): For cross-platform development
- **C# & XAML**: Core programming languages
- **CommunityToolkit.MVVM**: For implementing the MVVM pattern
- **LiveChartsCore**: For data visualization and charts
- **SignalR**: For real-time communication with the backend
- **HttpClient**: For RESTful API communication

### Platforms Supported (Multiplatform but Windows based)

- Windows

## Getting Started

### Prerequisites
- Visual Studio 2022 or later with .NET MAUI workload (or Rider)
- .NET 9.0 SDK
- Backend API service running (see Backend Repository)

### Installation

1. Clone the repository
```bash
git clone https://github.com/bouba-34/restau_app_frontend.git
```

2. Open the solution in Visual Studio

3. Configure the API endpoint in `AppConstants.cs`

4. Build and run the application

### Configuration

The application is designed to connect to a backend API. You can configure the API endpoint in the settings page or by modifying the default values in the `AppConstants.cs` file.

## Project Structure

```
├── admin/
│   ├── Controls/              # Reusable UI components
│   ├── Converters/            # Value converters
│   ├── Helpers/               # Utility classes
│   ├── Models/                # Local data models
│   ├── Resources/             # App resources (styles, colors, fonts)
│   ├── Services/              # Business logic services
│   │   ├── Implementation/    # Service implementations
│   │   └── Interfaces/        # Service interfaces
│   ├── ViewModels/            # MVVM ViewModels
│   ├── Views/                 # XAML UI pages
│   └── App.xaml               # Application entry point
└── Shared/                    # Shared library with common models and services
```

## Controls

The application includes several custom controls for consistent UI experience:

- `CardView`: Standard card container with optional header and actions
- `LoadingIndicator`: Customizable loading spinner with text
- `MenuItemCard`: Card layout for displaying menu items
- `OrderCard`: Card layout for displaying order information
- `ReservationCard`: Card layout for displaying reservation information
- `StatisticsCard`: Card for displaying metrics with icons and comparison data
- `StatusBadge`: Color-coded status indicator

## Authentication

The system provides a secure authentication mechanism:
- Login with username and password
- Role-based access (Admin, Staff)
- Automatic token refresh
- Secure credential storage

![Login Screen](screenshots/login.png)

## Real-time Updates

The application uses SignalR for real-time updates:
- New orders notifications
- Status changes
- Reservation updates

## Error Handling

Comprehensive error handling with user-friendly error messages and logging.

## Contributing

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Acknowledgments

- Font Awesome for the icon set
- LiveChartsCore for the chart visualizations
- Microsoft for .NET MAUI framework

---

