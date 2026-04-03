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
        var tableMatch = Regex.Match(markdown, @"\| Memory \| Storage \| Ports \| GPU \| Weight \| PSU \| CPU \|\r?\n\| :----- \| :--------- \| :------------------------------------- \| :---------------------- \| :------ \| :--------- \| :-------------------------------------------- \|\r?\n([\s\S]+?)\r?\n\r?\n", RegexOptions.Multiline);

        if (!tableMatch.Success) 
        {
            logger.LogWarning("Markdown table not found in seed file.");
            return;
        }

        var rows = tableMatch.Groups[1].Value.Split('\n', StringSplitOptions.RemoveEmptyEntries);

        foreach (var row in rows)
        {
            var cols = row.Split('|').Select(c => c.Trim()).ToArray();
            if (cols.Length < 8) continue;

            // Mapping: | 1:Memory | 2:Storage | 3:Ports | 4:GPU | 5:Weight | 6:PSU | 7:CPU |
            var rawMemory = cols[1];
            var rawStorage = cols[2];
            var rawPorts = cols[3];
            var rawGpu = cols[4];
            var rawWeight = cols[5];
            var rawPsu = cols[6];
            var rawCpu = cols[7];

            // 1. Create the Machine
            var weightKg = cleansingService.ParseWeight(rawWeight);
            var machine = new HardwareInventory($"Machine-{Guid.NewGuid().ToString()[..8]}", weightKg);

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
            var ramProduct = await GetOrCreateProduct(ramCatId, genericId, "Standard Memory");
            machine.AddConfiguration(ramProduct, 1, ramCapacity, "DIMM Slot");

            // 5. Add Storage
            var storageCapacity = cleansingService.ParseCapacity(rawStorage);
            var storageType = cleansingService.NormalizeStorageType(rawStorage);
            var storageProduct = await GetOrCreateProduct(storageCatId, genericId, $"{storageType} Drive");
            machine.AddConfiguration(storageProduct, 1, storageCapacity, "SATA/NVMe");

            // 6. Add PSU
            var psuWatts = cleansingService.ParseCapacity(rawPsu); 
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
