using BoardGameApp.Application.Authors;
using BoardGameApp.Web.Areas.Authors.Models;
using Microsoft.AspNetCore.Mvc;

namespace BoardGameApp.Web.Areas.Authors.Controllers;

[Area("Authors")]
public sealed class AuthorController : Controller
{
    private readonly IAuthorService authorService;

    public AuthorController(IAuthorService authorService)
    {
        this.authorService = authorService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(
        string? name,
        int page = 1,
        CancellationToken cancellationToken = default)
    {
        SetAuthorsNavigation("Authors");

        var authors = await authorService.ListAsync(
            new AuthorFilter(name),
            page,
            cancellationToken);

        return View(new AuthorIndexViewModel
        {
            Authors = authors,
            Name = name,
            StatusMessage = TempData["StatusMessage"]?.ToString()
        });
    }

    [HttpGet]
    public IActionResult Create()
    {
        SetAuthorsNavigation("Create Author");

        return View(new CreateAuthorDto(string.Empty));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        CreateAuthorDto dto,
        CancellationToken cancellationToken = default)
    {
        SetAuthorsNavigation("Create Author");

        if (!ModelState.IsValid)
        {
            return View(dto);
        }

        try
        {
            await authorService.CreateAsync(dto, cancellationToken);
        }
        catch (InvalidOperationException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            return View(dto);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Details(
        int id,
        CancellationToken cancellationToken = default)
    {
        SetAuthorsNavigation("Author Details");

        var author = await authorService.GetByIdAsync(id, cancellationToken);

        if (author is null)
        {
            return NotFound();
        }

        return View(author);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(
        int id,
        CancellationToken cancellationToken = default)
    {
        SetAuthorsNavigation("Edit Author");

        var author = await authorService.GetByIdAsync(id, cancellationToken);

        if (author is null)
        {
            return NotFound();
        }

        return View(new UpdateAuthorDto(author.Id, author.Name));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        UpdateAuthorDto dto,
        CancellationToken cancellationToken = default)
    {
        SetAuthorsNavigation("Edit Author");

        if (!ModelState.IsValid)
        {
            return View(dto);
        }

        try
        {
            await authorService.UpdateAsync(dto, cancellationToken);
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
    public async Task<IActionResult> Delete(
        int id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await authorService.DeleteAsync(id, cancellationToken);
        }
        catch (InvalidOperationException exception)
        {
            TempData["StatusMessage"] = exception.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    private void SetAuthorsNavigation(string title)
    {
        ViewData["Title"] = title;
        ViewData["ActiveNav"] = "Authors";
    }
}
