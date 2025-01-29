using Microsoft.AspNetCore.Mvc;
using River.API.DTOs;
using River.API.Services;


namespace River.API.Controllers;


[ApiController]
[Route("api/[controller]")]
public class TransferController(
    ITransferService transferService,
    ILogger<TransferController> logger
) : ControllerBase
{

    private readonly ITransferService _transferService = transferService;
    private readonly ILogger<TransferController> _logger = logger;


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
            return StatusCode(200, response);
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