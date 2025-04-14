namespace Shared.Models.Order
{
    public enum OrderStatus
    {
        Placed,
        Preparing,
        Ready,
        Served,
        Completed,
        Cancelled
    }
    
    public enum OrderType
    {
        DineIn,
        TakeOut,
        Delivery
    }
    
    public enum PaymentStatus
    {
        Pending,
        Paid,
        Failed,
        Refunded
    }
    
    public enum PaymentMethod
    {
        Cash,
        CreditCard,
        DebitCard,
        MobilePayment,
        GiftCard,
        OnlinePayment
    }
}