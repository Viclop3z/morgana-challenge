using AutoFixture;
using AutoFixture.AutoMoq;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Data;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using UmbracoBridge.Application.Contracts;
using UmbracoBridge.Application.Features.Commands.DocumentType.Create;
using UmbracoBridge.Domain.Contracts.Infrastructure.Services;
using UmbracoBridge.Domain.Exceptions;
using UmbracoBridge.Domain.Models;
using Xunit;

public class DocumentTypeCommandHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IDocumentTypeService> _documentTypeServiceMock;
    private readonly Mock<ITokenManager> _tokenManagerMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly DocumentTypeCommandHandler _handler;
    private readonly Mock<Logger<DocumentTypeCommandHandler>> _logger;

    public DocumentTypeCommandHandlerTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());

        _documentTypeServiceMock = _fixture.Freeze<Mock<IDocumentTypeService>>();
        _tokenManagerMock = _fixture.Freeze<Mock<ITokenManager>>();
        _mapperMock = _fixture.Freeze<Mock<IMapper>>();
        _logger = _fixture.Freeze<Mock<Logger<DocumentTypeCommandHandler>>>();
        _handler = new DocumentTypeCommandHandler(
            _documentTypeServiceMock.Object,
            _tokenManagerMock.Object,
            _mapperMock.Object,_logger.Object

        );
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsExpectedResponse()
    {
        // Arrange
        var command = _fixture.Create<DocumentTypeCommand>();
        var token = _fixture.Create<string>();
        var request = _fixture.Create<DocumentTypeRequest>();
        var domainResponse = _fixture.Create<DocumentType>();
        var expectedResponse = _fixture.Create<DocumentTypeResponse>();

        _tokenManagerMock.Setup(t => t.GetToken()).ReturnsAsync(token);
        _mapperMock.Setup(m => m.Map<DocumentTypeRequest>(command)).Returns(request);
        _documentTypeServiceMock.Setup(s => s.CreateDocumentType(token, request)).ReturnsAsync(domainResponse);
        _mapperMock.Setup(m => m.Map<DocumentTypeResponse>(domainResponse)).Returns(expectedResponse);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedResponse, result);
    }

 
    [Fact]
    public async Task Handle_NullDomainResponse_ThrowsApplicationException()
    {
        // Arrange
        var command = _fixture.Create<DocumentTypeCommand>();
        var token = _fixture.Create<string>();
        var request = _fixture.Create<DocumentTypeRequest>();

        _tokenManagerMock.Setup(t => t.GetToken()).ReturnsAsync(token);
        _mapperMock.Setup(m => m.Map<DocumentTypeRequest>(command)).Returns(request);
        _documentTypeServiceMock.Setup(s => s.CreateDocumentType(token, request)).ReturnsAsync((DocumentType)null!);

        // Act & Assert
        await Assert.ThrowsAsync<ApplicationException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_FailedMapper_ThrowsMappingException()
    {
        // Arrange
        var command = _fixture.Create<DocumentTypeCommand>();
        var token = _fixture.Create<string>();
        var request = _fixture.Create<DocumentTypeRequest>();

        _tokenManagerMock.Setup(t => t.GetToken()).ReturnsAsync(token);
        _mapperMock.Setup(m => m.Map<DocumentTypeRequest>(command)).Throws(new AutoMapperMappingException("Mapping failed"));

        // Act & Assert
        await Assert.ThrowsAsync<AutoMapperMappingException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public void Constructor_NullDocumentTypeService_ThrowsArgumentNullException()
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentNullException>(() =>
            new DocumentTypeCommandHandler(null!, _tokenManagerMock.Object, _mapperMock.Object,_logger.Object));
        Assert.Equal("documentTypeService", ex.ParamName);
    }

    [Fact]
    public void Constructor_NullTokenManager_ThrowsArgumentNullException()
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentNullException>(() =>
            new DocumentTypeCommandHandler(_documentTypeServiceMock.Object, null!, _mapperMock.Object,_logger.Object));
        Assert.Equal("tokenManagerService", ex.ParamName);
    }

    [Fact]
    public void Constructor_NullMapper_ThrowsArgumentNullException()
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentNullException>(() =>
            new DocumentTypeCommandHandler(_documentTypeServiceMock.Object, _tokenManagerMock.Object, null!,_logger.Object));
        Assert.Equal("mapper", ex.ParamName);
    }

    [Fact]
    public async Task Handle_ThrowingServiceException_ThrowsDocumentTypeServiceException()
    {
        // Arrange
        var command = _fixture.Create<DocumentTypeCommand>();
        var token = _fixture.Create<string>();
        var request = _fixture.Create<DocumentTypeRequest>();

        _tokenManagerMock.Setup(t => t.GetToken()).ReturnsAsync(token);
        _mapperMock.Setup(m => m.Map<DocumentTypeRequest>(command)).Returns(request);
        _documentTypeServiceMock.Setup(s => s.CreateDocumentType(token, request))
            .ThrowsAsync(new ExternalServiceException("Service error",HttpStatusCode.InternalServerError));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ExternalServiceException>(() => _handler.Handle(command, CancellationToken.None));
        Assert.Equal("Service error", exception.Message);
    }


    [Fact]
    public async Task Handle_MinimalInput_StillReturnsResponse()
    {
        // Arrange
        var command = new DocumentTypeCommand(); // Comando vacío pero válido
        var token = "token";
        var request = new DocumentTypeRequest();
        var domainModel = new DocumentType();
        var expectedResponse = new DocumentTypeResponse();

        _tokenManagerMock.Setup(x => x.GetToken()).ReturnsAsync(token);
        _mapperMock.Setup(x => x.Map<DocumentTypeRequest>(command)).Returns(request);
        _documentTypeServiceMock.Setup(x => x.CreateDocumentType(token, request)).ReturnsAsync(domainModel);
        _mapperMock.Setup(x => x.Map<DocumentTypeResponse>(domainModel)).Returns(expectedResponse);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task Handle_EmptyCommand_ReturnsValidResponse()
    {
        // Arrange
        var command = new DocumentTypeCommand(); // Comando vacío
        var token = "valid-token";
        var request = new DocumentTypeRequest();
        var domainModel = new DocumentType();
        var expectedResponse = new DocumentTypeResponse();

        _tokenManagerMock.Setup(x => x.GetToken()).ReturnsAsync(token);
        _mapperMock.Setup(x => x.Map<DocumentTypeRequest>(command)).Returns(request);
        _documentTypeServiceMock.Setup(x => x.CreateDocumentType(token, request)).ReturnsAsync(domainModel);
        _mapperMock.Setup(x => x.Map<DocumentTypeResponse>(domainModel)).Returns(expectedResponse);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
    }
}
