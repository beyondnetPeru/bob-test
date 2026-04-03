using Domain.Primitives;

namespace Domain.Entities;

public sealed class Manufacturer : Entity
{
    public string Name { get; private set; } = string.Empty;

    public ICollection<Product> Products { get; private set; } = new List<Product>();

    public Manufacturer(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Name = name;
    }

    public void Update(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Name = name;
    }
}
