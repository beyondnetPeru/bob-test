using Domain.Entities;
using FluentAssertions;
using NUnit.Framework;

namespace Api.Tests.Domain.Entities;

[TestFixture]
public class ProductCategoryTests
{
    [Test]
    public void Constructor_WithValidName_ShouldCreateCategory()
    {
        // Act
        var category = new ProductCategory("RAM");

        // Assert
        category.Name.Should().Be("RAM");
        category.Products.Should().BeEmpty();
    }

    [Test]
    public void Constructor_WithEmptyName_ShouldThrowArgumentException()
    {
        // Act
        Action act = () => new ProductCategory("");

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Test]
    public void Constructor_WithNullName_ShouldThrowArgumentException()
    {
        // Act
        Action act = () => new ProductCategory(null!);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Test]
    public void Constructor_WithWhitespaceName_ShouldThrowArgumentException()
    {
        // Act
        Action act = () => new ProductCategory("   ");

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Test]
    public void Update_WithValidName_ShouldUpdateName()
    {
        // Arrange
        var category = new ProductCategory("CPU");

        // Act
        category.Update("GPU");

        // Assert
        category.Name.Should().Be("GPU");
    }

    [Test]
    public void Update_WithEmptyName_ShouldThrowArgumentException()
    {
        // Arrange
        var category = new ProductCategory("CPU");

        // Act
        Action act = () => category.Update("");

        // Assert
        act.Should().Throw<ArgumentException>();
    }
}
