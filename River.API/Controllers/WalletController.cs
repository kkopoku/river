using Microsoft.AspNetCore.Mvc;

namespace River.API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]

    public class WalletController : ControllerBase
    {

        [HttpGet("test")]
        public IActionResult TestRoute()
        {
            var response = new {
                message = "Hello World",
                status = "OK"
            };

            return Ok(response);
        }

    }

}