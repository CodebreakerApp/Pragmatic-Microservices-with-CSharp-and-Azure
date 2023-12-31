namespace Codebreaker.GameAPIs.Tests;

public class GamesServiceTests
{
    private readonly Mock<IGamesRepository> _gamesRepositoryMock = new();
    private Guid _endedGameId = Guid.Parse("4786C27B-3F9A-4C47-9947-F983CF7053E6");
    private Game _endedGame;
    private Guid _running6x4GameId = Guid.Parse("4786C27B-3F9A-4C47-9947-F983CF7053E7");
    private Game _running6x4Game;
    private Guid _notFoundGameId = Guid.Parse("4786C27B-3F9A-4C47-9947-F983CF7053E8");
    private Guid _running6x4MoveId1 = Guid.Parse("4786C27B-3F9A-4C47-9947-F983CF7053E9");
    private string[] _guessesMove1 = ["Red", "Green", "Blue", "Yellow"];

    public GamesServiceTests()
    {
        _endedGame = new(_endedGameId, "Game6x4", "Test", DateTime.Now, 4, 12)
        {
            Codes = ["Red", "Green", "Blue", "Yellow"],
            FieldValues = new Dictionary<string, IEnumerable<string>>()
            {
                { FieldCategories.Colors, new string[] { "Red", "Green", "Blue", "Yellow", "Purple", "Orange" } }
            },
            EndTime = DateTime.Now.AddMinutes(3)
        };

        _running6x4Game = new(_running6x4GameId, "Game6x4", "Test", DateTime.Now, 4, 12)
        {
            Codes = ["Red", "Green", "Blue", "Yellow"],
            FieldValues = new Dictionary<string, IEnumerable<string>>()
            {
                { FieldCategories.Colors, new string[] { "Red", "Green", "Blue", "Yellow", "Purple", "Orange" } }
            }
        };

        _gamesRepositoryMock.Setup(repo => repo.GetGameAsync(_endedGameId, CancellationToken.None)).ReturnsAsync(_endedGame);
        _gamesRepositoryMock.Setup(repo => repo.GetGameAsync(_running6x4GameId, CancellationToken.None)).ReturnsAsync(_running6x4Game);
        _gamesRepositoryMock.Setup(repo => repo.AddMoveAsync(_running6x4Game, It.IsAny<Move>(), CancellationToken.None));
    }

    [Fact]
    public async Task SetMoveAsync_Should_ThrowWithEndedGame()
    {
        await Assert.ThrowsAsync<CodebreakerException>(async () =>
        {
            GamesService gamesService = new GamesService(_gamesRepositoryMock.Object);
            await gamesService.SetMoveAsync(_endedGameId, "Game6x4", ["Red", "Green", "Blue", "Yellow"], 1, CancellationToken.None);
        });

        _gamesRepositoryMock.Verify(repo => repo.GetGameAsync(_endedGameId, CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task SetMoveAsync_Should_ThrowWithUnexpectedGAmeType()
    {
        await Assert.ThrowsAsync<CodebreakerException>(async () =>
        {
            GamesService gamesService = new GamesService(_gamesRepositoryMock.Object);
            await gamesService.SetMoveAsync(_running6x4GameId, "Game8x5", ["Red", "Green", "Blue", "Yellow"], 1, CancellationToken.None);
        });

        _gamesRepositoryMock.Verify(repo => repo.GetGameAsync(_running6x4GameId, CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task SetMoveAsync_Should_ThrowWithNotFoundGame()
    {
        await Assert.ThrowsAsync<CodebreakerException>(async () =>
        {
            GamesService gamesService = new GamesService(_gamesRepositoryMock.Object);
            await gamesService.SetMoveAsync(_notFoundGameId, "Game6x4", ["Red", "Green", "Blue", "Yellow"], 1, CancellationToken.None);
        });

        _gamesRepositoryMock.Verify(repo => repo.GetGameAsync(_notFoundGameId, CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task GetGameAsync_Should_ReturnAGame()
    {
        // Arrange
        GamesService gamesService = new GamesService(_gamesRepositoryMock.Object);

        // Act
        Game? game = await gamesService.GetGameAsync(_running6x4GameId, CancellationToken.None);

        // Assert
        Assert.Equal(_running6x4Game, game);
        _gamesRepositoryMock.Verify(repo => repo.GetGameAsync(_running6x4GameId, CancellationToken.None), Times.Once);

    }

    [Fact]
    public async Task SetMoveAsync_Should_UpdateGameAndAddMove()
    {
        // Arrange
        var gamesService = new GamesService(_gamesRepositoryMock.Object);

        // Act
        var result = await gamesService.SetMoveAsync(_running6x4GameId, "Game6x4", ["Red", "Green", "Blue", "Yellow"], 1, CancellationToken.None);

        // Assert
        Assert.Equal(_running6x4Game, result.Game);
        Assert.Single(result.Game.Moves);

        _gamesRepositoryMock.Verify(repo => repo.GetGameAsync(_running6x4GameId, CancellationToken.None), Times.Once);
        _gamesRepositoryMock.Verify(repo => repo.AddMoveAsync(_running6x4Game, It.IsAny<Move>(), CancellationToken.None), Times.Once);
    }
}
