using System.Text.RegularExpressions;

namespace Quillity.Common.Helpers;

public static partial class ValidationHelper
{
    [GeneratedRegex("^#(?:[0-9a-fA-F]{3}|[0-9a-fA-F]{6})$", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex HexColorRegex();
    private static readonly Regex _hexColorRegex = HexColorRegex();

    public static bool IsValidHexColor(string colorCode)
    {
        if (string.IsNullOrWhiteSpace(colorCode))
            return false;

        return _hexColorRegex.IsMatch(colorCode);
    }

    public static bool IsValidURL(string uri)
    {
        return Uri.TryCreate(uri, UriKind.Absolute, out var parsedUri) && (parsedUri.Scheme == Uri.UriSchemeHttp || parsedUri.Scheme == Uri.UriSchemeHttps);
    }

    public static bool IsValidOrNonEmptyGuid(string guid)
    {
        return Guid.TryParse(guid, out Guid parsedGuid) && parsedGuid != Guid.Empty;
    }

    public static bool IsValidOrNonEmptyGuid(Guid guid) => IsValidOrNonEmptyGuid(guid.ToString());
}
