namespace Advent_of_Code;

public static class Day15
{
    public static void Run(string inputText)
    {
        var input = new StringReader(inputText);
        State state = input.ReadState();
        List<Step> steps = input.ReadSteps().ToList();

        int totalGps = state.Apply(steps).Boxes.Sum(GetGPS);
        Console.WriteLine($"Total GPS: {totalGps}");
    }
    private static int GetGPS(this Point point) => point.Row * 100 + point.Column;  
    private static State Apply(this State state, IEnumerable<Step> steps) =>
        steps.Aggregate(state, Apply);
    private static State Apply(this State state, Step step) =>
        state.GetMove(step).ApplyTo(state);
    private static Point Apply(this Point point, Step step) =>
        new(point.Row + step.RowDiff, point.Column + step.ColumnDiff);
    private static IEnumerable<Point> Apply(this IEnumerable<Point> points, Step step) =>
        points.Select(p => p.Apply(step));

    private static State ApplyTo(this (Step step, IEnumerable<Point> boxes) move, State state) =>
        new(state.Robot.Apply(move.step),
            state.Boxes.Except(move.boxes).Union(move.boxes.Apply(move.step)).ToHashSet(),
            state.Walls);

    private static (Step step, IEnumerable<Point> boxes) GetMove(this State state, Step step)
    {
        List<Point> boxesToMove = new();
        Point position = state.Robot.Apply(step);
        while (state.Boxes.Contains(position))
        {
            boxesToMove.Add(position);
            position = position.Apply(step);
        }
        if (state.Walls.Contains(position)) return (new Step(0, 0), Enumerable.Empty<Point>());
        return (step, boxesToMove);
    }
    private static State ReadState(this TextReader input) => input.ReadMap().ToState();

    private static State ToState(this char[][] map) =>
        new(map.FindRobot(),
            map.FindBoxes().ToHashSet(),
            map.FindWalls().ToHashSet());
    private static Point FindRobot(this char[][] map) =>
        map.PositionOf(_robot).Single();
    private static IEnumerable<Point> FindBoxes(this char[][] map) =>
        map.PositionOf(_box);
    private static IEnumerable<Point> FindWalls(this char[][] map) =>
        map.PositionOf(_wall);
    private static IEnumerable<Point> PositionOf(this char[][] map, char c) =>
        from row in Enumerable.Range(0, map.Length)
        from col in Enumerable.Range(0, map[row].Length)
        where map[row][col] == c
        select new Point(row, col);

    private static IEnumerable<Step> ReadSteps(this TextReader input) =>
        input.ReadLines().SelectMany(line => line.ToCharArray()).Select(ToStep);

    private static Step ToStep(this char arrow) => arrow switch
    {
        '>' => new(0, 1),
        '<' => new(0, -1),
        '^' => new(-1, 0),
        _ => new(1, 0)
    };
    private static char[][] ReadMap(this TextReader input) =>
        input.ReadLines()
            .TakeWhile(line => !string.IsNullOrEmpty(line))
            .Select(line => line.ToCharArray())
            .ToArray();
    
    record State(Point Robot, HashSet<Point> Boxes, HashSet<Point> Walls);
    record struct Step(int RowDiff, int ColumnDiff);
    record struct Point(int Row, int Column);

    private readonly static char _box = 'O';
    private readonly static char _wall = '#';
    private readonly static char _robot = '@';
}