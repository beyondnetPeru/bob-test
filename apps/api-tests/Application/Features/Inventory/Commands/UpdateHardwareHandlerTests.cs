using Application.Abstractions.Repositories;
using Application.Common.Models;
using Application.Features.Inventory.Commands.UpdateHardware;
using Domain.Entities;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Api.Tests.Application.Features.Inventory.Commands.UpdateHardware;

[TestFixture]
public class UpdateHardwareHandlerTests
{
    private Mock<IInventoryRepository> _repositoryMock;
    private Mock<IUnitOfWork> _unitOfWorkMock;
    private Mock<ILogger<UpdateHardwareHandler>> _loggerMock;
    private UpdateHardwareHandler _handler;

    [SetUp]
    public void Setup()
    {
        _repositoryMock = new Mock<IInventoryRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<UpdateHardwareHandler>>();

        _handler = new UpdateHardwareHandler(
            _repositoryMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }

    [Test]
    public async Task Handle_WithValidRequest_ShouldUpdateHardwareAndReturnSuccess()
    {
        // Arrange
        var id = Guid.NewGuid();
        var command = new UpdateHardwareCommand(id, "NewAsset", 20m);
        
        var existingHardware = new HardwareInventory("OldAsset", 10m);

        _repositoryMock
            .Setup(repo => repo.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingHardware);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        
        _repositoryMock.Verify(r => r.Update(existingHardware), Times.Once);
        _unitOfWorkMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task Handle_WithInvalidId_ShouldReturnFailure()
    {
        // Arrange
        var command = new UpdateHardwareCommand(Guid.NewGuid(), "NewAsset", 20m);

        _repositoryMock
            .Setup(repo => repo.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((HardwareInventory?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("Error.NotFound");
        
        _repositoryMock.Verify(r => r.Update(It.IsAny<HardwareInventory>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
