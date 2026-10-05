namespace API;

public static class StringExtension
{
    public static bool EqualsCi(this string str, string secondStr) => str.Equals(secondStr, StringComparison.OrdinalIgnoreCase);
    public static string[] SplitCleanSpace(this string str, string delimiter = " ") => str.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
} 
