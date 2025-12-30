using Microsoft.AspNetCore.Mvc;
using OpenTelemetry.Trace;

namespace OrderService.Controllers
{
    [ApiController]
    [Route("/")]
    public class OrderController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly TracerProvider _tracerProvider; // Теперь храним TracerProvider
        private readonly ILogger<OrderController> _logger;

        public OrderController(
            ILogger<OrderController> logger, 
            IHttpClientFactory httpClientFactory,
            TracerProvider tracerProvider) // Получаем TracerProvider
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _tracerProvider = tracerProvider;
        }

        [HttpGet]
        public async Task<IActionResult> GetOrder()
        {
            // Получаем Tracer по имени сервиса
            var tracer = _tracerProvider.GetTracer("OrderService");
            
            // Создаём Span через Tracer
            using var span = tracer.StartActiveSpan("OrderService.GetOrder");

            try
            {
                var client = _httpClientFactory.CreateClient();
                var response = await client.GetAsync("http://service-b:8080/Calculate");

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    return Ok(new { OrderId = 123, Status = "Completed", CalculationResult = result });
                }
                else
                {
                    span?.SetStatus(Status.Error.WithDescription("Calculation service returned error"));
                    return StatusCode(500, "Calculation service error");
                }
            }
            catch (Exception ex)
            {
                span?.SetStatus(Status.Error.WithDescription(ex.Message));
                return StatusCode(500, ex.Message);
            }
            finally
            {
                span?.Dispose();
            }
        }
    }
}