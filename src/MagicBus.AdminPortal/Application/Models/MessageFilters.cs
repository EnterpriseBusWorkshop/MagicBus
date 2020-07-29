using System;

namespace MagicBus.AdminPortal.Application.Models
{
	public class MessageFilters
    {
        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo { get; set; }
        public string SbQueue { get; set; } //Used for DeadLetters

        public string TypeName { get; set; }

        public Type MessageType { get; set; }

        public string MessageId { get; set; }

        public string CorrelationId { get; set; }

        public int PageIndex { get; set; } = 0;

        public int PageSize { get; set; } = 20;

    }
}
