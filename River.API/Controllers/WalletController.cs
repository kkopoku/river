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
            _logger.LogInformation($"{tag} Wallet is being created via log");
            try{
                var createdWallet = await _walletServices.AddWalletAsync(createWalletDto);
                return StatusCode(201);
            }catch(Exception e){
                Console.WriteLine(tag+e.Message);
                return StatusCode(500);
            }
        }

    }

}