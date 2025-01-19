using System.Xml.Xsl;

namespace Advent_of_Code;

public static class Day01
{
    public static void Run(string inputText)
    {
        Console.WriteLine("Day 1");
        Console.WriteLine("Input test data:\n" + inputText);
        (List<int> left, List<int> right) = new StringReader(inputText).LoadLists();

        int totalDistance = left.Order()
            .Zip(right.Order(), (x, y) => Math.Abs(x - y))
            .Sum();

        int similarityScore = right
            // .Where(x => left.Contains(x))       // O(n) * O(n) = O(n^2)
            .Where(new HashSet<int>(left).Contains) // O(1) * O(n) = O(n)
            .Sum();
            // .GroupBy(x => x)
            // .Sum(group => group.Key * group.Count());
            
        Console.WriteLine("\n==============Output==============\n");
        Console.WriteLine(totalDistance);
        Console.WriteLine(similarityScore);
    }

    private static (List<int> left, List<int> right) ReadLists(this TextReader text)
    {
        (List<int> left, List<int> right) = (new(), new());
        while (text.ReadLine() is { } line)  // while (text.ReadLine() is string line)   
        {
            var values = line.ParseInts();
            left.Add(values[0]);
            right.Add(values[1]);
        }
        return (left, right);
    }

    private static (List<int> left, List<int> right) LoadLists(this TextReader text) =>
        text.ReadLines().Select(Common.ParseIntsNoSign).Transpose().ToPair();
}