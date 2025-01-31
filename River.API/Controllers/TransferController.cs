using Microsoft.AspNetCore.Mvc;
using River.API.DTOs;
using River.API.Services;
using System.Text.Json;


namespace River.API.Controllers;


[ApiController]
[Route("api/[controller]")]
public class TransferController(
    ITransferService transferService,
    ILogger<TransferController> logger,
    IKafkaProducer kafkaProducer
) : ControllerBase
{

    private readonly ITransferService _transferService = transferService;
    private readonly ILogger<TransferController> _logger = logger;
    private readonly IKafkaProducer _kafkaProducer = kafkaProducer;


    [HttpGet]
    public async Task<IActionResult> GetAllTransfersAsync(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10
    )
    {
        string tag = "[TransferController][GetAllTransfersAsync]";
        _logger.LogInformation($"{tag} Transfers are being fetched...");
        try
        {
            var response = await _transferService.GetAllTransfersAsync(pageNumber, pageSize);
            var code = int.Parse(response.Code);
            return StatusCode(code, response);
        }
        catch (Exception e)
        {
            _logger.LogInformation(tag + e.Message);
            var response = new ApiResponse<string>("500", e.Message);
            return StatusCode(500, response);
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateTransferAsync([FromBody] CreateTransferDto createTransferDto)
    {
        string tag = "[TransferController][CreateTransferAsync]";
        try
        {
            var response = await _transferService.CreateTransferAsync(createTransferDto);
            var code = int.Parse(response.Code);
            return StatusCode(code, response);
        }
        catch (Exception e)
        {
            _logger.LogInformation(tag + e.Message);
            var response = new ApiResponse<string>("500", e.Message);
            return StatusCode(500, response);
        }
    }


    [HttpPost]
    [Route("Kafka")]
    public async Task<IActionResult> CreateTransferFromKafkaAsync([FromBody] object body)
    {
        string tag = "[TransferController][CreateTransferFromKafkaAsync]";
        _logger.LogInformation(tag + $"Here is the body {body}");
        try
        {
            var jsonBody = JsonSerializer.Deserialize<JsonElement>(body.ToString() ?? "");
            _logger.LogInformation($"{tag} JsonBody: {jsonBody.GetProperty("name").GetString()}");
            await _kafkaProducer.ProduceAsync(
                        "river_transactions",
                        "10",
                        jsonBody.GetProperty("name").GetString() ?? ""
                    );
            var response = new ApiResponse<object>("200", "success", body);
            return StatusCode(200, response);
        }
        catch (Exception e)
        {
            _logger.LogInformation(tag + e.Message);
            var response = new ApiResponse<string>("500", e.Message);
            return StatusCode(500, response);
        }
    }

}