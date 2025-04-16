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
        DineIn = 0,
        TakeOut = 1,
        Delivery = 2
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