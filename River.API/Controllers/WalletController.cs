using Microsoft.AspNetCore.Mvc;
using River.API.DTOs;
using River.API.Services;

namespace River.API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class WalletController(
        IWalletServices walletServices,
        ILogger<WalletController> logger
    ): ControllerBase
    {

        private readonly IWalletServices _walletServices = walletServices;
        private readonly ILogger<WalletController> _logger = logger;

        [HttpGet("test")]
        public IActionResult TestRoute([FromBody] object body)
        {
            _logger.LogInformation(body.ToString());
            var response = new ApiResponse<string>(
                code: "200",
                message: "Test route successful",
                data: "Hello from River.API"
            );
            return Ok(response);
        }


        [HttpPost("create")]
        public async Task<IActionResult> CreateWallet([FromBody] CreateWalletDto createWalletDto){
            string tag = "[WalletController][CreateWallet]";
            _logger.LogInformation($"{tag} Wallet is being created ...");
            try{
                var response = await _walletServices.AddWalletAsync(createWalletDto);
                return StatusCode(201, response);
            }catch(Exception e){
                Console.WriteLine(tag+e.Message);
                var response = new ApiResponse<string>("500", e.Message);
                return StatusCode(500, response);
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetAllWallets(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10
        ){
            string tag = "[WalletController][GetAllWallets]";
            _logger.LogInformation($"{tag} Wallets are being fetched ...");
            try{
                var response = await _walletServices.GetAllWalletsAsync(pageNumber, pageSize);
                return StatusCode(201, response);
            }catch(Exception e){
                _logger.LogInformation(tag+e.Message);
                var response = new ApiResponse<string>("500", e.Message);
                return StatusCode(500, response);
            }
        }

    }

}