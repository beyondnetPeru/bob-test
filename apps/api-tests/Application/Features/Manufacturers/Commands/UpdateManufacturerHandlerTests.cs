using Application.Abstractions.Repositories;
using Application.Common.Models;
using Application.Features.Manufacturers.Commands.UpdateManufacturer;
using Domain.Entities;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Api.Tests.Application.Features.Manufacturers.Commands.UpdateManufacturer;

[TestFixture]
public class UpdateManufacturerHandlerTests
{
    private Mock<IManufacturerRepository> _repositoryMock;
    private Mock<IUnitOfWork> _unitOfWorkMock;
    private Mock<ILogger<UpdateManufacturerHandler>> _loggerMock;
    private UpdateManufacturerHandler _handler;

    [SetUp]
    public void Setup()
    {
        _repositoryMock = new Mock<IManufacturerRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<UpdateManufacturerHandler>>();

        _handler = new UpdateManufacturerHandler(
            _repositoryMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }

    [Test]
    public async Task Handle_WithValidRequest_ShouldUpdateAndReturnSuccess()
    {
        // Arrange
        var manufacturerId = Guid.NewGuid();
        var command = new UpdateManufacturerCommand(manufacturerId, "AMD");

        var existing = new Manufacturer("Intel");

        _repositoryMock
            .Setup(r => r.GetByIdAsync(manufacturerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _repositoryMock.Verify(r => r.Update(existing), Times.Once);
        _unitOfWorkMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task Handle_WithNonExistentManufacturer_ShouldReturnFailure()
    {
        // Arrange
        var command = new UpdateManufacturerCommand(Guid.NewGuid(), "AMD");

        _repositoryMock
            .Setup(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Manufacturer?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be(Error.NotFoundCode);

        _repositoryMock.Verify(r => r.Update(It.IsAny<Manufacturer>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
