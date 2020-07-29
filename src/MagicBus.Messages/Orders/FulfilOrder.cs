using System.Xml.Schema;
using MagicBus.Messages.Common;

namespace MagicBus.Messages.Orders
{
    /// <summary>
    /// command to fulfil an order
    /// </summary>
    public class FulfilOrder: MessageBase
    {
        public string OrderId { get; set; }

        public string EmailAddress  { get; set; }

        public string Name  { get; set; }

        public string Sku { get; set; }
        
    }
}