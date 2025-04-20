## Project Structure

```
restaurant-app/
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
    ├── AppShell.xaml        # Navigation shell
│   └── MauiProgram.cs       # App configuration and DI setup
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
