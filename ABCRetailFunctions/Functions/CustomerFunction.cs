using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ABCRetailFunctions.Functions.Entities;
using ABCRetailFunctions.Helpers;
using ABCRetailFunctions.Models;
using Azure.Data.Tables;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;

namespace ABCRetailFunctions.Functions;
public class CustomerFunctions
{
    private readonly string _conn;
    private readonly string _table;

    public CustomerFunctions(IConfiguration cfg)
    {
        _conn = cfg["STORAGE_CONNECTION"] ?? throw new InvalidOperationException("STORAGE_CONNECTION missing");
        _table = cfg["TABLE_CUSTOMER"] ?? "Customer";
    }

    [Function("Customers_List")]
    public async Task<HttpResponseData> List(
    [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "customers")] HttpRequestData req)
    {
        var table = new TableClient(_conn, _table);
        await table.CreateIfNotExistsAsync();

        var items = new List<CustomerDto>();
        await foreach (var e in table.QueryAsync<CustomerEntity>(x => x.PartitionKey == "Customer"))
            items.Add(Map.ToDto(e));

        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(items);
        return response;
    }


    [Function("Customers_Get")]
    public async Task<HttpResponseData> Get(
    [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "customers/{id}")] HttpRequestData req, string id)
    {
        var table = new TableClient(_conn, _table);
        try
        {
            var e = await table.GetEntityAsync<CustomerEntity>("Customer", id);
            return HttpJson.Ok(req, Map.ToDto(e.Value));
        }
        catch
        {
            return HttpJson.NotFound(req, "Customer not found");
        }
    }

    public record CustomerCreateUpdate(string? Name, string? Surname, string? Username, string? EmailAddress, string? ShippingAddress);

    [Function("Customers_Create")]
    public async Task<HttpResponseData> Create(
    [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "customers")] HttpRequestData req)
    {
        var input = await HttpJson.ReadAsync<CustomerCreateUpdate>(req);
        if (input is null || string.IsNullOrWhiteSpace(input.Name) || string.IsNullOrWhiteSpace(input.EmailAddress))
        {
            return HttpJson.Bad(req, "Name and Email are required");
        }

        var table = new TableClient(_conn, _table);
        await table.CreateIfNotExistsAsync();

        var e = new CustomerEntity
        {
            Name = input.Name!,
            Surname = input.Surname ?? "",
            Username = input.Username ?? "",
            EmailAddress = input.EmailAddress!,
            ShippingAddress = input.ShippingAddress ?? ""
        };
        await table.AddEntityAsync(e);
        return HttpJson.Created(req, Map.ToDto(e));
    }

    [Function("Customers_Update")]
    public async Task<HttpResponseData> Update(
    [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "customers/{id}")] HttpRequestData req, string id)
    {
        var input = await HttpJson.ReadAsync<CustomerCreateUpdate>(req);
        if (input is null) return HttpJson.Bad(req, "Invalid body");

        var table = new TableClient(_conn, _table);
        try
        {
            var resp = await table.GetEntityAsync<CustomerEntity>("Customer", id);
            var e = resp.Value;

            e.Name = input.Name ?? e.Name;
            e.Surname = input.Surname ?? e.Surname;
            e.Username = input.Username ?? e.Username;
            e.EmailAddress = input.EmailAddress ?? e.EmailAddress;
            e.ShippingAddress = input.ShippingAddress ?? e.ShippingAddress;

            await table.UpdateEntityAsync(e, e.ETag, TableUpdateMode.Replace);
            return HttpJson.Ok(req, Map.ToDto(e));
        }
        catch
        {
            return HttpJson.NotFound(req, "Customer not found");
        }
    }

    [Function("Customers_Delete")]
    public async Task<HttpResponseData> Delete(
    [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "customers/{id}")] HttpRequestData req, string id)
    {
        var table = new TableClient(_conn, _table);
        await table.DeleteEntityAsync("Customer", id);
        return HttpJson.NoContent(req);
    }

}
