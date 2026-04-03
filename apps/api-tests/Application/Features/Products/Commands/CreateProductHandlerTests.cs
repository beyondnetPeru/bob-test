using Application.Abstractions.Repositories;
using Application.Common.Models;
using Application.Features.Products.Commands.CreateProduct;
using Domain.Entities;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace Api.Tests.Application.Features.Products.Commands.CreateProduct;

[TestFixture]
public class CreateProductHandlerTests
{
    private Mock<IProductRepository> _productRepositoryMock;
    private Mock<ICategoryRepository> _categoryRepositoryMock;
    private Mock<IManufacturerRepository> _manufacturerRepositoryMock;
    private Mock<IUnitOfWork> _unitOfWorkMock;
    private Mock<ILogger<CreateProductHandler>> _loggerMock;
    private CreateProductHandler _handler;

    [SetUp]
    public void Setup()
    {
        _productRepositoryMock = new Mock<IProductRepository>();
        _categoryRepositoryMock = new Mock<ICategoryRepository>();
        _manufacturerRepositoryMock = new Mock<IManufacturerRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<CreateProductHandler>>();

        _handler = new CreateProductHandler(
            _productRepositoryMock.Object,
            _categoryRepositoryMock.Object,
            _manufacturerRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }

    [Test]
    public async Task Handle_WithValidRequest_ShouldCreateProductAndReturnSuccess()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var manufacturerId = Guid.NewGuid();
        var command = new CreateProductCommand(categoryId, manufacturerId, "ModelX", "Description");
        
        // Setup existing category and manufacturer
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
        result.Value.Should().NotBeEmpty();
        
        _productRepositoryMock.Verify(r => r.AddAsync(It.Is<Product>(p => p.ModelName == "ModelX"), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task Handle_WithInvalidCategory_ShouldReturnFailure()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var manufacturerId = Guid.NewGuid();
        var command = new CreateProductCommand(categoryId, manufacturerId, "ModelX", "Description");
        
        // Setup missing category
        _categoryRepositoryMock
            .Setup(repo => repo.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProductCategory?)null); 

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("Error.NotFound");
        
        _productRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
