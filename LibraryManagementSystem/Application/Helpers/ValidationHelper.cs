namespace Application.Helpers;

public class ValidationHelper
{
    public static bool IsValidString(string input, int minLength = 2)
    {
        return !string.IsNullOrWhiteSpace(input) && input.Trim().Length >= minLength;
    }

    public static bool IsValidIsbn(string isbn)
    {
        if (string.IsNullOrWhiteSpace(isbn)) return false;
        string cleanIsbn = isbn.Replace("-", "").Replace(" ", "").Trim();
        return cleanIsbn.Length == 10 || cleanIsbn.Length == 13;
    }
}