
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Runtime;
using Microsoft.Extensions.Options;
using System.Text.Json;
using CrimeStatistics.PostCodeService;
using Microsoft.AspNetCore.Mvc.Infrastructure;

internal class PostCodeService
{
    private readonly AppSettings _settings;
    private readonly IHttpClientFactory _httpClientFactory;

    public PostCodeService(IHttpClientFactory httpClientFactory, IOptions<AppSettings> options)
    {
        _httpClientFactory = httpClientFactory;
        _settings = options.Value;
    }


    public async Task<Tuple<string, string>> GetPostCodeCoordinates(string postcode)
    {
        var client = _httpClientFactory.CreateClient();

        // Build URL (e.g., https://example.com/crime?lat=...&long=...)
        var url = $"{_settings.PostCodeAPIUrl}/postcodes/{postcode}";

        try
        {
            // Make GET request
            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            // Read as string (alternatively, use response.Content.ReadFromJsonAsync<T>())
            var result = await response.Content.ReadAsStringAsync();
            var resultObj = JsonSerializer.Deserialize<Root>(result);
            return Tuple.Create(resultObj.result.latitude.ToString(),resultObj.result.longitude.ToString());
        }
        catch (HttpRequestException ex)
        {
            // Handle error
            // Log exception here if needed
            //return $"Error fetching crime data: {ex.Message}";
        }
        return Tuple.Create("50.736", "-1.883"); //TODO: Create Lat Long class
    }
}
