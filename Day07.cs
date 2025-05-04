namespace Advent_of_Code;

public static class Day07
{
    public static void Run(string inputText)
    {
        List<Equation> equations = new StringReader(inputText).ReadEquation().ToList();
        long simpleCalibration = equations.Where( eq => eq.CanProduceResult(Addition, Multiplicaiton))
            .Sum(eq => eq.Result);
        long extendedCalibration = equations.Where( eq => eq.CanProduceResult(Addition, Multiplicaiton, Concatination))
            .Sum(eq => eq.Result);
        
        Console.WriteLine($"  Simple Calibrated Result: {simpleCalibration}");
        Console.WriteLine($"Extended Calibrated Result: {extendedCalibration}");
    }

    private static bool CanProduceResult(this Equation equation, params Operator[] operators)
    {
        HashSet<long> produced = [equation.Values[0]];

        equation.Values[1..].ForEach(value =>
        {
            var expanded = operators.SelectMany(op => op(equation, produced, value));
            produced = [..expanded];
            // IEnumerable<long> addition = produced
            //     .Where(x => equation.Result - x >= value)
            //     .Select(x => value + x);
            // IEnumerable<long> multiplication = produced
            //     .Where(x => equation.Result / x >= value)
            //     .Select(x => value * x);
            // produced = [ ..addition, ..multiplication ];
        });
        
        
        return produced.Contains(equation.Result);
    }
    private delegate IEnumerable<long> Operator(Equation equation, IEnumerable<long> produced, long next );
    private static IEnumerable<long> Addition(this Equation equation, IEnumerable<long> produced, long next) =>
        produced.Where(x => equation.Result - x >= next).Select(x => x + next);
    
    private static IEnumerable<long> Multiplicaiton(this Equation equation, IEnumerable<long> produced, long next) =>
        produced.Where(x => equation.Result / x >= next).Select(x => x * next);

    private static IEnumerable<long> Concatination(this Equation equation, IEnumerable<long> produced, long next)
    {
        string nextString = next.ToString();
        foreach (long x in produced)
        {
            string concatenated = x.ToString() + nextString;
            if (long.TryParse(concatenated, out long value) && value <= equation.Result) yield return value;
        }
    }

    private static IEnumerable<Equation> ReadEquation(this TextReader textReader) =>
        textReader.ReadLines()
            .Select(Common.ParseLongsNoSign)
            .Select(values => new Equation(values[0], values[1..]));
    private record Equation(long Result, List<long> Values);
}