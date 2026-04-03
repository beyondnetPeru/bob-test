using Application.Abstractions.Repositories;
using Application.Common.Models;
using Application.Features.Categories.Commands.DeleteCategory;
using Domain.Entities;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Api.Tests.Application.Features.Categories.Commands.DeleteCategory;

[TestFixture]
public class DeleteCategoryHandlerTests
{
    private Mock<ICategoryRepository> _repositoryMock;
    private Mock<IUnitOfWork> _unitOfWorkMock;
    private Mock<ILogger<DeleteCategoryHandler>> _loggerMock;
    private DeleteCategoryHandler _handler;

    [SetUp]
    public void Setup()
    {
        _repositoryMock = new Mock<ICategoryRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<DeleteCategoryHandler>>();

        _handler = new DeleteCategoryHandler(
            _repositoryMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }

    [Test]
    public async Task Handle_WithValidId_ShouldDeleteAndReturnSuccess()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var command = new DeleteCategoryCommand(categoryId);

        var existingCategory = new ProductCategory("RAM");

        _repositoryMock
            .Setup(r => r.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingCategory);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _repositoryMock.Verify(r => r.Delete(existingCategory), Times.Once);
        _unitOfWorkMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task Handle_WithNonExistentCategory_ShouldReturnFailure()
    {
        // Arrange
        var command = new DeleteCategoryCommand(Guid.NewGuid());

        _repositoryMock
            .Setup(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProductCategory?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be(Error.NotFoundCode);

        _repositoryMock.Verify(r => r.Delete(It.IsAny<ProductCategory>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
