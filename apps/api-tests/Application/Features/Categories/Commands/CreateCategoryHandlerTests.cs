using Application.Abstractions.Repositories;
using Application.Common.Models;
using Application.Features.Categories.Commands.CreateCategory;
using Domain.Entities;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Api.Tests.Application.Features.Categories.Commands.CreateCategory;

[TestFixture]
public class CreateCategoryHandlerTests
{
    private Mock<ICategoryRepository> _repositoryMock;
    private Mock<IUnitOfWork> _unitOfWorkMock;
    private Mock<ILogger<CreateCategoryHandler>> _loggerMock;
    private CreateCategoryHandler _handler;

    [SetUp]
    public void Setup()
    {
        _repositoryMock = new Mock<ICategoryRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<CreateCategoryHandler>>();

        _handler = new CreateCategoryHandler(
            _repositoryMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }

    [Test]
    public async Task Handle_WithValidName_ShouldCreateCategoryAndReturnSuccess()
    {
        // Arrange
        var command = new CreateCategoryCommand("RAM");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();

        _repositoryMock.Verify(r => r.AddAsync(
            It.Is<ProductCategory>(c => c.Name == "RAM"),
            It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
