namespace Quillity.Common.Values;

public static class Constraints
{
    public static class Regex
    {
        public const string LetterOnly = "^[a-zA-Z]+$";
    }

    public const int URL_KEY_LENGTH = 8;
    public const int MIN_STRING_LENGTH = 8;

    public const int NAME_STRING_LENGTH = 96;
    public const int DESCRIPTION_STRING_LENGTH = 256;
}
