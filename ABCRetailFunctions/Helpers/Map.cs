using ABCRetailFunctions.Functions.Entities;
using ABCRetailFunctions.Models;

namespace ABCRetailFunctions.Helpers;

public static class Map
{
    // Table Entity --> DTOs returned to MVC

    public static CustomerDto ToDto(CustomerEntity e)
    => new(
    Id: e.RowKey,
    Name: e.Name,
    Surname: e.Surname,
    Username: e.Username,
    EmailAddress: e.EmailAddress,
    ShippingAddress: e.ShippingAddress
    );


    public static ProductDto ToDto(ProductEntity e)
    => new(
    Id: e.RowKey,
    ProductName: e.ProductName,
    Description: e.Description,
    Price: (decimal)e.Price,
    StockAvailable: e.StockAvailable,
    ImageURL: e.ImageURL
    );

    public static OrderDto ToDto(OrderEntity e)
    {
        var unitPrice = (decimal)e.UnitPrice;
        var total = unitPrice * e.Quantity;

        return new OrderDto
        (
            Id: e.RowKey,
            CustomerID: e.CustomerID,
            ProductID: e.ProductID,
            ProductName: e.ProductName,
            Quantity: e.Quantity,
            UnitPrice: unitPrice,
            TotalAmount: total,
            OrderDate: e.OrderDate,
            Status: e.Status
        );
    }
}
