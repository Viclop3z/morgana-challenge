using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using UmbracoBridge.Application.Contracts;
using UmbracoBridge.Application.Features.Queries.HealthCheck;
using UmbracoBridge.Domain.Contracts.Infrastructure.Services;
using UmbracoBridge.Domain.Models;
using Xunit;

public class HealthCheckQueryHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<ITokenManager> _tokenManagerMock;
    private readonly Mock<IHealthCheckService> _healthCheckServiceMock;
    private readonly HealthCheckQueryHandler _handler;

    public HealthCheckQueryHandlerTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());

        _tokenManagerMock = _fixture.Freeze<Mock<ITokenManager>>();
        _healthCheckServiceMock = _fixture.Freeze<Mock<IHealthCheckService>>();

        _handler = new HealthCheckQueryHandler(
            _tokenManagerMock.Object,
            _healthCheckServiceMock.Object
        );
    }
 
    [Fact]
    public async Task Handle_ValidRequest_ReturnsHealthCheckResponse()
    {
        // Arrange
        var token = "valid-token";
        var query = new GetHealthCehckQuery();
        var healthCheckModel = new HealthCheckGroup
        {
            Total = 2,
            Items = new List<UmbracoBridge.Domain.Models.HealthCheckGroupItem>
            {
                new UmbracoBridge.Domain.Models.HealthCheckGroupItem { Name ="ServiceA" },
                new UmbracoBridge.Domain.Models.HealthCheckGroupItem { Name ="ServiceB" },
            }

        };

        _tokenManagerMock.Setup(x => x.GetToken()).ReturnsAsync(token);
        _healthCheckServiceMock.Setup(x => x.GetHealthCheck(token)).ReturnsAsync(healthCheckModel);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Total);
        Assert.Equal(2, result.Items.Count);
        Assert.Contains(result.Items, i => i.Name == "ServiceA");
        Assert.Contains(result.Items, i => i.Name == "ServiceB");
    }

    [Fact]
    public async Task Handle_HealthCheckServiceReturnsNull_ReturnsNull()
    {
        // Arrange
        var query = new GetHealthCehckQuery();
        var token = "valid-token";

        _tokenManagerMock.Setup(x => x.GetToken()).ReturnsAsync(token);
        _healthCheckServiceMock.Setup(x => x.GetHealthCheck(token)).ReturnsAsync((HealthCheckGroup)null!);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_HealthCheckServiceThrows_ThrowsException()
    {
        // Arrange
        var query = new GetHealthCehckQuery();
        var token = "valid-token";

        _tokenManagerMock.Setup(x => x.GetToken()).ReturnsAsync(token);
        _healthCheckServiceMock.Setup(x => x.GetHealthCheck(token)).ThrowsAsync(new Exception("Service error"));

        // Act & Assert
        var ex = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(query, CancellationToken.None));
        Assert.Equal("Service error", ex.Message);
    }

    [Fact]
    public void Constructor_NullTokenManager_ThrowsArgumentNullException()
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentNullException>(() =>
            new HealthCheckQueryHandler(null!, _healthCheckServiceMock.Object));
        Assert.Equal("tokenManagerService", ex.ParamName);
    }

    [Fact]
    public void Constructor_NullHealthCheckService_ThrowsArgumentNullException()
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentNullException>(() =>
            new HealthCheckQueryHandler(_tokenManagerMock.Object, null!));
        Assert.Equal("healthCheckService", ex.ParamName);
    }


    [Fact]
    public async Task Handle_EmptyItemsInHealthCheck_ReturnsResponseWithEmptyList()
    {
        // Arrange
        var query = new GetHealthCehckQuery();
        var token = "valid-token";

        var healthCheckModel = new HealthCheckGroup
        {
            Total = 0,
            Items = new List<UmbracoBridge.Domain.Models.HealthCheckGroupItem>()
        };

        _tokenManagerMock.Setup(x => x.GetToken()).ReturnsAsync(token);
        _healthCheckServiceMock.Setup(x => x.GetHealthCheck(token)).ReturnsAsync(healthCheckModel);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Items);
        Assert.Equal(0, result.Total);
    }

    [Fact]
    public async Task Handle_SingleItemHealthCheck_ReturnsValidResponse()
    {
        // Arrange
        var query = new GetHealthCehckQuery();
        var token = "valid-token";

        var healthCheckModel = new HealthCheckGroup
        {
            Total = 1,
            Items = new List<UmbracoBridge.Domain.Models.HealthCheckGroupItem>
            {
                new UmbracoBridge.Domain.Models.HealthCheckGroupItem { Name = "Database" }
            }
        };

        _tokenManagerMock.Setup(x => x.GetToken()).ReturnsAsync(token);
        _healthCheckServiceMock.Setup(x => x.GetHealthCheck(token)).ReturnsAsync(healthCheckModel);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Items);
        Assert.Equal("Database", result.Items.First().Name);
    }
}