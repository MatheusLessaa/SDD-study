using BoardGameApp.Application.Authors;
using BoardGameApp.Application.Common;
using BoardGameApp.Web.Areas.Authors.Controllers;
using BoardGameApp.Web.Areas.Authors.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace BoardGameApp.Tests;

public class AuthorControllerTests
{
    [Fact]
    public async Task Index_lists_authors_with_filters()
    {
        var service = new FakeAuthorService();
        var controller = CreateController(service);

        var result = await controller.Index("Kiesling", page: 2);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<AuthorIndexViewModel>(viewResult.Model);
        Assert.Equal("Kiesling", model.Name);
        Assert.Equal(new AuthorFilter("Kiesling"), service.LastFilter);
        Assert.Equal(2, service.LastPage);
        Assert.Equal("Authors", controller.ViewData["ActiveNav"]);
    }

    [Fact]
    public async Task Create_post_redirects_to_index_when_author_is_created()
    {
        var service = new FakeAuthorService();
        var controller = CreateController(service);

        var result = await controller.Create(new CreateAuthorDto("Reiner Knizia"));

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(AuthorController.Index), redirect.ActionName);
        Assert.Equal("Reiner Knizia", service.CreatedAuthor?.Name);
    }

    [Fact]
    public async Task Details_returns_not_found_when_author_does_not_exist()
    {
        var service = new FakeAuthorService { AuthorToReturn = null };
        var controller = CreateController(service);

        var result = await controller.Details(99);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Edit_post_redirects_to_index_when_author_is_updated()
    {
        var service = new FakeAuthorService();
        var controller = CreateController(service);
        var dto = new UpdateAuthorDto(1, "Updated Author");

        var result = await controller.Edit(dto);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(AuthorController.Index), redirect.ActionName);
        Assert.Equal(dto, service.UpdatedAuthor);
    }

    [Fact]
    public async Task Delete_redirects_to_index_when_author_is_deleted()
    {
        var service = new FakeAuthorService();
        var controller = CreateController(service);

        var result = await controller.Delete(7);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(AuthorController.Index), redirect.ActionName);
        Assert.Equal(7, service.DeletedId);
    }

    [Fact]
    public async Task Delete_preserves_service_error_in_temp_data()
    {
        var service = new FakeAuthorService { DeleteException = new InvalidOperationException("Author is used.") };
        var controller = CreateController(service);

        var result = await controller.Delete(7);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(AuthorController.Index), redirect.ActionName);
        Assert.Equal("Author is used.", controller.TempData["StatusMessage"]);
    }

    private static AuthorController CreateController(FakeAuthorService service)
    {
        return new AuthorController(service)
        {
            TempData = new TempDataDictionary(
                new Microsoft.AspNetCore.Http.DefaultHttpContext(),
                new FakeTempDataProvider())
        };
    }

    private sealed class FakeAuthorService : IAuthorService
    {
        public CreateAuthorDto? CreatedAuthor { get; private set; }

        public UpdateAuthorDto? UpdatedAuthor { get; private set; }

        public int? DeletedId { get; private set; }

        public AuthorFilter? LastFilter { get; private set; }

        public int? LastPage { get; private set; }

        public AuthorViewDto? AuthorToReturn { get; init; } = new(1, "Reiner Knizia");

        public InvalidOperationException? DeleteException { get; init; }

        public Task<AuthorViewDto> CreateAsync(
            CreateAuthorDto dto,
            CancellationToken cancellationToken = default)
        {
            CreatedAuthor = dto;

            return Task.FromResult(new AuthorViewDto(1, dto.Name));
        }

        public Task<AuthorViewDto> UpdateAsync(
            UpdateAuthorDto dto,
            CancellationToken cancellationToken = default)
        {
            UpdatedAuthor = dto;

            return Task.FromResult(new AuthorViewDto(dto.Id, dto.Name));
        }

        public Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            DeletedId = id;

            if (DeleteException is not null)
            {
                throw DeleteException;
            }

            return Task.CompletedTask;
        }

        public Task<AuthorViewDto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(AuthorToReturn);
        }

        public Task<PagedResult<AuthorViewDto>> ListAsync(
            AuthorFilter filter,
            int page = 1,
            CancellationToken cancellationToken = default)
        {
            LastFilter = filter;
            LastPage = page;

            return Task.FromResult(new PagedResult<AuthorViewDto>(
                [new AuthorViewDto(1, "Reiner Knizia")],
                page,
                20,
                1));
        }
    }

    private sealed class FakeTempDataProvider : ITempDataProvider
    {
        public IDictionary<string, object> LoadTempData(Microsoft.AspNetCore.Http.HttpContext context)
        {
            return new Dictionary<string, object>();
        }

        public void SaveTempData(
            Microsoft.AspNetCore.Http.HttpContext context,
            IDictionary<string, object> values)
        {
        }
    }
}
