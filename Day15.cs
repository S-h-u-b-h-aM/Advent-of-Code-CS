namespace Advent_of_Code;

public static class Day15
{
    public static void Run(string inputText)
    {
        var input = new StringReader(inputText);
        State state = input.ReadState();
        List<Step> steps = input.ReadSteps().ToList();

        int totalGps = state.Apply(steps).Boxes.Index.Values.Sum(GetGPS);
        int scaledGps = state.Scale().Apply(steps).Boxes.Index.Values.Distinct().Sum(GetGPS);
        Console.WriteLine($"  Total GPS: {totalGps}");
        Console.WriteLine($"Scaled GPS: {scaledGps}");
    }

    private static State Scale(this State state) =>
        new(state.Robot.ScaleRobot(),
            state.Boxes.Index.Values.Select(box => box.Position.ScaleBox()).ToBoxes(),
            state.Walls.SelectMany(ScaleWall).ToHashSet());
    private static Point ScaleRobot(this Point robot) =>
        new(robot.Row, robot.Column * 2);
    private static Box ScaleBox(this Point box) =>
        new(new(box.Row, box.Column * 2), 2);
    private static IEnumerable<Point> ScaleWall(this Point wall) =>
        [wall with { Column = wall.Column * 2 }, wall with { Column = wall.Column * 2 + 1 }];
    private static int GetGPS(this Box box) => box.Position.Row * 100 + box.Position.Column;  
    private static State Apply(this State state, IEnumerable<Step> steps) =>
        steps.Aggregate(state, Apply);
    private static State Apply(this State state, Step step) =>
        state.GetMove(step).ApplyTo(state);
    private static Point Apply(this Point point, Step step) =>
        new(point.Row + step.RowDiff, point.Column + step.ColumnDiff);
    private static IEnumerable<Box> Apply(this IEnumerable<Box> points, Step step) =>
        points.Select(box => box with { Position = box.Position.Apply(step) });
    private static State ApplyTo(this (Step step, IEnumerable<Box> boxes) move, State state) =>
        new(state.Robot.Apply(move.step),
            state.Boxes.Index.Values.Except(move.boxes).Concat(move.boxes.Apply(move.step)).ToBoxes(),
            state.Walls);

    private static (Step step, IEnumerable<Box> boxes) GetMove(this State state, Step step)
    {
        HashSet<Box> boxesToMove = [];
        List<Point> pushing = [state.Robot.Apply(step)];
        while (pushing.Count > 0)
        {
            if (pushing.Any(state.Walls.Contains)) return (new Step(0, 0), Enumerable.Empty<Box>());
            pushing = state.Boxes.GetBoxes(pushing)
                .Where(boxesToMove.Add)
                .SelectMany(ToPosition)
                .Select(pair => pair.position.Apply(step))
                .ToList();
        }
        return (step, boxesToMove);
    }
    private static State ReadState(this TextReader input) => input.ReadMap().ToState();

    private static State ToState(this char[][] map) =>
        new(map.FindRobot(),
            map.FindBoxes().ToBoxes(),
            map.FindWalls().ToHashSet());
    private static Point FindRobot(this char[][] map) =>
        map.PositionOf(_robot).Single();
    private static IEnumerable<Box> FindBoxes(this char[][] map) =>
        map.PositionOf(_box).Select(pos => new Box(pos, 1));
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

    private static IEnumerable<Box> GetBoxes(this Boxes boxes, IEnumerable<Point> positions) =>
        positions.Where(boxes.Index.ContainsKey)
            .Select(pos => boxes.Index[pos])
            .Distinct();
    private static Boxes ToBoxes(this IEnumerable<Box> boxes) =>
        new (boxes.SelectMany(ToPosition)
            .ToDictionary(pair => pair.position, pair => pair.box)
        );
    private static IEnumerable<(Point position, Box box)> ToPosition(this Box box) =>
        Enumerable.Range(0, box.Size)
            .Select(i => (new Point(box.Position.Row, box.Position.Column + i), box));
    record State(Point Robot, Boxes Boxes, HashSet<Point> Walls);
    record Boxes(Dictionary<Point, Box> Index);
    record struct Box(Point Position, int Size);
    record struct Step(int RowDiff, int ColumnDiff);
    record struct Point(int Row, int Column);

    private readonly static char _box = 'O';
    private readonly static char _wall = '#';
    private readonly static char _robot = '@';
}