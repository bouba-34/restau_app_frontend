namespace Shared.Constants
{
    public static class Messages
    {
        // Success messages
        public const string LoginSuccess = "Login successful";
        public const string RegisterSuccess = "Registration successful";
        public const string OrderPlaced = "Your order has been placed successfully";
        public const string ReservationCreated = "Your reservation has been created successfully";
        public const string ProfileUpdated = "Profile updated successfully";
        public const string PasswordChanged = "Password changed successfully";
        public const string ItemAddedToCart = "Item added to cart";
        public const string CartUpdated = "Cart updated";
        public const string PaymentSuccessful = "Payment processed successfully";
        public const string NotificationSent = "Notification sent successfully";
        
        // Error messages
        public const string LoginFailed = "Invalid username or password";
        public const string RegisterFailed = "Registration failed. Please try again.";
        public const string NetworkError = "Network error. Please check your connection.";
        public const string UnauthorizedAccess = "Unauthorized access. Please login again.";
        public const string ServerError = "Server error occurred. Please try again later.";
        public const string ValidationError = "Please check your input and try again.";
        public const string ItemNotAvailable = "This item is currently not available";
        public const string EmptyCart = "Your cart is empty";
        public const string ReservationFailed = "Failed to create reservation. Please try again.";
        public const string NoTablesAvailable = "No tables available for the selected time and party size";
        
        // Information messages
        public const string LoadingData = "Loading data...";
        public const string ProcessingRequest = "Processing your request...";
        public const string WaitingForPayment = "Waiting for payment...";
        public const string OrderInProgress = "Your order is in progress";
        public const string ReservationPending = "Your reservation is pending confirmation";
        
        // Confirmation messages
        public const string ConfirmLogout = "Are you sure you want to logout?";
        public const string ConfirmCancelOrder = "Are you sure you want to cancel this order?";
        public const string ConfirmCancelReservation = "Are you sure you want to cancel this reservation?";
        public const string ConfirmPlaceOrder = "Confirm your order details";
        public const string ConfirmPayment = "Confirm payment of {0:C}";
        public const string ConfirmDeleteItem = "Are you sure you want to delete this item?";
    }
}