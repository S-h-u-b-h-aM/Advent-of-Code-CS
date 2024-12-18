using System.Xml.Xsl;

namespace Advent_of_Code;

public static class Day01
{
    public static void Execute()
    {
        (List<int> left, List<int> right) = Console.In.ReadLists();

        int totalDistance = left.Order()
            .Zip(right.Order(), (x, y) => Math.Abs(x - y))
            .Sum();
        Console.WriteLine(totalDistance);
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