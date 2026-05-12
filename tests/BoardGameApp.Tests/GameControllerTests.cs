using BoardGameApp.Application.Common;
using BoardGameApp.Application.Games;
using BoardGameApp.Web.Areas.Games.Controllers;
using BoardGameApp.Web.Areas.Games.Models;
using Microsoft.AspNetCore.Mvc;

namespace BoardGameApp.Tests;

public class GameControllerTests
{
    [Fact]
    public async Task Index_lists_games_with_filters()
    {
        var service = new FakeGameService();
        var controller = new GameController(service);

        var result = await controller.Index(
            id: 7,
            name: "Azul",
            author: "Michael",
            genreId: 2,
            publisherId: 3,
            page: 2,
            includeInactive: true);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<GameIndexViewModel>(viewResult.Model);
        Assert.Equal(7, model.Id);
        Assert.Equal("Azul", model.Name);
        Assert.Equal("Michael", model.Author);
        Assert.Equal(2, model.GenreId);
        Assert.Equal(3, model.PublisherId);
        Assert.True(model.IncludeInactive);
        Assert.Equal(new GameFilter(7, "Azul", "Michael", 2, 3), service.LastFilter);
        Assert.Equal(2, service.LastPage);
        Assert.True(service.LastIncludeInactive);
        Assert.Equal("Games", controller.ViewData["ActiveNav"]);
    }

    [Fact]
    public async Task Create_post_redirects_to_index_when_game_is_created()
    {
        var service = new FakeGameService();
        var controller = new GameController(service);
        var dto = new CreateGameDto("Azul", 1, 2, "Michael Kiesling", 4);

        var result = await controller.Create(dto);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(GameController.Index), redirect.ActionName);
        Assert.Equal(dto, service.CreatedGame);
    }

    [Fact]
    public async Task Edit_get_returns_not_found_when_game_does_not_exist()
    {
        var service = new FakeGameService { GameToReturn = null };
        var controller = new GameController(service);

        var result = await controller.Edit(99);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Edit_post_redirects_to_index_when_game_is_updated()
    {
        var service = new FakeGameService();
        var controller = new GameController(service);
        var dto = new UpdateGameDto(1, "Azul", 1, 2, "Michael Kiesling", 3, 4, true);

        var result = await controller.Edit(dto);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(GameController.Index), redirect.ActionName);
        Assert.Equal(dto, service.UpdatedGame);
    }

    [Fact]
    public async Task Deactivate_redirects_to_index_when_game_is_deactivated()
    {
        var service = new FakeGameService();
        var controller = new GameController(service);

        var result = await controller.Deactivate(7);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(GameController.Index), redirect.ActionName);
        Assert.Equal(7, service.DeactivatedId);
    }

    [Fact]
    public async Task Activate_redirects_to_index_including_inactive_games()
    {
        var service = new FakeGameService();
        var controller = new GameController(service);

        var result = await controller.Activate(7);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(GameController.Index), redirect.ActionName);
        Assert.True((bool?)redirect.RouteValues?["includeInactive"]);
        Assert.Equal(7, service.ActivatedId);
    }

    private sealed class FakeGameService : IGameService
    {
        public CreateGameDto? CreatedGame { get; private set; }

        public UpdateGameDto? UpdatedGame { get; private set; }

        public int? ActivatedId { get; private set; }

        public int? DeactivatedId { get; private set; }

        public GameFilter? LastFilter { get; private set; }

        public int? LastPage { get; private set; }

        public bool? LastIncludeInactive { get; private set; }

        public GameViewDto? GameToReturn { get; init; } = new(
            1,
            "Azul",
            1,
            2,
            "Michael Kiesling",
            3,
            4,
            true);

        public Task<GameViewDto> CreateAsync(
            CreateGameDto dto,
            CancellationToken cancellationToken = default)
        {
            CreatedGame = dto;

            return Task.FromResult(new GameViewDto(
                1,
                dto.Name,
                dto.PublisherId,
                dto.GenreId,
                dto.Author,
                0,
                dto.MaxPlayers,
                true));
        }

        public Task<GameViewDto> UpdateAsync(
            UpdateGameDto dto,
            CancellationToken cancellationToken = default)
        {
            UpdatedGame = dto;

            return Task.FromResult(new GameViewDto(
                dto.Id,
                dto.Name,
                dto.PublisherId,
                dto.GenreId,
                dto.Author,
                dto.TimesPlayed,
                dto.MaxPlayers,
                dto.IsActive));
        }

        public Task ActivateAsync(int id, CancellationToken cancellationToken = default)
        {
            ActivatedId = id;

            return Task.CompletedTask;
        }

        public Task DeactivateAsync(int id, CancellationToken cancellationToken = default)
        {
            DeactivatedId = id;

            return Task.CompletedTask;
        }

        public Task<GameViewDto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(GameToReturn);
        }

        public Task<PagedResult<GameViewDto>> ListAsync(
            GameFilter filter,
            int page = 1,
            bool includeInactive = false,
            CancellationToken cancellationToken = default)
        {
            LastFilter = filter;
            LastPage = page;
            LastIncludeInactive = includeInactive;

            return Task.FromResult(new PagedResult<GameViewDto>(
                [new GameViewDto(1, "Azul", 1, 2, "Michael Kiesling", 3, 4, true)],
                page,
                20,
                1));
        }
    }
}
