using Application.Abstractions.Repositories;
using Application.Common.Models;
using Application.Features.Products.Commands.UpdateProduct;
using Domain.Entities;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Api.Tests.Application.Features.Products.Commands.UpdateProduct;

[TestFixture]
public class UpdateProductHandlerTests
{
    private Mock<IProductRepository> _productRepositoryMock;
    private Mock<ICategoryRepository> _categoryRepositoryMock;
    private Mock<IManufacturerRepository> _manufacturerRepositoryMock;
    private Mock<IUnitOfWork> _unitOfWorkMock;
    private Mock<ILogger<UpdateProductHandler>> _loggerMock;
    private UpdateProductHandler _handler;

    [SetUp]
    public void Setup()
    {
        _productRepositoryMock = new Mock<IProductRepository>();
        _categoryRepositoryMock = new Mock<ICategoryRepository>();
        _manufacturerRepositoryMock = new Mock<IManufacturerRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<UpdateProductHandler>>();

        _handler = new UpdateProductHandler(
            _productRepositoryMock.Object,
            _categoryRepositoryMock.Object,
            _manufacturerRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }

    [Test]
    public async Task Handle_WithValidRequest_ShouldUpdateProductAndReturnSuccess()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var manufacturerId = Guid.NewGuid();
        var command = new UpdateProductCommand(productId, categoryId, manufacturerId, "NewModel", "NewDesc");

        var existingProduct = new Product(Guid.NewGuid(), Guid.NewGuid(), "OldModel", "OldDesc");

        _productRepositoryMock
            .Setup(repo => repo.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingProduct);
            
        _categoryRepositoryMock
            .Setup(repo => repo.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ProductCategory("Category1")); 
            
        _manufacturerRepositoryMock
            .Setup(repo => repo.GetByIdAsync(manufacturerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Manufacturer("Manufacturer1")); 

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        
        _productRepositoryMock.Verify(r => r.Update(existingProduct), Times.Once);
        _unitOfWorkMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task Handle_WithInvalidProduct_ShouldReturnFailure()
    {
        // Arrange
        var command = new UpdateProductCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "NewModel", "NewDesc");

        _productRepositoryMock
            .Setup(repo => repo.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("Error.NotFound");
        
        _productRepositoryMock.Verify(r => r.Update(It.IsAny<Product>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
