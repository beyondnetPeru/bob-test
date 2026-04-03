using Application.Abstractions.Repositories;
using Application.Common.Models;
using Application.Features.Manufacturers.Commands.CreateManufacturer;
using Domain.Entities;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Api.Tests.Application.Features.Manufacturers.Commands.CreateManufacturer;

[TestFixture]
public class CreateManufacturerHandlerTests
{
    private Mock<IManufacturerRepository> _repositoryMock;
    private Mock<IUnitOfWork> _unitOfWorkMock;
    private Mock<ILogger<CreateManufacturerHandler>> _loggerMock;
    private CreateManufacturerHandler _handler;

    [SetUp]
    public void Setup()
    {
        _repositoryMock = new Mock<IManufacturerRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<CreateManufacturerHandler>>();

        _handler = new CreateManufacturerHandler(
            _repositoryMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }

    [Test]
    public async Task Handle_WithValidName_ShouldCreateManufacturerAndReturnSuccess()
    {
        // Arrange
        var command = new CreateManufacturerCommand("Intel");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();

        _repositoryMock.Verify(r => r.AddAsync(
            It.Is<Manufacturer>(m => m.Name == "Intel"),
            It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
