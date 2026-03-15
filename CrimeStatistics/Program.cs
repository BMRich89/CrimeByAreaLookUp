
using System.Text.Json;
using CrimeStatistics.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddHttpClient();
builder.Services.AddOptions<AppSettings>()
    .Bind(builder.Configuration.GetSection("ConnectionStrings"))
    .ValidateDataAnnotations();
builder.Services.AddSingleton<CrimeService>();
builder.Services.AddSingleton<PostCodeService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/postcode", (string postcode) =>
{
    var postCodeService = app.Services.GetRequiredService<PostCodeService>();
    var result = postCodeService.GetPostCodeCoordinates(postcode);
    return result;
})
.WithName("");

app.MapGet("/CrimeByPostcode", async (string postcode) =>
{
    //TODO: Create validation service
    var isPostcodeValid = !string.IsNullOrWhiteSpace(postcode) && postcode.Length >= 5 && postcode.Length <= 10;
    if (!isPostcodeValid)
    {
        //TODO
    }
    var postCodeService = app.Services.GetRequiredService<PostCodeService>();
    Tuple<string, string> LatLong = await postCodeService.GetPostCodeCoordinates(postcode);

    var crimeService = app.Services.GetRequiredService<CrimeService>();
    var crimeJson = await crimeService.FetchCrimeDataByCoordinates(LatLong.Item1, LatLong.Item2);

    if (!double.TryParse(LatLong.Item1, out var centerLat) || !double.TryParse(LatLong.Item2, out var centerLng))
    {
        return Results.Problem("Invalid coordinates returned for postcode.");
    }

    JsonElement crimes;
    try
    {
        crimes = JsonSerializer.Deserialize<JsonElement>(crimeJson);
    }
    catch (JsonException)
    {
        return Results.Problem("Failed to parse crime data from upstream API.");
    }

    var response = new CrimeSearchResponse
    {
        SearchCenterLat = centerLat,
        SearchCenterLng = centerLng,
        Crimes = crimes
    };

    return Results.Ok(response);
})
.WithName("CrimeByPostcode");


app.MapGet("/CrimeByCategoryByPostcode", async (string postcode) =>
{
    //TODO: Create validation service
    var isPostcodeValid = !string.IsNullOrWhiteSpace(postcode) && postcode.Length >= 5 && postcode.Length <= 10;
    if (!isPostcodeValid)
    {
        //TODO
    }
    var postCodeService = app.Services.GetRequiredService<PostCodeService>();
    Tuple<string, string> LatLong = await postCodeService.GetPostCodeCoordinates(postcode);

    var crimeService = app.Services.GetRequiredService<CrimeService>();
    var result = await crimeService.FetchCrimeByCategoryByPostcode(LatLong.Item1, LatLong.Item2);

    return result;
})
.WithName("CrimeByCategoryByPostcode");

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
