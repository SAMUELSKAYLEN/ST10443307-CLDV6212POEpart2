using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCRetailFunctions.Models;

public record CustomerDto(string Id, string Name, string Surname, string Username, string EmailAddress, string ShippingAddress);
public record ProductDto(string Id, string ProductName, string Description, decimal Price, int StockAvailable, string ImageURL);
public record OrderDto(
string Id, string CustomerID, string ProductID, string ProductName,
int Quantity, decimal UnitPrice, decimal TotalAmount, DateTimeOffset OrderDate, string Status);
