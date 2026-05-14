using BoardGameApp.Application.Common;
using BoardGameApp.Application.Games;
using BoardGameApp.Application.Matches;
using BoardGameApp.Application.Players;
using BoardGameApp.Web.Areas.Matches.Controllers;
using BoardGameApp.Web.Areas.Matches.Models;
using Microsoft.AspNetCore.Mvc;

namespace BoardGameApp.Tests;

public class MatchControllerTests
{
    [Fact]
    public async Task Index_lists_matches_with_filters()
    {
        var service = new FakeMatchService();
        var controller = CreateController(service);

        var result = await controller.Index(id: 7, gameId: 3, page: 2);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<MatchIndexViewModel>(viewResult.Model);
        Assert.Equal(7, model.Id);
        Assert.Equal(3, model.GameId);
        Assert.Equal(new MatchFilter(7, 3), service.LastFilter);
        Assert.Equal(2, service.LastPage);
        Assert.Equal("Matches", controller.ViewData["ActiveNav"]);
    }

    [Fact]
    public async Task Create_post_redirects_to_index_when_match_is_created()
    {
        var service = new FakeMatchService();
        var controller = CreateController(service);
        var dto = new MatchCreateViewModel
        {
            GameId = 1,
            PlayerIds = "1,2",
            Scores = "5,8",
            WinnerPlayerId = 2
        };

        var result = await controller.Create(dto);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(MatchController.Index), redirect.ActionName);
        Assert.Equal(dto.ToDto(), service.CreatedMatch);
    }

    [Fact]
    public async Task Create_get_returns_players_and_games_for_selection_modal()
    {
        var controller = CreateController(new FakeMatchService());

        var result = await controller.Create();

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<MatchCreateViewModel>(viewResult.Model);
        Assert.Single(model.Games);
        Assert.Equal(2, model.Players.Count);
    }

    [Fact]
    public async Task Edit_get_returns_not_found_when_match_does_not_exist()
    {
        var service = new FakeMatchService { MatchToReturn = null };
        var controller = CreateController(service);

        var result = await controller.Edit(99);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Edit_get_returns_score_update_dto()
    {
        var service = new FakeMatchService();
        var controller = CreateController(service);

        var result = await controller.Edit(5);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<MatchEditScoresViewModel>(viewResult.Model);
        Assert.Equal(5, model.Match.Id);
        Assert.Equal(1, model.Match.GameId);
        Assert.Equal("1,2", model.Match.PlayerIds);
        Assert.Equal("5,8", model.ScoreUpdate.Scores);
    }

    [Fact]
    public async Task Edit_post_updates_scores_and_redirects_to_index()
    {
        var service = new FakeMatchService();
        var controller = CreateController(service);
        var dto = new UpdateMatchScoresDto(5, "9,4");

        var result = await controller.Edit(dto);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(MatchController.Index), redirect.ActionName);
        Assert.Equal(dto, service.UpdatedScores);
    }

    private static MatchController CreateController(FakeMatchService service)
    {
        return new MatchController(service, new FakeGameService(), new FakePlayerService());
    }

    private sealed class FakeMatchService : IMatchService
    {
        public CreateMatchDto? CreatedMatch { get; private set; }

        public UpdateMatchScoresDto? UpdatedScores { get; private set; }

        public MatchFilter? LastFilter { get; private set; }

        public int? LastPage { get; private set; }

        public MatchViewDto? MatchToReturn { get; init; } = new(
            5,
            1,
            "1,2",
            "5,8",
            2,
            new DateTime(2026, 5, 12, 18, 30, 0));

        public Task<MatchViewDto> CreateAsync(
            CreateMatchDto dto,
            CancellationToken cancellationToken = default)
        {
            CreatedMatch = dto;

            return Task.FromResult(new MatchViewDto(
                1,
                dto.GameId,
                dto.PlayerIds,
                dto.Scores,
                2,
                DateTime.Now));
        }

        public Task<MatchViewDto> UpdateScoresAsync(
            UpdateMatchScoresDto dto,
            CancellationToken cancellationToken = default)
        {
            UpdatedScores = dto;

            return Task.FromResult(new MatchViewDto(
                dto.Id,
                1,
                "1,2",
                dto.Scores,
                1,
                new DateTime(2026, 5, 12, 18, 30, 0)));
        }

        public Task<MatchViewDto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(MatchToReturn);
        }

        public Task<PagedResult<MatchViewDto>> ListAsync(
            MatchFilter filter,
            int page = 1,
            CancellationToken cancellationToken = default)
        {
            LastFilter = filter;
            LastPage = page;

            return Task.FromResult(new PagedResult<MatchViewDto>(
                [new MatchViewDto(1, 1, "1,2", "5,8", 2, new DateTime(2026, 5, 12, 18, 30, 0))],
                page,
                20,
                1));
        }
    }

    private sealed class FakeGameService : IGameService
    {
        public Task<GameViewDto> CreateAsync(
            CreateGameDto dto,
            CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        public Task<GameViewDto> UpdateAsync(
            UpdateGameDto dto,
            CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        public Task ActivateAsync(int id, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        public Task DeactivateAsync(int id, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        public Task<GameViewDto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        public Task<PagedResult<GameViewDto>> ListAsync(
            GameFilter filter,
            int page = 1,
            bool includeInactive = false,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new PagedResult<GameViewDto>(
                [new GameViewDto(1, "Azul", 1, 1, 1, 0, 4, true)],
                1,
                20,
                1));
        }
    }

    private sealed class FakePlayerService : IPlayerService
    {
        public Task<PlayerViewDto> CreateAsync(
            CreatePlayerDto dto,
            CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        public Task<PlayerViewDto> UpdateAsync(
            UpdatePlayerDto dto,
            CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        public Task DeactivateAsync(int id, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        public Task<PlayerViewDto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        public Task<PagedResult<PlayerViewDto>> ListAsync(
            PlayerFilter filter,
            int page = 1,
            bool includeInactive = false,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new PagedResult<PlayerViewDto>(
                [
                    new PlayerViewDto(1, "Ada Lovelace", "1111", true),
                    new PlayerViewDto(2, "Grace Hopper", "2222", true)
                ],
                1,
                20,
                2));
        }
    }
}
