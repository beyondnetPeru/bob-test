using System.Text.RegularExpressions;
using Application.Abstractions.Services;

namespace Infrastructure.Services;

public sealed class CleansingService : ICleansingService
{
    public decimal ParseWeight(string rawWeight)
    {
        // Extract number and unit using Regex
        var match = Regex.Match(rawWeight, @"(\d+(\.\d+)?)\s*(kg|lb)", RegexOptions.IgnoreCase);
        if (!match.Success) return 0;

        var value = decimal.Parse(match.Groups[1].Value);
        var unit = match.Groups[3].Value.ToLower();

        // Convert lb to kg (1 lb = 0.453592 kg)
        return unit == "lb" ? Math.Round(value * 0.453592m, 2) : value;
    }

    public decimal ParseCapacity(string rawCapacity)
    {
        // Extract number and unit (GB, TB, MB)
        var match = Regex.Match(rawCapacity, @"(\d+)\s*(GB|TB|MB|SSD|HDD|SDD)", RegexOptions.IgnoreCase);
        if (!match.Success) return 0;

        var value = decimal.Parse(match.Groups[1].Value);
        var unit = match.Groups[2].Value.ToUpper();

        return unit switch
        {
            "TB" => value * 1024,
            "MB" => Math.Round(value / 1024m, 2),
            _ => value
        };
    }

    public string NormalizeStorageType(string rawStorage)
    {
        if (rawStorage.Contains("SSD", StringComparison.OrdinalIgnoreCase) || 
            rawStorage.Contains("SDD", StringComparison.OrdinalIgnoreCase))
            return "SSD";
        
        return "HDD";
    }
}
