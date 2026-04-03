using Domain.Entities;
using FluentAssertions;
using NUnit.Framework;

namespace Api.Tests.Domain.Entities;

[TestFixture]
public class HardwareInventoryTests
{
    [Test]
    public void Constructor_WithValidData_ShouldCreateHardware()
    {
        // Act
        var hardware = new HardwareInventory("Server1", 10.5m);

        // Assert
        hardware.AssetName.Should().Be("Server1");
        hardware.WeightKg.Should().Be(10.5m);
        hardware.AssetConfigurations.Should().BeEmpty();
    }

    [Test]
    public void Constructor_WithEmptyName_ShouldThrowArgumentException()
    {
        // Act
        Action act = () => new HardwareInventory("", 10.5m);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Test]
    public void Constructor_WithZeroOrNegativeWeight_ShouldThrowArgumentOutOfRangeException()
    {
        // Act
        Action act = () => new HardwareInventory("Server1", 0m);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Test]
    public void UpdateBasicSpecs_WithValidData_ShouldUpdate()
    {
        // Arrange
        var hardware = new HardwareInventory("Server1", 10m);

        // Act
        hardware.UpdateBasicSpecs("Server2", 15m);

        // Assert
        hardware.AssetName.Should().Be("Server2");
        hardware.WeightKg.Should().Be(15m);
    }
}
