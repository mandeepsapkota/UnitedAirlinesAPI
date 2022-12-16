using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using UnitedAirlinesAPI.Infrastructure;
using UnitedAirlinesAPI.Models;

namespace UnitedAirlinesAPI.Services
{
    public class CargoService : ICargoService
    {
        private readonly SettingConfig _settingConfig;

        public CargoService(IOptions<SettingConfig> settingConfig)
        {
            _settingConfig = settingConfig.Value;
        }

        public async Task<CargoResponse> GetCargoManifestByAirportCodeFlightAndDate(CargoRequest request)
        {
            string requestParams = ""; // JsonConvert.SerializeObject(request);
            string content = await GetCargoManifestFromRepo(requestParams, _settingConfig.MockUrl, _settingConfig.APIKey);
            var cargoResponse = JsonConvert.DeserializeObject<CargoResponse>(content);
            return cargoResponse;
        }

        private async Task<string> GetCargoManifestFromRepo(string requestParams, string url, string apiKey)
        {
            string result = string.Empty;
            try
            {
                HttpResponseMessage response;

                var _uri = new Uri(url);
                var _method = new HttpMethod("GET");
                var _httpClient = new HttpClient();
                _httpClient.DefaultRequestHeaders.Add("x-api-key", apiKey);
                response = await _httpClient.GetAsync(_uri);

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
}
