using BoardGameApp.Application.Common;
using BoardGameApp.Application.Players;
using BoardGameApp.Web.Areas.Players.Models;
using Microsoft.AspNetCore.Mvc;

namespace BoardGameApp.Web.Areas.Players.Controllers;

[Area("Players")]
public sealed class PlayerController : Controller
{
    private readonly IPlayerService playerService;

    public PlayerController(IPlayerService playerService)
    {
        this.playerService = playerService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(
        string? fullName,
        string? whatsApp,
        int page = 1,
        bool includeInactive = false,
        CancellationToken cancellationToken = default)
    {
        SetPlayersNavigation("Players");

        var players = await playerService.ListAsync(
            new PlayerFilter(fullName, whatsApp),
            page,
            includeInactive,
            cancellationToken);

        return View(new PlayerIndexViewModel
        {
            Players = players,
            FullName = fullName,
            WhatsApp = whatsApp,
            IncludeInactive = includeInactive
        });
    }

    [HttpGet]
    public IActionResult Create()
    {
        SetPlayersNavigation("Create Player");

        return View(new CreatePlayerDto(string.Empty, string.Empty));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        CreatePlayerDto dto,
        CancellationToken cancellationToken = default)
    {
        SetPlayersNavigation("Create Player");

        if (!ModelState.IsValid)
        {
            return View(dto);
        }

        try
        {
            await playerService.CreateAsync(dto, cancellationToken);
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
        SetPlayersNavigation("Edit Player");

        var player = await playerService.GetByIdAsync(id, cancellationToken);

        if (player is null)
        {
            return NotFound();
        }

        return View(new UpdatePlayerDto(
            player.Id,
            player.FullName,
            player.WhatsApp,
            player.IsActive));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        UpdatePlayerDto dto,
        CancellationToken cancellationToken = default)
    {
        SetPlayersNavigation("Edit Player");

        if (!ModelState.IsValid)
        {
            return View(dto);
        }

        try
        {
            await playerService.UpdateAsync(dto, cancellationToken);
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
            await playerService.DeactivateAsync(id, cancellationToken);
        }
        catch (InvalidOperationException)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    private void SetPlayersNavigation(string title)
    {
        ViewData["Title"] = title;
        ViewData["ActiveNav"] = "Players";
    }
}
