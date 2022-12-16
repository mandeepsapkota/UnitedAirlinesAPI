using Microsoft.AspNetCore.Mvc;
using UnitedAirlinesAPI.Models;
using UnitedAirlinesAPI.Services;

namespace UnitedAirlinesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CargoController : Controller
    {
        private readonly ICargoService _cargoService;

        public CargoController(ICargoService cargoService)
        {
            _cargoService = cargoService;
        }

        /// <summary>
        /// Gets the cargo manifest information by airport station, flight and date
        /// </summary>
        /// <param name="request">Airport station, flight and date</param>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST api/cargo/manifest
        ///     {
        ///        "airportcode": PDX,
        ///        "flightdate": "AS-7095-01Jan2023",
        ///     }
        ///
        /// </remarks>
        /// <response code="200">Returns the cargo manifest</response>
        /// <response code="400">If the request parameters is invalid</response>
        /// <response code="404">No data found</response>
        [HttpPost]
        [Route("/manifest")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCargoInfoAsync([FromBody] CargoRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var result = await _cargoService.GetCargoManifestByAirportCodeFlightAndDate(request);
                if (result != null)
                {
                    return Ok(result);
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
