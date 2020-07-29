using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using ShopifySharp;
using Xunit;

namespace MagicBus.Tests.Json
{
    
    /// <summary>
    /// test out ability to deserialize the json sent by shopify
    /// </summary>
    public class ShopifyJsonTests
    {

        [Fact]
        public void ShouldDeserializerOrderJson()
        {
            var json = GetFileFromEmbeddedResource("MagicBus.Tests.Json.OrderCreated.json");
            Assert.NotEmpty(json);
            var orderDetails = JsonConvert.DeserializeObject<Order>(json);
            Assert.NotNull(orderDetails);
        }
        
        
        
        public static string GetFileFromEmbeddedResource(string name) {
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(name))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
    
    
    
}