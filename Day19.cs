namespace Advent_of_Code;

public static class Day19
{
    public static void Run(StringReader In)
    {
        var towels = In.ReadTowels().ToArray();
        var patterns = In.ReadPatterns().ToList();
        int possibleCount = patterns.Count(pattern => pattern.CountArrangements(towels) > 0);
        long totalCount = patterns.Sum(pattern => pattern.CountArrangements(towels));
        Console.WriteLine($"Possible patterns: {possibleCount}");
        Console.WriteLine($"   Total patterns: {totalCount}");
    }
    static long CountArrangements(this string pattern, string[] towels)
    {
        long[] counts = new long[pattern.Length + 1];
        counts[0] = 1;
        for (int k = 0; k < pattern.Length; k++)
        {
            if (counts[k] == 0) continue;
            string remainingPattern = pattern[k..]; //.Substring(k);
            foreach (string towel in towels.Where(remainingPattern.StartsWith))
            {
                counts[k + towel.Length] += counts[k];
            }
        }
        return counts[pattern.Length];
    }
    static IEnumerable<string> ReadPatterns(this TextReader text) =>
        text.ReadLines().Where(line => !string.IsNullOrWhiteSpace(line));
    static IEnumerable<string> ReadTowels(this TextReader text) =>
        text.ReadLine()?.Split(", ", StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();
    
}