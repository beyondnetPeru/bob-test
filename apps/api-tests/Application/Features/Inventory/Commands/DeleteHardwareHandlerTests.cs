using Application.Abstractions.Repositories;
using Application.Common.Models;
using Application.Features.Inventory.Commands.DeleteHardware;
using Domain.Entities;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Api.Tests.Application.Features.Inventory.Commands.DeleteHardware;

[TestFixture]
public class DeleteHardwareHandlerTests
{
    private Mock<IInventoryRepository> _repositoryMock;
    private Mock<IUnitOfWork> _unitOfWorkMock;
    private Mock<ILogger<DeleteHardwareHandler>> _loggerMock;
    private DeleteHardwareHandler _handler;

    [SetUp]
    public void Setup()
    {
        _repositoryMock = new Mock<IInventoryRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<DeleteHardwareHandler>>();

        _handler = new DeleteHardwareHandler(
            _repositoryMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }

    [Test]
    public async Task Handle_WithValidId_ShouldDeleteAndReturnSuccess()
    {
        // Arrange
        var hardwareId = Guid.NewGuid();
        var command = new DeleteHardwareCommand(hardwareId);

        var existing = new HardwareInventory("Server1", 10m);

        _repositoryMock
            .Setup(r => r.GetByIdAsync(hardwareId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _repositoryMock.Verify(r => r.Delete(existing), Times.Once);
        _unitOfWorkMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task Handle_WithNonExistentHardware_ShouldReturnFailure()
    {
        // Arrange
        var command = new DeleteHardwareCommand(Guid.NewGuid());

        _repositoryMock
            .Setup(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((HardwareInventory?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be(Error.NotFoundCode);

        _repositoryMock.Verify(r => r.Delete(It.IsAny<HardwareInventory>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
