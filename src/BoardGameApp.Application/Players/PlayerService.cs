using BoardGameApp.Application.Common;

namespace BoardGameApp.Application.Players;

public sealed class PlayerService : IPlayerService
{
    private const string BrazilianWhatsAppFormatPattern = @"^\(\d{2}\) (?:\d{4}-\d{4}|9 \d{4}-\d{4})$";

    private readonly IPlayerRepository playerRepository;

    public PlayerService(IPlayerRepository playerRepository)
    {
        this.playerRepository = playerRepository;
    }

    public async Task<PlayerViewDto> CreateAsync(
        CreatePlayerDto dto,
        CancellationToken cancellationToken = default)
    {
        EnsureBrazilianWhatsAppFormat(dto.WhatsApp);
        await EnsureUniqueFullNameAsync(dto.FullName, excludingId: null, cancellationToken);
        await EnsureUniqueWhatsAppAsync(dto.WhatsApp, excludingId: null, cancellationToken);

        var player = await playerRepository.CreateAsync(dto.ToEntity(), cancellationToken);

        return PlayerViewDto.FromEntity(player);
    }

    public async Task<PlayerViewDto> UpdateAsync(
        UpdatePlayerDto dto,
        CancellationToken cancellationToken = default)
    {
        var player = await playerRepository.GetByIdAsync(dto.Id, cancellationToken)
            ?? throw new InvalidOperationException($"Player with id '{dto.Id}' was not found.");

        EnsureBrazilianWhatsAppFormat(dto.WhatsApp);
        await EnsureUniqueFullNameAsync(dto.FullName, dto.Id, cancellationToken);
        await EnsureUniqueWhatsAppAsync(dto.WhatsApp, dto.Id, cancellationToken);

        dto.ApplyTo(player);
        await playerRepository.UpdateAsync(player, cancellationToken);

        return PlayerViewDto.FromEntity(player);
    }

    public async Task DeactivateAsync(int id, CancellationToken cancellationToken = default)
    {
        var player = await playerRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new InvalidOperationException($"Player with id '{id}' was not found.");

        player.IsActive = false;
        await playerRepository.UpdateAsync(player, cancellationToken);
    }

    public async Task<PlayerViewDto?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var player = await playerRepository.GetByIdAsync(id, cancellationToken);

        return player is null ? null : PlayerViewDto.FromEntity(player);
    }

    public async Task<PagedResult<PlayerViewDto>> ListAsync(
        PlayerFilter filter,
        int page = 1,
        bool includeInactive = false,
        CancellationToken cancellationToken = default)
    {
        var players = await playerRepository.ListAsync(
            filter,
            page,
            includeInactive,
            cancellationToken);
        var items = players.Items
            .Select(PlayerViewDto.FromEntity)
            .ToList();

        return new PagedResult<PlayerViewDto>(
            items,
            players.Page,
            players.PageSize,
            players.TotalCount);
    }

    private async Task EnsureUniqueFullNameAsync(
        string fullName,
        int? excludingId,
        CancellationToken cancellationToken)
    {
        if (await playerRepository.ExistsByFullNameAsync(fullName, excludingId, cancellationToken))
        {
            throw new InvalidOperationException($"Player full name '{fullName}' is already in use.");
        }
    }

    private async Task EnsureUniqueWhatsAppAsync(
        string whatsApp,
        int? excludingId,
        CancellationToken cancellationToken)
    {
        if (await playerRepository.ExistsByWhatsAppAsync(whatsApp, excludingId, cancellationToken))
        {
            throw new InvalidOperationException($"Player WhatsApp '{whatsApp}' is already in use.");
        }
    }

    private static void EnsureBrazilianWhatsAppFormat(string whatsApp)
    {
        if (!System.Text.RegularExpressions.Regex.IsMatch(whatsApp, BrazilianWhatsAppFormatPattern))
        {
            throw new InvalidOperationException("Player WhatsApp must use the Brazilian format '(32) 1111-1111' or '(32) 9 1111-1111'.");
        }
    }
}
