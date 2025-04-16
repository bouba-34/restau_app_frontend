using Shared.Models.Menu;
using System.Windows.Input;

namespace admin.Controls;

public partial class MenuItemCard : ContentView
{
    public static readonly BindableProperty MenuItemProperty =
        BindableProperty.Create(nameof(MenuItem), typeof(Shared.Models.Menu.MenuItem), typeof(MenuItemCard), null);

    public static readonly BindableProperty EditCommandProperty =
        BindableProperty.Create(nameof(EditCommand), typeof(ICommand), typeof(MenuItemCard), null);

    public static readonly BindableProperty ToggleAvailabilityCommandProperty =
        BindableProperty.Create(nameof(ToggleAvailabilityCommand), typeof(ICommand), typeof(MenuItemCard), null);

    public Shared.Models.Menu.MenuItem MenuItem
    {
        get => (Shared.Models.Menu.MenuItem)GetValue(MenuItemProperty);
        set => SetValue(MenuItemProperty, value);
    }

    public ICommand EditCommand
    {
        get => (ICommand)GetValue(EditCommandProperty);
        set => SetValue(EditCommandProperty, value);
    }

    public ICommand ToggleAvailabilityCommand
    {
        get => (ICommand)GetValue(ToggleAvailabilityCommandProperty);
        set => SetValue(ToggleAvailabilityCommandProperty, value);
    }

    public MenuItemCard()
    {
        InitializeComponent();
    }
}