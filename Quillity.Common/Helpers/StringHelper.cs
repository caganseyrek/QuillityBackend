namespace Quillity.Common.Helpers;

public static class StringHelper
{
    public static string Capitalize(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        if (input.Length == 1)
            return input.ToUpper();

        return char.ToUpper(input[0]) + input.Substring(1).ToLower();
    }

    public static bool IsEqualIgnoreCase(string? input, string? compareTo)
    {
        if (input is null && compareTo is null)
            return true;

        if (input is null || compareTo is null)
            return false;
        
        return string.Equals(input, compareTo, StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsEqualIgnoreCase(Guid? input, Guid? compareTo)
    {
        return IsEqualIgnoreCase(input?.ToString(), compareTo?.ToString());
    }

    public static string? NullIfWhiteSpace(string? input)
    {
        return string.IsNullOrWhiteSpace(input) ? null : input;
    }

    public static string GetInitialLetters(string userName)
    {
        var words = userName.Split(" ");

        var firstWordInitial = Capitalize(words.ElementAt(0).Substring(0, 1));
        var lastWordInitial = Capitalize(words.ElementAt(words.Length - 1).Substring(0, 1));

        return string.Concat(firstWordInitial, lastWordInitial);
    }
}
