using UnitedAirlinesAPI.Models;

namespace UnitedAirlinesAPI.Services
{
    public interface ICargoService
    {
        Task<CargoResponse> GetCargoManifestByAirportCodeFlightAndDate(CargoRequest request);
    }
}
