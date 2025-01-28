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
            Console.WriteLine(body);

            var response = new {
                message = "Hello World",
                status = "OK"
            };

            return Ok(response);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateWallet([FromBody] CreateWalletDto createWalletDto){
            string tag = "[WalletController][CreateWallet]";
            _logger.LogInformation($"{tag} Wallet is being created ...");
            try{
                var createdWallet = await _walletServices.AddWalletAsync(createWalletDto);
                return StatusCode(201, createdWallet);
            }catch(Exception e){
                Console.WriteLine(tag+e.Message);
                return StatusCode(500);
            }
        }


        [HttpGet]
        public async Task<IActionResult> CreateWallet(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10
        ){
            string tag = "[WalletController][CreateWallet]";
            _logger.LogInformation($"{tag} Wallets are being fetched ...");
            try{
                var wallets = await _walletServices.GetAllWalletsAsync(pageNumber, pageSize);
                return StatusCode(201, wallets);
            }catch(Exception e){
                Console.WriteLine(tag+e.Message);
                return StatusCode(500);
            }
        }

    }

}