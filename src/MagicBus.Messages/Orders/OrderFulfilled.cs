using MagicBus.Messages.Common;

namespace MagicBus.Messages.Orders
{
    public class OrderFulfilled: MessageBase
    {
        public  string OrderId { get; set; }
    }
}