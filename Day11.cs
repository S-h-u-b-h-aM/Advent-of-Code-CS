namespace Advent_of_Code;

public static class Day11
{
    public static void Run(string inputText)
    {
        IEnumerable<long> numbers = new StringReader(inputText).ReadNumbers();
        long count25 = numbers.CountDescendents(25);
        long count75 = numbers.CountDescendents(75);
        
        Console.WriteLine($"Count after 25 iterations: {count25}");
        Console.WriteLine($"Count after 75 iterations: {count75}");
    }
    private static Dictionary<(long number, int iterations), long> Cache { get; } = new(); 
    private static long CountDescendents(this IEnumerable<long> numbers, int iterations) =>
        numbers.Sum(number => number.CountDescendents(iterations));

    private static long CountDescendents(this long number, int iterations) =>
        Cache.TryGetValue((number, iterations), out long count)
            ? count
            : Cache[(number, iterations)] = number.FullCountDescendents(iterations);
    private static long FullCountDescendents(this long number, int iterations) =>
        iterations is 0 ? 1 : number.Expand().CountDescendents(iterations - 1);

    private static long[] Expand(this long number) =>
        number is 0 ? [1]
        : number.DigitsCount() is int count && count % 2 == 0 ? number.Split((count / 2).Power10())
        : [number * 2024];

    private static long[] Split(this long number, long divisor) =>
        [number / divisor, number % divisor];
    private static long Power10(this int n) => n is 0 ? 1 : 10 * (n - 1).Power10();
    private static int DigitsCount(this long number) => number < 10 ? 1 : 1 + (number / 10).DigitsCount();
    private static IEnumerable<long> ReadNumbers(this TextReader text) =>
        (text.ReadLine() ?? string.Empty).ParseLongsNoSign();
}