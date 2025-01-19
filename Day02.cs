using System.Dynamic;

namespace Advent_of_Code;

public static class Day02
{
    public static void Run(string inputText)
    {
        Console.WriteLine("Day 2");
        Console.WriteLine("Input test data:\n>>" + inputText);
        
        List<List<int>> allLists = new StringReader(inputText).LoadLists();
        int safeCount = allLists.Count(IsSafe);
        int tolerantSafeCount = allLists.Count(list => list.Expand().Any(IsSafe));
        
        Console.WriteLine($"Total count: {allLists.Count}");
        Console.WriteLine($"Total safe count: {safeCount}");
        Console.WriteLine($"Total tolerant safe count: {tolerantSafeCount}");
    }

    private static IEnumerable<List<int>> Expand(this List<int> values) =>
        new[] { values }.Concat(
            Enumerable.Range(0, values.Count).Select(values.ExceptAt));
    private static List<int> ExceptAt(this List<int> values, int index) =>
        values.Take(index).Concat(values.Skip(index + 1)).ToList();
    private static bool IsSafe(this List<int> values) =>
        values.Count < 2 || values.IsSafe(Math.Sign(values[1] - values[0]));
    private static bool IsSafe(this List<int> values, int diffSign) =>
        values.Pairs().All(pair => pair.IsSafe(diffSign));

    private static IEnumerable<(int prev, int next)> Pairs(this IEnumerable<int> values)
    {
        using var enumerator = values.GetEnumerator();
        if (!enumerator.MoveNext()) yield break;
        
        int prev = enumerator.Current;
        while (enumerator.MoveNext())
        {
            yield return (prev, enumerator.Current);
            prev = enumerator.Current;
        }
    }
    private static bool IsSafe(this (int prev, int next) pair, int diffSign) =>
        Math.Abs(pair.next - pair.prev) >= 1 &&
        Math.Abs(pair.next - pair.prev) <= 3 &&
        Math.Sign(pair.next - pair.prev) == diffSign;
    private static List<List<int>> LoadLists(this TextReader text) => 
        text.ReadLines().Select(Common.ParseIntsNoSign).ToList();
}