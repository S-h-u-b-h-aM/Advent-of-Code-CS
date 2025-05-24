namespace Advent_of_Code;

public static class Day13
{
    private static readonly int _costAPresess = 3;
    private static readonly int _costBPresess = 1;
    private static readonly long _offset = 10000000000000 ;
    public static void Run(string inputText)
    {
        var machines = new StringReader(inputText).ReadMachines().ToList();
        long totalCost = machines.SelectMany(GetCheapestPlay).ToCost();
        long farCost = machines.Select(ToCorrectedMachine).SelectMany(GetCheapestPlay).ToCost();
        Console.WriteLine($"    Total cost: {totalCost}");
        Console.WriteLine($"Corrected cost: {farCost}");
    }
    private static Machine ToCorrectedMachine(this Machine machine) =>
        machine with { Prize = new(machine.Prize.X + _offset, machine.Prize.Y + _offset) };
    private static long ToCost(this IEnumerable<(long aPresses, long bPresses)> buttonPresses) =>
        buttonPresses.Sum(pair => pair.aPresses * _costAPresess + pair.bPresses * _costBPresess);
    private static IEnumerable<(long aPresses, long bPresses)> GetCheapestPlay(this Machine machine) => 
        machine.SolvePolynomial_CramerRule(out var cheapestPlay) 
            ? machine.ToButtonPresses(cheapestPlay.aPresses) 
            : Enumerable.Empty<(long, long)>();
    private static bool SolvePolynomial_CramerRule(this Machine machine, out (long aPresses, long bPresses) solution)
    {   /*
            a1 x + b1 y = c1
            a2 x + b2 y = c2
           ------------------
            D = a1*b2 - a2*b1
            x = (c1 * b2 - c2 * b1) / D     (D != 0)
            y = (c2 * a1 - c1 * a2) / D     (D != 0)
        */
        var (a, b, prize) = machine;
        long determinent = a.X * b.Y - a.Y * b.X;
        if (determinent == 0)
        {
            solution = (long.MinValue, long.MinValue); 
            return false;
        }
        long aPresses = (prize.X * b.Y - prize.Y * b.X) / determinent;
        long bPresses = (prize.Y * a.X - prize.X * a.Y) / determinent;
        solution = (aPresses, bPresses);
        return true;
    }
    private static IEnumerable<(long aPresses, long bPresses)> ToButtonPresses(this Machine machine, long aPresses)
    {
        long bPresses = (machine.Prize.X - aPresses * machine.A.X) / machine.B.X;
        if (aPresses * machine.A.X + bPresses * machine.B.X != machine.Prize.X) yield break;
        if (aPresses * machine.A.Y + bPresses * machine.B.Y != machine.Prize.Y) yield break;
        
        yield return (aPresses, bPresses);
    }
    private static long MaxAPresses(this Machine machine) => Math.Min(machine.Prize.X / machine.A.X, machine.Prize.Y / machine.A.Y);
    private static IEnumerable<Machine> ReadMachines(this TextReader textReader) =>
        textReader.ReadCoordinatesTriplets()
            .Select(coord => new Machine(coord[0], coord[1], coord[2]));
    private static IEnumerable<Coordinates[]> ReadCoordinatesTriplets(this TextReader textReader) => 
        textReader.ReadCoordinates()
            .Select((coord, index) => (coord, index))
            .GroupBy(tuple => tuple.index / 3)
            .Select(group => group.Select(tuple => tuple.coord).ToArray());
    private static IEnumerable<Coordinates> ReadCoordinates(this TextReader textReader) =>
        textReader.ReadLines()
            .Where(line => line.Length > 0)
            .Select(Common.ParseIntsNoSign)
            .Select(values => new Coordinates(values[0], values[1]));
    record struct Coordinates(long X, long Y);
    record struct Machine(Coordinates A, Coordinates B, Coordinates Prize);
}