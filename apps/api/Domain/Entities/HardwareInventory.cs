using Domain.Primitives;

namespace Domain.Entities;

public sealed class HardwareInventory : Entity
{
    private readonly List<AssetConfiguration> _assetConfigurations = new();

    public string AssetName { get; private set; } = string.Empty;
    public decimal WeightKg { get; private set; }
    public string DeviceCategory { get; private set; } = "Desktop PC";
    public string? PerformanceTier { get; private set; }

    public IReadOnlyCollection<AssetConfiguration> AssetConfigurations => _assetConfigurations.AsReadOnly();

    public HardwareInventory(string assetName, decimal weightKg, string deviceCategory = "Desktop PC")
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(assetName);
        if (weightKg <= 0) throw new ArgumentOutOfRangeException(nameof(weightKg), "Weight must be greater than zero.");

        AssetName = assetName;
        WeightKg = weightKg;
        DeviceCategory = NormalizeDeviceCategory(deviceCategory);
    }

    public void AddConfiguration(Product product, int quantity, decimal? standardValue, string? location)
    {
        ArgumentNullException.ThrowIfNull(product);
        if (quantity < 1) throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be at least 1.");

        _assetConfigurations.Add(new AssetConfiguration(Id, product.Id, quantity, standardValue, location));
        UpdatePerformanceTier();
    }

    public void UpdateBasicSpecs(string assetName, decimal weightKg, string deviceCategory = "Desktop PC")
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(assetName);
        if (weightKg <= 0) throw new ArgumentOutOfRangeException(nameof(weightKg), "Weight must be greater than zero.");

        AssetName = assetName;
        WeightKg = weightKg;
        DeviceCategory = NormalizeDeviceCategory(deviceCategory);
    }

    private static string NormalizeDeviceCategory(string? deviceCategory)
    {
        return string.IsNullOrWhiteSpace(deviceCategory)
            ? "Desktop PC"
            : deviceCategory.Trim();
    }

    private void UpdatePerformanceTier()
    {
        var ramValue = _assetConfigurations
            .FirstOrDefault(c => c.Product?.Category?.Name == "RAM")?.StandardValue ?? 0;

        var cpuModel = _assetConfigurations
            .FirstOrDefault(c => c.Product?.Category?.Name == "CPU")?.Product?.ModelName ?? string.Empty;

        PerformanceTier = (ramValue, cpuModel) switch
        {
            _ when ramValue >= 32 || cpuModel.Contains("i7") || cpuModel.Contains("Extreme") => "Workstation",
            _ when ramValue >= 16 || _assetConfigurations.Any(c => c.Product?.ModelName?.Contains("1080") == true) => "High-End",
            _ when ramValue >= 8 => "Mid-Range",
            _ => "Entry"
        };
    }
}
