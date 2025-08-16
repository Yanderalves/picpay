using Newtonsoft.Json;
using Picpay.DTO;

namespace Picpay.Api;

public class AuthorizationClient
{
    private readonly HttpClient _client;
    public AuthorizationClient(HttpClient client) => _client = client;
    
    public async Task<AuthorizationResponseDTO> AuthorizeTransferAsync()
    {
        _client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");
        var response = await _client.GetAsync("https://util.devi.tools/api/v2/authorize");
        var content = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<AuthorizationResponseDTO>(content);
    }
}