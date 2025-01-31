using System.Text;

namespace Advent_of_Code;

public static class Day05
{
    public static void Run(string inputText)
    {
        var textReader = new StringReader(inputText);
        var sortOrder = textReader.ReadSortOrder().ToHashSet();
        IComparer<int> comparer = Comparer<int>.Create((a,b) => 
            sortOrder.Contains((a,b)) ? -1 
                : sortOrder.Contains((b,a)) ? 1
                : 0);
        List<List<int>> allPrints = textReader.ReadPrinedPages().ToList();

        var middlePageSum = allPrints
            .Where(pages => pages.IsSorted(comparer))
            .Sum(pages => pages.MiddlePage());

        var correctMiddlePageSum = allPrints
            .Where(pages => !pages.IsSorted(comparer))
            .Sum(MiddlePage);  //.Sum(pages => pages.MiddlePage());
        
        Console.WriteLine($"        Middle pages sum: {middlePageSum}");
        Console.WriteLine($"Correct Middle pages sum: {correctMiddlePageSum}");
    }
    private static int MiddlePage(this List<int> pages) => pages[pages.Count / 2];

    private static int MiddlePage(this IEnumerable<int> pages)
    {
        using var half = pages.GetEnumerator();
        using var full = pages.GetEnumerator();

        while (full.MoveNext() && half.MoveNext() && full.MoveNext())
        {
        }
        return half.Current;
    }
    private static bool IsSorted(this List<int> pages, IComparer<int> comparer) => 
        pages.SelectMany((prev, index) => pages[(index+1)..].Select(next => (prev, next)))
            .All(pair => comparer.Compare(pair.prev, pair.next) <= 0);

    private static IEnumerable<(int before, int after)> ReadSortOrder(this TextReader text) =>
        text.ReadLines().TakeWhile(line => !string.IsNullOrWhiteSpace(line)).Select(ToSortOrder);

    private static (int before, int after) ToSortOrder(this string line)
    {
        var parts = line.Split("|");
        return (int.Parse(parts[0]), int.Parse(parts[1]));
    }

    private static IEnumerable<List<int>> ReadPrinedPages(this TextReader text) =>
        text.ReadLines().Select(Common.ParseIntsNoSign);
}