using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure;
using Azure.Data.Tables;

namespace ABCRetailFunctions
{
    internal class PersonEntity : ITableEntity
    {
        // Azure Table Storage required keys
        public string PartitionKey { get; set; } = "People";
        public string RowKey { get; set; } = Guid.NewGuid().ToString();

        // Metadata
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }

        // Custom properties
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? ProductImageURL { get; set; }

        // Optional: Add other relevant properties for retail context
        public string? ProductName { get; set; }
        public string? ProductCategory { get; set; }
        public decimal? ProductPrice { get; set; }

        // Default constructor (required by Azure Tables)
        public PersonEntity() { }

        // Convenience constructor
        public PersonEntity(string name, string email, string imageUrl)
        {
            PartitionKey = "People";
            RowKey = Guid.NewGuid().ToString();
            Name = name;
            Email = email;
            ProductImageURL = imageUrl;
        }
    }
}
    