using Azure;
using Azure.Data.Tables;

namespace ABCRetailFunctions.Functions.Entities;

public class CustomerEntity : ITableEntity
{
    // Azure Table Storage required keys
    public string PartitionKey { get; set; } = "Customer";
    public string RowKey { get; set; } = Guid.NewGuid().ToString("N");

    // Metadata
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }

    // Custom properties
    public string Name { get; set; } = "";
    public string Surname { get; set; } = "";
    public string Username { get; set; } = "";
    public string EmailAddress { get; set; } = "";
    public string ShippingAddress { get; set; } = "";
}
    public class ProductEntity : ITableEntity
    {
        public string PartitionKey { get; set; } = "Product";
        public string RowKey { get; set; } = Guid.NewGuid().ToString("N");
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }

        public string? ProductName { get; set; } = "";
        public string? Description { get; set; } = "";
        public double Price { get; set; }
        public int StockAvailable { get; set; }
        public string ImageURL { get; set; } = "";
    }

    public class OrderEntity : ITableEntity
    {
   
    public string PartitionKey { get; set; } = "Order";
    public string RowKey { get; set; } = Guid.NewGuid().ToString("N");
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }

    public string CustomerID { get; set; } = "";
    public string ProductID { get; set; } = "";
    public string ProductName { get; set; } = "";
    public int Quantity { get; set; }
    public double UnitPrice { get; set; } 
    public DateTimeOffset OrderDate { get; set; } = DateTimeOffset.UtcNow;
    public string Status { get; set; } = "Submitted";
    }
