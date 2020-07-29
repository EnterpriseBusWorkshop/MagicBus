namespace MagicBus.Messages.Common
{
    /// <summary>
    /// message not intended to do anything - just lets us send across the bus or into MessageStore
    /// </summary>
    public class TestMessage: MessageBase
    {
        public string Description { get; set; } = "This is a TEST MESSAGE";

    }
}