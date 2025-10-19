using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Newtonsoft.Json;
using Stage_0_Task.Models;

namespace Stage_0_Task.Controllers
{
    [Route("api/me")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClient;
        private readonly APIResponse _apiResponse;
        private readonly string _catFactURL; 

        public ProfileController(IHttpClientFactory httpClient)
        {
            _httpClient = httpClient;
            _apiResponse = new();
            _catFactURL = "https://catfact.ninja/fact";
        }

        [HttpGet]
        [EnableRateLimiting("Fixed")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> GetProfile()
        {
            var client = _httpClient.CreateClient("ProfileAPI");
            HttpRequestMessage message = new HttpRequestMessage();
            message.Headers.Add("Accept", "application/json");
            message.RequestUri = new Uri(_catFactURL);
            message.Method = HttpMethod.Get;

            HttpResponseMessage response = null;
            response = await client.SendAsync(message);

            if (response.IsSuccessStatusCode == false)
            {
                throw new InvalidOperationException();
            }

            var result = await response.Content.ReadAsStringAsync();
            string catFact = JsonConvert.DeserializeObject<CatFactAPIResponse>(result).Fact;

            _apiResponse.Status = "success";
            _apiResponse.User = new UserModel()
            {
                Email = "ikechukwugodwin22@gmail.com",
                Name = "Ikechukwu F. Godwin",
                Stack = "C#/ASP.NET Web API"
            };
            _apiResponse.TimeStamp = DateTime.UtcNow;
            _apiResponse.Fact = catFact;
            return Ok(_apiResponse);
        }

    }
}
