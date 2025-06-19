using AutoFixture;
using AutoFixture.AutoMoq;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using UmbracoBridge.Application.Features.Commands.DocumentType.Create;
using UmbracoBridge.Application.Features.Commands.DocumentType.Delete;
using UmbracoBridge.Application.Features.Queries.HealthCheck;
using UmbracoBridge.Controllers;
using Xunit;

public class ManagementControllerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<ILogger<ManagementController>> _loggerMock;
    private readonly ManagementController _controller;

    public ManagementControllerTests()

    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _mediatorMock = _fixture.Freeze<Mock<IMediator>>();
        _loggerMock = _fixture.Freeze<Mock<ILogger<ManagementController>>>();
        _controller = new ManagementController(_mediatorMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task HealthCheck_ReturnsExpectedResult()
    {
        // Arrange
        var expected = _fixture.Create<HealthCheckResponse>();
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetHealthCehckQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        // Act
        var result = await _controller.Get();

        // Assert
        var okResult = Assert.IsType<ActionResult<HealthCheckResponse>>(result);
        Assert.Equal(expected, okResult.Value);
    }

    [Fact]
    public async Task PostDocumentType_ReturnsExpectedResponse()
    {
        // Arrange
        var command = _fixture.Create<DocumentTypeCommand>();
        var expected = _fixture.Create<DocumentTypeResponse>();

        _mediatorMock
            .Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        // Act
        var result = await _controller.Post(command);

        // Assert
        var actionResult = Assert.IsType<ActionResult<DocumentTypeResponse>>(result);
        Assert.Equal(expected, actionResult.Value);
    }

    [Fact]
    public async Task DeleteDocumentType_ReturnsNoContent()
    {
        // Arrange
        var id = _fixture.Create<string>();

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<DeleteDocumentCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Unit.Value);

        // Act
        var result = await _controller.Delete(id);

        // Assert
        Assert.IsType<ActionResult<Unit>>(result);
    }

    [Fact]
    public async Task Constructor_WithNullMediator_ThrowsArgumentNullException()
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentNullException>(() =>
            new ManagementController(null!, _loggerMock.Object));
        Assert.Equal("mediator", ex.ParamName);
    }

    [Fact]
    public async Task Constructor_WithNullLogger_ThrowsArgumentNullException()
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentNullException>(() =>
            new ManagementController(_mediatorMock.Object, null!));
        Assert.Equal("logger", ex.ParamName);
    }

  
}
