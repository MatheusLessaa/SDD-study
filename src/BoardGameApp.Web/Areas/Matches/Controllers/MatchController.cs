using BoardGameApp.Application.Games;
using BoardGameApp.Application.Matches;
using BoardGameApp.Application.Players;
using BoardGameApp.Web.Areas.Matches.Models;
using Microsoft.AspNetCore.Mvc;

namespace BoardGameApp.Web.Areas.Matches.Controllers;

[Area("Matches")]
public sealed class MatchController : Controller
{
    private readonly IMatchService matchService;
    private readonly IGameService gameService;
    private readonly IPlayerService playerService;

    public MatchController(
        IMatchService matchService,
        IGameService gameService,
        IPlayerService playerService)
    {
        this.matchService = matchService;
        this.gameService = gameService;
        this.playerService = playerService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(
        int? id,
        string? gameName,
        int page = 1,
        CancellationToken cancellationToken = default)
    {
        SetMatchesNavigation("Matches");

        var matches = await matchService.ListAsync(
            new MatchFilter(id, gameName),
            page,
            cancellationToken);

        return View(new MatchIndexViewModel
        {
            Matches = matches,
            Id = id,
            GameName = gameName
        });
    }

    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken cancellationToken = default)
    {
        SetMatchesNavigation("Create Match");

        return View(await BuildCreateViewModelAsync(new MatchCreateViewModel(), cancellationToken));
    }

    [HttpGet]
    public async Task<IActionResult> Details(
        int id,
        CancellationToken cancellationToken = default)
    {
        SetMatchesNavigation("Match Details");

        var match = await matchService.GetByIdAsync(id, cancellationToken);

        if (match is null)
        {
            return NotFound();
        }

        return PartialView("_Details", match);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        MatchCreateViewModel model,
        CancellationToken cancellationToken = default)
    {
        SetMatchesNavigation("Create Match");

        if (!ModelState.IsValid)
        {
            return View(await BuildCreateViewModelAsync(model, cancellationToken));
        }

        try
        {
            await matchService.CreateAsync(model.ToDto(), cancellationToken);
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
        SetMatchesNavigation("Edit Match Scores");

        var match = await matchService.GetByIdAsync(id, cancellationToken);

        if (match is null)
        {
            return NotFound();
        }

        return View(new MatchEditScoresViewModel
        {
            Match = match
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        UpdateMatchScoresDto dto,
        CancellationToken cancellationToken = default)
    {
        SetMatchesNavigation("Edit Match Scores");

        if (!ModelState.IsValid)
        {
            return View(dto);
        }

        try
        {
            await matchService.UpdateScoresAsync(dto, cancellationToken);
        }
        catch (InvalidOperationException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            return View(dto);
        }

        return RedirectToAction(nameof(Index));
    }

    private void SetMatchesNavigation(string title)
    {
        ViewData["Title"] = title;
        ViewData["ActiveNav"] = "Matches";
    }

    private async Task<MatchCreateViewModel> BuildCreateViewModelAsync(
        MatchCreateViewModel model,
        CancellationToken cancellationToken)
    {
        var games = await gameService.ListAsync(
            new GameFilter(),
            page: 1,
            includeInactive: false,
            cancellationToken);
        var players = await playerService.ListAsync(
            new PlayerFilter(),
            page: 1,
            includeInactive: false,
            cancellationToken);

        return new MatchCreateViewModel
        {
            GameId = model.GameId,
            PlayerIds = model.PlayerIds,
            Scores = model.Scores,
            WinnerPlayerId = model.WinnerPlayerId,
            Games = games.Items,
            Players = players.Items
        };
    }
}
