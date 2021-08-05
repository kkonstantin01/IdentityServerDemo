using Microsoft.AspNetCore.Mvc;
using RestSharp;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WebApiClient.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MainController : ControllerBase
    {
        private const string WebApiGet = "https://localhost:44326/api/Customers";
        private const string IdentityServer = "https://localhost:5001/";

        private readonly IHttpClientFactory _httpClientFactory;

        public MainController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public async Task<IActionResult> Get() 
        {
            // Get Test
            string content = await GetTest(WebApiGet);

            return Ok(content);
        }

        private async Task<string> GetTest(string url)
        {
            // RestSharp
            IRestClient apiClient = new RestClient();
            IRestRequest restRequest = new RestRequest(url);

            string accessToken = await GetAccessToken();
            restRequest.AddHeader("authorization", $"Bearer {accessToken}");

            var result = await apiClient.GetAsync<string>(restRequest);

            return result;
        }

        private async Task<string> GetAccessToken()
        {
            // oauthClient
            // RestSharp
            var tokenClient = new RestClient($"{IdentityServer}connect/token");

            IRestRequest tokenRequest = new RestRequest(Method.POST);
            tokenRequest.AddHeader("content-type", "application/x-www-form-urlencoded");
            var grantType = "client_credentials";

            tokenRequest.AddParameter("client_id", "oauthClient", ParameterType.GetOrPost);
            tokenRequest.AddParameter("grant_type", grantType, ParameterType.GetOrPost);
            tokenRequest.AddParameter("scope", "api1.read", ParameterType.GetOrPost);
            tokenRequest.AddParameter("client_secret", "secret", ParameterType.GetOrPost);

            IRestResponse response = await tokenClient.ExecuteAsync(tokenRequest);

            var tknResponse = JsonSerializer.Deserialize<TknResponse>(response.Content);

            return tknResponse.AccessToken;
        }

        public class TknResponse
        {
            [JsonPropertyName("access_token")]
            public string AccessToken { get; set; }

            [JsonPropertyName("expires_in")]
            public int ExpiresIn { get; set; }

            [JsonPropertyName("token_type")]
            public string TokenType { get; set; }

            [JsonPropertyName("scope")]
            public string Scope { get; set; }
        }
    }
}
