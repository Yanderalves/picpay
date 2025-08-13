using Newtonsoft.Json;
using PicpaySimplificado.DTO;

namespace PicpaySimplificado.Api;

public class AuthorizationClient
{
    private readonly HttpClient _client;
    public AuthorizationClient(HttpClient client) => _client = client;
    
    public async Task<AuthorizationResponse> AuthorizeTransferAsync()
    {
        _client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");
        var response = await _client.GetAsync("https://util.devi.tools/api/v2/authorize");
        return JsonConvert.DeserializeObject<AuthorizationResponse>(response.Content.ReadAsStringAsync().Result);
    }
}