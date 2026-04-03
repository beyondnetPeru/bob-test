using Domain.Entities;
using FluentAssertions;
using NUnit.Framework;

namespace Api.Tests.Domain.Entities;

[TestFixture]
public class ProductTests
{
    [Test]
    public void Constructor_WithValidData_ShouldCreateProduct()
    {
        // Arrange
        var catId = Guid.NewGuid();
        var manId = Guid.NewGuid();

        // Act
        var product = new Product(catId, manId, "ModelXYZ", "Some desc");

        // Assert
        product.CategoryId.Should().Be(catId);
        product.ManufacturerId.Should().Be(manId);
        product.ModelName.Should().Be("ModelXYZ");
        product.Description.Should().Be("Some desc");
    }

    [Test]
    public void Constructor_WithEmptyCategoryId_ShouldThrowArgumentException()
    {
        // Act
        Action act = () => new Product(Guid.Empty, Guid.NewGuid(), "Model", "Desc");

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage("*CategoryId cannot be empty.*");
    }
    
    [Test]
    public void Update_WithValidData_ShouldUpdateProduct()
    {
        // Arrange
        var product = new Product(Guid.NewGuid(), Guid.NewGuid(), "OldModel", "OldDesc");
        var newCatId = Guid.NewGuid();
        var newManId = Guid.NewGuid();

        // Act
        product.Update(newCatId, newManId, "NewModel", "NewDesc");

        // Assert
        product.ModelName.Should().Be("NewModel");
        product.Description.Should().Be("NewDesc");
        product.CategoryId.Should().Be(newCatId);
    }
}
