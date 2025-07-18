using System;
using System.Configuration;

public class AppSettings
{
    private IConfigurationRoot ConfigRoot;

    public string? ApiKey { get; set; }
    public string? BaseUrl { get; set; }
    public string? PostCodeUrl readonly = Con;
    public string? CrimeUrl { get; set; }
}

