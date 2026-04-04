using Application.Abstractions.Services;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace Infrastructure.Data;

public sealed class DataSeeder(
    AppDbContext context, 
    ICleansingService cleansingService,
    ILogger<DataSeeder> logger)
{
    public async Task SeedAsync(string filePath)
    {
        if (await context.HardwareInventories.AnyAsync()) return;

        logger.LogInformation("Starting database seeding from {FilePath}", filePath);

        // Ensure Categories and Manufacturers exist
        var cpuCatId = await GetOrCreateCategory("CPU");
        var gpuCatId = await GetOrCreateCategory("GPU");
        var ramCatId = await GetOrCreateCategory("RAM");
        var storageCatId = await GetOrCreateCategory("STORAGE");
        var psuCatId = await GetOrCreateCategory("PSU");
        var portCatId = await GetOrCreateCategory("PORT");

        var intelId = await GetOrCreateManufacturer("Intel");
        var amdId = await GetOrCreateManufacturer("AMD");
        var nvidiaId = await GetOrCreateManufacturer("NVIDIA");
        var genericId = await GetOrCreateManufacturer("Generic");

        var markdown = await File.ReadAllTextAsync(filePath);
        var rows = ExtractHardwareRows(markdown);

        if (rows.Count == 0)
        {
            logger.LogWarning("Markdown table not found in seed file.");
            return;
        }

        logger.LogInformation("Found {Count} raw hardware rows to seed", rows.Count);

        for (var i = 0; i < rows.Count; i++)
        {
            var row = rows[i];

            // Mapping: Memory | Storage | Ports | GPU | Weight | PSU | CPU
            var rawMemory = row[0];
            var rawStorage = row[1];
            var rawPorts = row[2];
            var rawGpu = row[3];
            var rawWeight = row[4];
            var rawPsu = row[5];
            var rawCpu = row[6];

            // 1. Create the Machine
            var weightKg = cleansingService.ParseWeight(rawWeight);
            var deviceCategory = DetermineDeviceCategory(weightKg, rawCpu, rawGpu, rawMemory);
            var machine = new HardwareInventory($"Asset-{i + 1:000}", weightKg, deviceCategory);

            // 2. Add CPU
            var cpuManufacturerId = rawCpu.Contains("Intel", StringComparison.OrdinalIgnoreCase) ? intelId : amdId;
            var cpuProduct = await GetOrCreateProduct(cpuCatId, cpuManufacturerId, rawCpu);
            machine.AddConfiguration(cpuProduct, 1, null, "Socket");

            // 3. Add GPU
            var gpuManufacturerId = rawGpu.Contains("NVIDIA", StringComparison.OrdinalIgnoreCase) ? nvidiaId : 
                                   (rawGpu.Contains("Radeon", StringComparison.OrdinalIgnoreCase) ? amdId : genericId);
            var gpuProduct = await GetOrCreateProduct(gpuCatId, gpuManufacturerId, rawGpu);
            machine.AddConfiguration(gpuProduct, 1, null, "PCIe Slot");

            // 4. Add RAM
            var ramCapacity = cleansingService.ParseCapacity(rawMemory);
            var ramProduct = await GetOrCreateProduct(ramCatId, genericId, NormalizeMemoryLabel(rawMemory));
            machine.AddConfiguration(ramProduct, 1, ramCapacity, "DIMM Slot");

            // 5. Add Storage
            var storageCapacity = cleansingService.ParseCapacity(rawStorage);
            var storageProduct = await GetOrCreateProduct(storageCatId, genericId, NormalizeStorageLabel(rawStorage));
            machine.AddConfiguration(storageProduct, 1, storageCapacity, "Drive Bay");

            // 6. Add PSU
            var psuWatts = ParseNumericValue(rawPsu);
            var psuProduct = await GetOrCreateProduct(psuCatId, genericId, rawPsu);
            machine.AddConfiguration(psuProduct, 1, psuWatts, "Chassis Internal");

            // 7. Add Ports 
            var ports = rawPorts.Split(',').Select(p => p.Trim());
            foreach (var port in ports)
            {
                var portMatch = Regex.Match(port, @"(\d+)\s*x\s*(.*)");
                if (portMatch.Success)
                {
                    var qty = int.Parse(portMatch.Groups[1].Value);
                    var portName = portMatch.Groups[2].Value;
                    var portProduct = await GetOrCreateProduct(portCatId, genericId, portName);
                    machine.AddConfiguration(portProduct, qty, null, "External I/O");
                }
            }

            context.HardwareInventories.Add(machine);
        }

        await context.SaveChangesAsync();
        logger.LogInformation("Successfully seeded database from {FilePath}", filePath);
    }

    private static List<string[]> ExtractHardwareRows(string markdown)
    {
        var lines = markdown
            .Split('\n', StringSplitOptions.None)
            .Select(l => l.Trim())
            .ToList();

        var headerIndex = lines.FindIndex(l =>
            l.StartsWith("|", StringComparison.Ordinal) &&
            l.Contains("Memory", StringComparison.OrdinalIgnoreCase) &&
            l.Contains("Storage", StringComparison.OrdinalIgnoreCase) &&
            l.Contains("CPU", StringComparison.OrdinalIgnoreCase));

        if (headerIndex < 0)
            return [];

        var rowList = new List<string[]>();

        for (var i = headerIndex + 1; i < lines.Count; i++)
        {
            var line = lines[i];

            if (string.IsNullOrWhiteSpace(line))
                break;

            if (!line.StartsWith("|", StringComparison.Ordinal))
                break;

            // Skip markdown separator rows like | :--- | :--- |
            if (line.Contains(":---", StringComparison.Ordinal) ||
                line.Contains("---", StringComparison.Ordinal) && !Regex.IsMatch(line, @"\d"))
            {
                continue;
            }

            var cols = line
                .Trim('|')
                .Split('|', StringSplitOptions.None)
                .Select(c => c.Trim())
                .ToArray();

            if (cols.Length == 7)
                rowList.Add(cols);
        }

        return rowList;
    }

    private static decimal? ParseNumericValue(string rawValue)
    {
        var match = Regex.Match(rawValue, @"(\d+(\.\d+)?)");
        if (!match.Success)
            return null;

        return decimal.Parse(match.Groups[1].Value);
    }

    private static string NormalizeMemoryLabel(string rawMemory)
        => Regex.Replace(rawMemory.Trim().ToUpperInvariant(), @"\s+", " ");

    private static string NormalizeStorageLabel(string rawStorage)
        => Regex.Replace(rawStorage.Trim().ToUpperInvariant().Replace("SDD", "SSD"), @"\s+", " ");

    private static string DetermineDeviceCategory(decimal weightKg, string rawCpu, string rawGpu, string rawMemory)
    {
        var memoryValue = ParseNumericValue(rawMemory) ?? 0;

        if (rawCpu.Contains("Extreme", StringComparison.OrdinalIgnoreCase) ||
            rawCpu.Contains("i7", StringComparison.OrdinalIgnoreCase) ||
            rawGpu.Contains("1080", StringComparison.OrdinalIgnoreCase) ||
            memoryValue >= 32)
        {
            return "Workstation";
        }

        if (weightKg <= 6m)
        {
            return "Mini PC";
        }

        return "Desktop PC";
    }

    private async Task<Guid> GetOrCreateCategory(string name)
    {
        var cat = await context.ProductCategories.FirstOrDefaultAsync(c => c.Name == name);
        if (cat == null)
        {
            cat = new ProductCategory(name);
            context.ProductCategories.Add(cat);
            await context.SaveChangesAsync();
        }
        return cat.Id;
    }

    private async Task<Guid> GetOrCreateManufacturer(string name)
    {
        var mnf = await context.Manufacturers.FirstOrDefaultAsync(m => m.Name == name);
        if (mnf == null)
        {
            mnf = new Manufacturer(name);
            context.Manufacturers.Add(mnf);
            await context.SaveChangesAsync();
        }
        return mnf.Id;
    }

    private async Task<Product> GetOrCreateProduct(Guid catId, Guid mnfId, string model)
    {
        var prod = await context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.CategoryId == catId && p.ManufacturerId == mnfId && p.ModelName == model);
        if (prod == null)
        {
            prod = new Product(catId, mnfId, model, "");
            context.Products.Add(prod);
            await context.SaveChangesAsync();
            
            prod = await context.Products
                .Include(p => p.Category)
                .FirstAsync(p => p.Id == prod.Id);
        }
        return prod;
    }
}
