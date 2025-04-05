using ErrorOr;
using System.Security.Claims;
using System.Text.Json;

namespace Conductor.Api.Common;

public sealed class AccessToken
{
    public const string KeeperKind = "keeper";

    public const string RegularKind = "regular";

    public const string ObserverKind = "observer";

    public static readonly string[] Kinds = [KeeperKind, RegularKind, ObserverKind];

    public required Dictionary<Guid, string> Cabins { get; init; }

    private AccessToken()
    {
    }

    public required Guid UserId { get; init; }

    public required string Kind { get; init; }

    public static ErrorOr<AccessToken> CreateFrom(ClaimsPrincipal principal)
    {
        var nameIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier);

        if (nameIdClaim is null)
        {
            return Error.Validation("Jwt does not contains name identifier");
        }

        if (!Guid.TryParse(nameIdClaim.Value, out var userId))
        {
            return Error.Validation($"Bad name identifier format: {nameIdClaim.Value}");
        }

        var kind = principal.FindFirstValue("kind");
        if (kind is null || !Kinds.Contains(kind))
        {
            return Error.Validation("Jwt does not contains kind claim");
        }

        var cabinsClaim = principal.FindFirstValue("cabins");

        if (cabinsClaim is null)
        {
            return new AccessToken
            {
                UserId = userId,
                Kind = kind,
                Cabins = new()
            };
        }

        var cabinsDictionary = JsonSerializer.Deserialize<Dictionary<Guid, string>>(cabinsClaim);
        if (cabinsDictionary is null)
        {
            return new AccessToken
            {
                UserId = userId,
                Kind = kind,
                Cabins = new()
            };
        }

        var token = new AccessToken
        {
            UserId = userId,
            Kind = kind,
            Cabins = cabinsDictionary
        };

        return token;
    }

    public bool IsOwnerOf(Guid cabinId)
    {
        if (Cabins.TryGetValue(cabinId, out var role))
        {
            return role == "owner";
        }

        return false;
    }

    public bool IsOwnerOrTeamOf(Guid cabinId)
    {
        if (Cabins.TryGetValue(cabinId, out var role))
        {
            return role == "owner" || role == "team";
        }

        return false;
    }

    public bool HasRoleIn(Guid cabinId)
    {
        return Cabins.TryGetValue(cabinId, out _);
    }
}
