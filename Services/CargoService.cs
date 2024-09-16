using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using UnitedAirlinesAPI.Infrastructure;
using UnitedAirlinesAPI.Models;

namespace UnitedAirlinesAPI.Services;

public class CargoService : ICargoService
{
    private readonly APIConfig _apiConfig;
    private readonly IHttpClientFactory _httpClientFactory;

    public CargoService(IOptions<APIConfig> apiConfig, IHttpClientFactory httpClientFactory )
    {
        _apiConfig = apiConfig.Value;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<CargoResponse> GetCargoManifestByAirportCodeFlightAndDate(CargoRequest request)
    {
        string requestParams = ""; // JsonConvert.SerializeObject(request);
        string content = await GetCargoManifestFromRepo(requestParams, _apiConfig.MockUrl, _apiConfig.APIKey);
        var cargoResponse = JsonConvert.DeserializeObject<CargoResponse>(content);
        return cargoResponse!;
    }

    private async Task<string> GetCargoManifestFromRepo(string requestParams, string url, string apiKey)
    {
        string result = string.Empty;
        try
        {
            HttpResponseMessage response;

            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Add("x-api-key", apiKey);
            response = await client.GetAsync(url);

            result = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();
            return result;
        }
        catch (Exception ex)
        {
            throw new Exception(result, ex);
        }
    }
}
