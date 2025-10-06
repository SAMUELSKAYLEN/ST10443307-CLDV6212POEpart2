using System.Net;
using System.Text.Json;
using Azure.Data.Tables;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace ABCRetailFunctions;

public class Function1
{
    private readonly ILogger<Function1> _logger;
    private TableClient _tableClient;

    public Function1(ILogger<Function1> logger)
    {
        _logger = logger;
    }

    [Function("Function1")]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");
        return new OkObjectResult("Welcome to Azure Functions!");
    }

    [Function("Products_Create")]
    public async Task<HttpResponseData> CreateProduct(
    [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "products")] HttpRequestData req)
    {
        _logger.LogInformation("Creating new product...");

        var product = await JsonSerializer.DeserializeAsync<PersonEntity>(req.Body);
        product.PartitionKey = "Products";
        product.RowKey = Guid.NewGuid().ToString();

        await _tableClient.AddEntityAsync(product);

        var response = req.CreateResponse(HttpStatusCode.Created);
        await response.WriteAsJsonAsync(product);
        return response;
    }

    [Function("Products_List")]
    public async Task<HttpResponseData> ListProducts(
    [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "products")] HttpRequestData req)
    {
        var products = new List<PersonEntity>();

        await foreach (var product in _tableClient.QueryAsync<PersonEntity>())
        {
            products.Add(product);
        }

        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(products);
        return response;
    }


    [Function("Products_Get")]
    public async Task<HttpResponseData> GetProduct(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "products/{id}")] HttpRequestData req,
        string id)
    {
        var product = await _tableClient.GetEntityAsync<PersonEntity>("Products", id);
        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(product.Value);
        return response;
    }

    [Function("Products_Update")]
    public async Task<HttpResponseData> UpdateProduct(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "products")] HttpRequestData req)
    {
        var updatedProduct = await JsonSerializer.DeserializeAsync<PersonEntity>(req.Body);
        await _tableClient.UpsertEntityAsync(updatedProduct);
        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(updatedProduct);
        return response;
    }

    [Function("Products_Delete")]
    public async Task<HttpResponseData> DeleteProduct(
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "products/{id}")] HttpRequestData req,
        string id)
    {
        await _tableClient.DeleteEntityAsync("Products", id);
        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteStringAsync("Product deleted successfully.");
        return response;
    }

}