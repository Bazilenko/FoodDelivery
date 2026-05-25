namespace Orders.Dal.Enums
{
    public enum OrderStatus
    {
        Pending,
        Confirmed,
        Preparing,
        ReadyForPickup,
        OutForDelivery,
        Delivered,
        CancelledByCustomer,
        CancelledByRestaurant,
        Failed
    }
}