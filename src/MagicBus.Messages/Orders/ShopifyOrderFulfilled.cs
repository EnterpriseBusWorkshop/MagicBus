using MagicBus.Messages.Common;

namespace MagicBus.Messages.Orders
{
    public class ShopifyOrderFulfilled: MessageBase
    {
        public long ShopifyOrderId { get; set; }
    }
}