using BoardGameApp.Application.Common;
using BoardGameApp.Application.Players;
using BoardGameApp.Web.Areas.Players.Controllers;
using BoardGameApp.Web.Areas.Players.Models;
using Microsoft.AspNetCore.Mvc;

namespace BoardGameApp.Tests;

public class PlayerControllerTests
{
    [Fact]
    public async Task Index_lists_players_with_filters()
    {
        var service = new FakePlayerService();
        var controller = new PlayerController(service);

        var result = await controller.Index("Ada", "555", page: 2, includeInactive: true);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<PlayerIndexViewModel>(viewResult.Model);
        Assert.Equal("Ada", model.FullName);
        Assert.Equal("555", model.WhatsApp);
        Assert.True(model.IncludeInactive);
        Assert.Equal(new PlayerFilter("Ada", "555"), service.LastFilter);
        Assert.Equal(2, service.LastPage);
        Assert.True(service.LastIncludeInactive);
        Assert.Equal("Players", controller.ViewData["ActiveNav"]);
    }

    [Fact]
    public async Task Create_post_redirects_to_index_when_player_is_created()
    {
        var service = new FakePlayerService();
        var controller = new PlayerController(service);

        var result = await controller.Create(new CreatePlayerDto("Ada Lovelace", "555"));

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(PlayerController.Index), redirect.ActionName);
        Assert.Equal("Ada Lovelace", service.CreatedPlayer?.FullName);
    }

    [Fact]
    public async Task Edit_get_returns_not_found_when_player_does_not_exist()
    {
        var service = new FakePlayerService { PlayerToReturn = null };
        var controller = new PlayerController(service);

        var result = await controller.Edit(99);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Edit_post_redirects_to_index_when_player_is_updated()
    {
        var service = new FakePlayerService();
        var controller = new PlayerController(service);
        var dto = new UpdatePlayerDto(1, "Ada Lovelace", "555", true);

        var result = await controller.Edit(dto);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(PlayerController.Index), redirect.ActionName);
        Assert.Equal(dto, service.UpdatedPlayer);
    }

    [Fact]
    public async Task Deactivate_redirects_to_index_when_player_is_deactivated()
    {
        var service = new FakePlayerService();
        var controller = new PlayerController(service);

        var result = await controller.Deactivate(7);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(PlayerController.Index), redirect.ActionName);
        Assert.Equal(7, service.DeactivatedId);
    }

    private sealed class FakePlayerService : IPlayerService
    {
        public CreatePlayerDto? CreatedPlayer { get; private set; }

        public UpdatePlayerDto? UpdatedPlayer { get; private set; }

        public int? DeactivatedId { get; private set; }

        public PlayerFilter? LastFilter { get; private set; }

        public int? LastPage { get; private set; }

        public bool? LastIncludeInactive { get; private set; }

        public PlayerViewDto? PlayerToReturn { get; init; } = new(1, "Ada Lovelace", "555", true);

        public Task<PlayerViewDto> CreateAsync(
            CreatePlayerDto dto,
            CancellationToken cancellationToken = default)
        {
            CreatedPlayer = dto;

            return Task.FromResult(new PlayerViewDto(1, dto.FullName, dto.WhatsApp, true));
        }

        public Task<PlayerViewDto> UpdateAsync(
            UpdatePlayerDto dto,
            CancellationToken cancellationToken = default)
        {
            UpdatedPlayer = dto;

            return Task.FromResult(new PlayerViewDto(dto.Id, dto.FullName, dto.WhatsApp, dto.IsActive));
        }

        public Task DeactivateAsync(int id, CancellationToken cancellationToken = default)
        {
            DeactivatedId = id;

            return Task.CompletedTask;
        }

        public Task<PlayerViewDto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(PlayerToReturn);
        }

        public Task<PagedResult<PlayerViewDto>> ListAsync(
            PlayerFilter filter,
            int page = 1,
            bool includeInactive = false,
            CancellationToken cancellationToken = default)
        {
            LastFilter = filter;
            LastPage = page;
            LastIncludeInactive = includeInactive;

            return Task.FromResult(new PagedResult<PlayerViewDto>(
                [new PlayerViewDto(1, "Ada Lovelace", "555", true)],
                page,
                20,
                1));
        }
    }
}
