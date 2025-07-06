namespace Advent_of_Code;

public static class Day19
{
    public static void Run(StringReader In)
    {
        var towels = In.ReadTowels().ToArray();
        var patterns = In.ReadPatterns().ToList();
        int possibleCount = patterns.Count(pattern => pattern.CanCreate(towels));
        Console.WriteLine($"Possible patterns: {possibleCount}");
    }
    static bool CanCreate(this string pattern, string[] towels)
    {
        bool[] possible = new bool[pattern.Length + 1];
        possible[0] = true;
        for (int k = 0; k < pattern.Length; k++)
        {
            if (!possible[k]) continue;
            string remainingPattern = pattern[k..]; //.Substring(k);
            foreach (string towel in towels.Where(remainingPattern.StartsWith))
            {
                possible[k + towel.Length] = true;
            }
        }
        return possible[pattern.Length];
    }
    static IEnumerable<string> ReadPatterns(this TextReader text) =>
        text.ReadLines().Where(line => !string.IsNullOrWhiteSpace(line));
    static IEnumerable<string> ReadTowels(this TextReader text) =>
        text.ReadLine()?.Split(", ", StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();
    
}