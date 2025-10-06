using System.Diagnostics;
using System.Text.Json;
using ABCRetail.Models;
using ABCRetail.Models.ViewModels;
using ABCRetail.Services;
using Microsoft.AspNetCore.Mvc;
using ABCRetail.Models;

namespace ABCRetail.Controllers
{
    public class HomeController : Controller
    {
        private readonly IAzureStorageService _storageService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public HomeController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public HomeController(IAzureStorageService storageService)
        {
            _storageService = storageService;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _storageService.GetAllEntitiesAsync<Product>();
            var customers = await _storageService.GetAllEntitiesAsync<Customer>();
            var orders = await _storageService.GetAllEntitiesAsync<Order>();

            var viewModel = new HomeViewModel
            {
                FeaturedProducts = products.Take(5).ToList(),
                ProductCount = products.Count,
                CustomerCount = customers.Count,
                OrderCount = orders.Count
            };

            return View(viewModel);
        
            //PART 2
            var httpClient = _httpClientFactory.CreateClient();
            var apiBaseUrl = _configuration["FunctionApi:BaseUrl"];

            try
            {
                var httpResponseMessage = await httpClient.GetAsync($"{apiBaseUrl}people");
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    using var contentStream = await
                    httpResponseMessage.Content.ReadAsStreamAsync();
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                    };
                    var people = await
                    JsonSerializer.DeserializeAsync<IEnumerable<ApiModels>>
                    (contentStream, options);
                    return View(people);
                }
            }
            catch (HttpRequestException)
            {
                ViewBag.ErrorMessage = "Could not connect to the API." +
                "Please ensure the Azure Function is running.";
                return View(new List<ApiModels>());
            }
            ViewBag.ErrorMessage = "An error occured while retrieving data from the API.";
            return View(new List<ApiModels>());
        }

        [HttpGet]
        public IActionResult AddWithImage()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddWithImage(AddProductImage model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var httpClient = _httpClientFactory.CreateClient();
            var apiBaseUrl = _configuration["FunctionApi:BaseUrl"];
            // Use MulitpartFormDataContent to send the form data and the file
            using var formData = new MultipartFormDataContent();
            formData.Add(new StringContent(model.Name), "Name");
            formData.Add(new StringContent(model.EmailAddress), "Email");
            if (model.ProductImage != null)
            {
                formData.Add(new StreamContent(
                    model.ProductImage.OpenReadStream()),
                    "ProfileImage",
                    model.ProductImage.FileName);
            }

            var httpResponseMessage = await
                httpClient.PostAsync($"{apiBaseUrl}people-with-image", formData);

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = $"Successfully added" +
                    $"{model.Name} with an image";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError(string.Empty, "An error occured while" +
                "calling the API.");
            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> InitializeStorage()
        {
            try
            {
                // Force re-initialization of storage
                await _storageService.GetAllEntitiesAsync<Customer>(); // This will trigger initialization
                TempData["Success"] = "Azure Storage initialized successfully!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Failed to initialize storage: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]

        //ICE task 4
        public IActionResult Contact()
        {
            return View();
        }

    }
}