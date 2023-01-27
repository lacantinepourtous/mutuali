namespace YellowDuck.Api.Constants
{
    public static class StripeEnums
    {
        public const string PaymentStatus_Paid = "paid";
        public const string PaymentStatus_Unpaid= "unpaid";
        public const string PaymentStatus_NoPaymentRequired = "no_payment_required";

        public const string CancellationReason_Duplicate = "duplicate";
        public const string CancellationReason_Fraudulent = "fraudulent";
        public const string CancellationReason_RequestedByCustomer = "requested_by_customer";
        public const string CancellationReason_Abandoned = "abandoned";

        public const string PaymentIntentStatus_RequiresPaymentMethod = "requires_payment_method";
        public const string PaymentIntentStatus_RequiresConfirmation = "requires_confirmation";
        public const string PaymentIntentStatus_RequiresAction = "requires_action";
        public const string PaymentIntentStatus_Processing = "processing";
        public const string PaymentIntentStatus_Canceled = "canceled";
        public const string PaymentIntentStatus_Succeeded = "succeeded";

        public const string CheckoutSessionExpired = "checkout.session.expired";
    }
}
