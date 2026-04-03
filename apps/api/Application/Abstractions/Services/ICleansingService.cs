namespace Application.Abstractions.Services;

public interface ICleansingService
{
    decimal ParseWeight(string rawWeight);
    decimal ParseCapacity(string rawCapacity);
    string NormalizeStorageType(string rawStorage);
}
