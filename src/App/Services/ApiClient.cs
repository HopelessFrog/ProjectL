using System.Net.Http.Json;
using System.Text.Json;

namespace App.Services;

public class ApiClient
{
    private readonly HttpClient _httpClient;

    public ApiClient(string baseUrl)
    {
        _httpClient = new HttpClient { BaseAddress = new Uri(baseUrl) };
    }

    public async Task<ApiResponse> GetAsync(string endpoint)
    {
        var response = await _httpClient.GetAsync(endpoint);
        var content = await response.Content.ReadAsStringAsync();
        return new ApiResponse(response.StatusCode, content);
    }

    public async Task<ApiResponse> PostAsync<T>(string endpoint, T data)
    {
        var response = await _httpClient.PostAsJsonAsync(endpoint, data);
        var content = await response.Content.ReadAsStringAsync();
        return new ApiResponse(response.StatusCode, content);
    }

    public async Task<ApiResponse> PatchAsync<T>(string endpoint, T data)
    {
        var response = await _httpClient.PatchAsJsonAsync(endpoint, data);
        var content = await response.Content.ReadAsStringAsync();
        return new ApiResponse(response.StatusCode, content);
    }

    public async Task<ApiResponse> DeleteAsync(string endpoint)
    {
        var response = await _httpClient.DeleteAsync(endpoint);
        var content = await response.Content.ReadAsStringAsync();
        return new ApiResponse(response.StatusCode, content);
    }

    public Guid? ExtractIdFromResponse(string jsonResponse, string propertyName = "id")
    {
        try
        {
            var element = JsonSerializer.Deserialize<JsonElement>(jsonResponse);
            if (element.TryGetProperty(propertyName, out var idProp))
            {
                return Guid.Parse(idProp.GetString()!);
            }
        }
        catch
        {
            // Ignore
        }
        return null;
    }
}

public record ApiResponse(System.Net.HttpStatusCode StatusCode, string Content)
{
    public bool IsSuccess => (int)StatusCode >= 200 && (int)StatusCode < 300;
}
