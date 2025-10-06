using System.Text.Json;
using Azure.Storage.Queues;

namespace TestQueue
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            //Connection Sring
            var connectionString = "DefaultEndpointsProtocol=https;AccountName=cldvpoestorageacc;AccountKey=yIyP8YhKsjBu3yScS9jKzKRUIBwOit80Gwmj8zdmysv2JbPk82fFwIZTwe7vNsyqz4u8t2fn+jSx+ASt9+9BOg==;EndpointSuffix=core.windows.net";
            // Queue name must match the one your function listens to
            var queueClient = new QueueClient(
                connectionString,
                "stock-updates",
                new QueueClientOptions { MessageEncoding = QueueMessageEncoding.Base64 } // ensure its base64
            );
            // Create queue if it doesn't exit
            await queueClient.CreateIfNotExistsAsync();
            // Build test object
            var person = new { Name = "Kristen Oliver", Email = "Kristen@gmail.com" };
            // Serialize object to JSON
            string json = JsonSerializer.Serialize(person);
            // Send as plain JSON string
            await queueClient.SendMessageAsync(json);
            Console.WriteLine($"Message sent: {json}");
        }
    }
}
