
using Microsoft.Extensions.Options;
using System.Net.Http;

//TODO: add abstract for API Services i.e postcode and crime
internal class CrimeService
{
    private readonly AppSettings _settings;
    private readonly IHttpClientFactory _httpClientFactory;
    public CrimeService(IHttpClientFactory httpClientFactory, IOptions<AppSettings> options)
    {
        _httpClientFactory = httpClientFactory;
        _settings = options.Value;
    }

    internal async Task<string> FetchCrimeDataByCoordinates(string lat, string @long)
    {
        var client = _httpClientFactory.CreateClient();

        // Build URL (e.g., https://example.com/crime?lat=...&long=...)
        var url = $"{_settings.CrimeAPIUrl}/crimes-street/all-crime?lat={lat}&lng={@long}";

        try
        {
            // Make GET request
            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            // Read as string (alternatively, use response.Content.ReadFromJsonAsync<T>())
            var result = await response.Content.ReadAsStringAsync();
            return result;
        }
        catch (HttpRequestException ex)
        {
            // Handle error
            // Log exception here if needed
            return $"Error fetching crime data: {ex.Message}";
        }
    }
}