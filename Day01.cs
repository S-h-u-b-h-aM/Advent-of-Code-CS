using System.Xml.Xsl;

namespace Advent_of_Code;

static class Day01
{
    public static void Run()
    {
        (List<int> left, List<int> right) = Console.In.ReadLists();

        int totalDistance = left.Order()
            .Zip(right.Order(), (x, y) => Math.Abs(x - y))
            .Sum();

        int similarityScore = right
            // .Where(x => left.Contains(x))       // O(n) * O(n) = O(n^2)
            .Where(new HashSet<int>(left).Contains) // O(1) * O(n) = O(n)
            .Sum();
            // .GroupBy(x => x)
            // .Sum(group => group.Key * group.Count());
        Console.WriteLine(totalDistance);
        Console.WriteLine(similarityScore);
    }

    private static (List<int> left, List<int> right) ReadLists(this TextReader text)
    {
        (List<int> left, List<int> right) = (new(), new());
        while (text.ReadLine() is string line)
        {
            var values = line.ParseInts();
            left.Add(values[0]);
            right.Add(values[1]);
        }
        return (left, right);
    }
}