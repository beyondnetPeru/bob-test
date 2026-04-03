using Domain.Entities;
using FluentAssertions;
using NUnit.Framework;

namespace Api.Tests.Domain.Entities;

[TestFixture]
public class ManufacturerTests
{
    [Test]
    public void Constructor_WithValidName_ShouldCreateManufacturer()
    {
        // Act
        var manufacturer = new Manufacturer("Intel");

        // Assert
        manufacturer.Name.Should().Be("Intel");
        manufacturer.Products.Should().BeEmpty();
    }

    [Test]
    public void Constructor_WithEmptyName_ShouldThrowArgumentException()
    {
        // Act
        Action act = () => new Manufacturer("");

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Test]
    public void Constructor_WithNullName_ShouldThrowArgumentException()
    {
        // Act
        Action act = () => new Manufacturer(null!);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Test]
    public void Constructor_WithWhitespaceName_ShouldThrowArgumentException()
    {
        // Act
        Action act = () => new Manufacturer("   ");

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Test]
    public void Update_WithValidName_ShouldUpdateName()
    {
        // Arrange
        var manufacturer = new Manufacturer("Intel");

        // Act
        manufacturer.Update("AMD");

        // Assert
        manufacturer.Name.Should().Be("AMD");
    }

    [Test]
    public void Update_WithEmptyName_ShouldThrowArgumentException()
    {
        // Arrange
        var manufacturer = new Manufacturer("Intel");

        // Act
        Action act = () => manufacturer.Update("");

        // Assert
        act.Should().Throw<ArgumentException>();
    }
}
