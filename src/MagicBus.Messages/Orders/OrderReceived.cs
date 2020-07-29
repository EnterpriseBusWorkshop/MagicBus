using MagicBus.Messages.Common;
using MagicBus.Messages.Models;

namespace MagicBus.Messages.Orders
{
    public class OrderReceived: MessageBase
    {
        public ShopifySharp.Order OrderDetails { get; set; }
    }
    
}