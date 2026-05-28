using BoardGameApp.Application.Games;
using BoardGameApp.Web.Areas.Games.Models;
using Microsoft.AspNetCore.Mvc;

namespace BoardGameApp.Web.Areas.Games.Controllers;

[Area("Games")]
public sealed class GameController : Controller
{
    private readonly IGameService gameService;

    public GameController(IGameService gameService)
    {
        this.gameService = gameService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(
        int? id,
        string? name,
        string? author,
        int? genreId,
        int? publisherId,
        int page = 1,
        bool includeInactive = false,
        CancellationToken cancellationToken = default)
    {
        SetGamesNavigation("Games");

        var games = await gameService.ListAsync(
            new GameFilter(id, name, author, genreId, publisherId),
            page,
            includeInactive,
            cancellationToken);
        var genreOptions = await gameService.ListGenreOptionsAsync(cancellationToken);
        var publisherOptions = await gameService.ListPublisherOptionsAsync(cancellationToken);

        return View(new GameIndexViewModel
        {
            Games = games,
            GenreOptions = genreOptions,
            PublisherOptions = publisherOptions,
            Id = id,
            Name = name,
            Author = author,
            GenreId = genreId,
            PublisherId = publisherId,
            IncludeInactive = includeInactive
        });
    }

    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken cancellationToken = default)
    {
        SetGamesNavigation("Create Game");

        return View(await BuildCreateViewModelAsync(cancellationToken: cancellationToken));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        GameCreateViewModel model,
        CancellationToken cancellationToken = default)
    {
        SetGamesNavigation("Create Game");

        if (!ModelState.IsValid)
        {
            return View(await BuildCreateViewModelAsync(model, cancellationToken));
        }

        try
        {
            await gameService.CreateAsync(model.ToDto(), cancellationToken);
        }
        catch (InvalidOperationException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            return View(await BuildCreateViewModelAsync(model, cancellationToken));
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(
        int id,
        CancellationToken cancellationToken = default)
    {
        SetGamesNavigation("Edit Game");

        var game = await gameService.GetByIdAsync(id, cancellationToken);

        if (game is null)
        {
            return NotFound();
        }

        return View(await BuildEditViewModelAsync(
            new GameEditViewModel
            {
                Id = game.Id,
                Name = game.Name,
                PublisherId = game.PublisherId,
                GenreId = game.GenreId,
                AuthorId = game.AuthorId,
                TimesPlayed = game.TimesPlayed,
                MaxPlayers = game.MaxPlayers,
                IsActive = game.IsActive
            },
            cancellationToken));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        GameEditViewModel model,
        CancellationToken cancellationToken = default)
    {
        SetGamesNavigation("Edit Game");

        if (!ModelState.IsValid)
        {
            return View(await BuildEditViewModelAsync(model, cancellationToken));
        }

        try
        {
            await gameService.UpdateAsync(model.ToDto(), cancellationToken);
        }
        catch (InvalidOperationException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            return View(await BuildEditViewModelAsync(model, cancellationToken));
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Deactivate(
        int id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await gameService.DeactivateAsync(id, cancellationToken);
        }
        catch (InvalidOperationException)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Activate(
        int id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await gameService.ActivateAsync(id, cancellationToken);
        }
        catch (InvalidOperationException)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index), new { includeInactive = true });
    }

    private void SetGamesNavigation(string title)
    {
        ViewData["Title"] = title;
        ViewData["ActiveNav"] = "Games";
    }

    private async Task<GameCreateViewModel> BuildCreateViewModelAsync(
        GameCreateViewModel? model = null,
        CancellationToken cancellationToken = default)
    {
        var genreOptions = await gameService.ListGenreOptionsAsync(cancellationToken);
        var publisherOptions = await gameService.ListPublisherOptionsAsync(cancellationToken);

        return new GameCreateViewModel
        {
            Name = model?.Name ?? string.Empty,
            PublisherId = model?.PublisherId ?? 0,
            GenreId = model?.GenreId ?? 0,
            AuthorId = model?.AuthorId ?? 0,
            MaxPlayers = model?.MaxPlayers ?? 1,
            GenreOptions = genreOptions,
            PublisherOptions = publisherOptions
        };
    }

    private async Task<GameEditViewModel> BuildEditViewModelAsync(
        GameEditViewModel model,
        CancellationToken cancellationToken = default)
    {
        var publisherOptions = await gameService.ListPublisherOptionsAsync(cancellationToken);

        return new GameEditViewModel
        {
            Id = model.Id,
            Name = model.Name,
            PublisherId = model.PublisherId,
            GenreId = model.GenreId,
            AuthorId = model.AuthorId,
            TimesPlayed = model.TimesPlayed,
            MaxPlayers = model.MaxPlayers,
            IsActive = model.IsActive,
            PublisherOptions = publisherOptions
        };
    }
}
