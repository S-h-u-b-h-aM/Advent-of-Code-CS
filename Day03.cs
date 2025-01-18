using System.Text.RegularExpressions;

namespace Advent_of_Code;

static class Day03
{
    public static void Run()
    {
        var text = Console.In.ReadLines();
        var instructions = text.SelectMany(Parse).ToList();
        var sum = instructions.OfType<Multiply>().SumProducts();    // Part 1 of Day 3
        var enablingSum = instructions.SumProducts();
        
        Console.WriteLine($"  Simple sum: {sum}");
        Console.WriteLine($"Enabling sum: {enablingSum}");
    }

    private static int SumProducts(this IEnumerable<Instruction> instructions) =>
        instructions.Aggregate(
            (sum: 0, include: true),
            (acc, instruction) => instruction switch
            {
                Pause => (sum: acc.sum, include: false),
                Resume => (sum: acc.sum, include: true),
                Multiply mul when acc.include => (sum: acc.sum + mul.a * mul.b, include: acc.include),
                _ => acc
            }
        ).sum;
    private static IEnumerable<(int a, int b)> Parse1(this string line) =>
        Regex.Matches(line, @"mul\((?<a>\d+),(?<b>\d+)\)")    // MatchCollection : IEnumerable<Match>
            .Select(m => (int.Parse(m.Groups["a"].Value), int.Parse(m.Groups["b"].Value)));
    
    private static IEnumerable<Instruction> Parse(this string line) =>
        Regex.Matches(line, @"(?<mul>mul)\((?<a>\d+),(?<b>\d+)\)|(?<dont>don't)(\))|(?<do>do)\(\)")    // MatchCollection : IEnumerable<Match>
            .Select(match => match switch
                {
                    _ when match.Groups["dont"].Success => (Instruction)new Pause(),
                    _ when match.Groups["do"].Success => new Resume(),
                    _ => new Multiply(int.Parse(match.Groups["a"].Value), int.Parse(match.Groups["b"].Value))
                });
    
}
abstract record Instruction;
record Multiply(int a, int b) : Instruction;
record Pause : Instruction;
record Resume : Instruction;