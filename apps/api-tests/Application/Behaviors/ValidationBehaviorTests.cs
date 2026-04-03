using Application.Behaviors;
using Application.Common.Models;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Moq;
using NUnit.Framework;

namespace Api.Tests.Application.Behaviors;

// Dummy command/result types for testing the pipeline behavior in isolation
public sealed record TestCommand(string Name) : IRequest<Result<Guid>>;
public sealed record TestVoidCommand(string Name) : IRequest<Result>;

[TestFixture]
public class ValidationBehaviorTests
{
    [Test]
    public async Task Handle_WithNoValidators_ShouldCallNext()
    {
        // Arrange
        var validators = Enumerable.Empty<IValidator<TestCommand>>();
        var behavior = new ValidationBehavior<TestCommand, Result<Guid>>(validators);
        var command = new TestCommand("ValidName");
        var expectedId = Guid.NewGuid();

        // Act
        var result = await behavior.Handle(
            command,
            () => Task.FromResult(Result.Success(expectedId)),
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(expectedId);
    }

    [Test]
    public async Task Handle_WithPassingValidators_ShouldCallNext()
    {
        // Arrange
        var validatorMock = new Mock<IValidator<TestCommand>>();
        validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TestCommand>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        var behavior = new ValidationBehavior<TestCommand, Result<Guid>>(new[] { validatorMock.Object });
        var command = new TestCommand("ValidName");
        var expectedId = Guid.NewGuid();

        // Act
        var result = await behavior.Handle(
            command,
            () => Task.FromResult(Result.Success(expectedId)),
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(expectedId);
    }

    [Test]
    public async Task Handle_WithFailingValidator_ShouldReturnTypedResultFailure()
    {
        // Arrange
        var validatorMock = new Mock<IValidator<TestCommand>>();
        validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TestCommand>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(new[]
            {
                new ValidationFailure("Name", "Name is required")
            }));

        var behavior = new ValidationBehavior<TestCommand, Result<Guid>>(new[] { validatorMock.Object });
        var command = new TestCommand("");

        // Act
        var result = await behavior.Handle(
            command,
            () => Task.FromResult(Result.Success(Guid.NewGuid())),
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be(Error.ValidationCode);
        result.Error.Message.Should().Contain("Name is required");
    }

    [Test]
    public async Task Handle_WithFailingValidator_AndNonGenericResult_ShouldReturnFailure()
    {
        // Arrange
        var validatorMock = new Mock<IValidator<TestVoidCommand>>();
        validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TestVoidCommand>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(new[]
            {
                new ValidationFailure("Name", "Name cannot be empty")
            }));

        var behavior = new ValidationBehavior<TestVoidCommand, Result>(new[] { validatorMock.Object });
        var command = new TestVoidCommand("");

        // Act
        var result = await behavior.Handle(
            command,
            () => Task.FromResult(Result.Success()),
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be(Error.ValidationCode);
        result.Error.Message.Should().Contain("Name cannot be empty");
    }

    [Test]
    public async Task Handle_WithMultipleFailures_ShouldCombineErrorMessages()
    {
        // Arrange
        var validatorMock = new Mock<IValidator<TestCommand>>();
        validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TestCommand>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(new[]
            {
                new ValidationFailure("Name", "Name is required"),
                new ValidationFailure("Name", "Name must be at most 255 characters")
            }));

        var behavior = new ValidationBehavior<TestCommand, Result<Guid>>(new[] { validatorMock.Object });
        var command = new TestCommand("");

        // Act
        var result = await behavior.Handle(
            command,
            () => Task.FromResult(Result.Success(Guid.NewGuid())),
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Message.Should().Contain("Name is required");
        result.Error.Message.Should().Contain("Name must be at most 255 characters");
    }
}
