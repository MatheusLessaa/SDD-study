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

        return View(new GameIndexViewModel
        {
            Games = games,
            GenreOptions = genreOptions,
            Id = id,
            Name = name,
            Author = author,
            GenreId = genreId,
            PublisherId = publisherId,
            IncludeInactive = includeInactive
        });
    }

    [HttpGet]
    public IActionResult Create()
    {
        SetGamesNavigation("Create Game");

        return View(new CreateGameDto(string.Empty, 0, 0, 0, 1));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        CreateGameDto dto,
        CancellationToken cancellationToken = default)
    {
        SetGamesNavigation("Create Game");

        if (!ModelState.IsValid)
        {
            return View(dto);
        }

        try
        {
            await gameService.CreateAsync(dto, cancellationToken);
        }
        catch (InvalidOperationException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            return View(dto);
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

        return View(new UpdateGameDto(
            game.Id,
            game.Name,
            game.PublisherId,
            game.GenreId,
            game.AuthorId,
            game.TimesPlayed,
            game.MaxPlayers,
            game.IsActive));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        UpdateGameDto dto,
        CancellationToken cancellationToken = default)
    {
        SetGamesNavigation("Edit Game");

        if (!ModelState.IsValid)
        {
            return View(dto);
        }

        try
        {
            await gameService.UpdateAsync(dto, cancellationToken);
        }
        catch (InvalidOperationException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            return View(dto);
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
}
