using Application.Abstractions.Repositories;
using Application.Common.Models;
using Application.Features.Inventory.Commands.CreateHardware;
using Domain.Entities;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Api.Tests.Application.Features.Inventory.Commands.CreateHardware;

[TestFixture]
public class CreateHardwareHandlerTests
{
    private Mock<IInventoryRepository> _repositoryMock;
    private Mock<IUnitOfWork> _unitOfWorkMock;
    private Mock<ILogger<CreateHardwareHandler>> _loggerMock;
    private CreateHardwareHandler _handler;

    [SetUp]
    public void Setup()
    {
        _repositoryMock = new Mock<IInventoryRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<CreateHardwareHandler>>();

        _handler = new CreateHardwareHandler(
            _repositoryMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }

    [Test]
    public async Task Handle_WithValidRequest_ShouldCreateHardwareAndReturnSuccess()
    {
        // Arrange
        var command = new CreateHardwareCommand("ServerAsset", 15.5m);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
        
        _repositoryMock.Verify(r => r.AddAsync(It.Is<HardwareInventory>(h => h.AssetName == "ServerAsset" && h.WeightKg == 15.5m), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
