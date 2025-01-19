using System.Text.RegularExpressions;

namespace Advent_of_Code;

public static class Day03
{
    public static string Description
    {
        get => "--- Day 3: Mull It Over ---\n\"" +
               "Our computers are having issues, so I have no idea if we have any Chief Historians in stock! You're welcome to check the warehouse, " +
               "though,\" says the mildly flustered shopkeeper at the North Pole Toboggan Rental Shop. The Historians head out to take a look.\n" +
               "\nThe shopkeeper turns to you. \"Any chance you can see why our computers are having issues again?\"\n" +
               "\nThe computer appears to be trying to run a program, but its memory (your puzzle input) is corrupted. " +
               "All of the instructions have been jumbled up!\n\nIt seems like the goal of the program is just to multiply some numbers. " +
               "It does that with instructions like mul(X,Y), where X and Y are each 1-3 digit numbers. " +
               "For instance, mul(44,46) multiplies 44 by 46 to get a result of 2024. Similarly, mul(123,4) would multiply 123 by 4.\n" +
               "\nHowever, because the program's memory has been corrupted, there are also many invalid characters that should be ignored, even if they look like part of a mul instruction. " +
               "Sequences like mul(4*, mul(6,9!, ?(12,34), or mul ( 2 , 4 ) do nothing.\n" +
               "\nFor example, consider the following section of corrupted memory:\n" +
               "\nxmul(2,4)%&mul[3,7]!@^do_not_mul(5,5)+mul(32,64]then(mul(11,8)mul(8,5))\nOnly the four highlighted sections are real mul instructions. " +
               "Adding up the result of each instruction produces 161 (2*4 + 5*5 + 11*8 + 8*5).\n\nScan the corrupted memory for uncorrupted mul instructions. " +
               "What do you get if you add up all of the results of the multiplications?"; 
    }
    public static void Run(string inputText)
    {
        // var text = Console.In.ReadLines();
        Console.WriteLine("Day 3");
        Console.WriteLine("Input test data:\n" + inputText);
        
        var text = new StringReader(inputText).ReadLines();
        var instructions = text.SelectMany(Parse).ToList();
        var sum = instructions.OfType<Multiply>().SumProducts();    // Part 1 of Day 3
        var enablingSum = instructions.SumProducts();
        Console.WriteLine("\n==============Output==============\n");
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