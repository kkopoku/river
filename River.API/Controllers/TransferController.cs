using Microsoft.AspNetCore.Mvc;
using River.API.DTOs;
using River.API.DTOs.Transfer;
using River.API.Services;
using MongoDB.Bson;


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
    [Route("reverse")]
    public async Task<IActionResult> ReverseTransferAsync([FromBody] ReverseTransferDto reverseTransferDto)
    {
        string tag = "[TransferController.cs][ReverseTransferAsync]";
        try
        {
            var response = await _transferService.ReverseTransferAsync(reverseTransferDto);
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



    [HttpGet]
    [Route("status/{id}")]
    public async Task<IActionResult> CheckTransferStatus(string id)
    {
        string tag = "[TransferController.cs][CheckTransferStatus]";

        try
        {
            var response = await _transferService.GetTransferAsync(id);
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

}