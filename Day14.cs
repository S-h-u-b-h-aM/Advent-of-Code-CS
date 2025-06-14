namespace Advent_of_Code;

public static class Day14
{
    public static void Run(StringReader In)
    {
        var robots = In.ReadRobots().ToList();
        Coordinates roomSize = new(101, 103);
        int time = 100;
        
        int safetyFactor = robots.Move(time, roomSize).GetSafetyFactor(roomSize);
        Console.WriteLine($"Safety factor: {safetyFactor}");

        foreach ((List<Robot> state, int time) candidate in robots.GetXMasTreeCoordinates(roomSize))
        {
            candidate.state.Print(candidate.time, roomSize);
            Console.Write($"Keep searching? (y/n) ");
            if (Console.ReadLine()?.ToLower() == "n") break;
        }
    }

    private static IEnumerable<(List<Robot> state, int time)> GetXMasTreeCoordinates(
        this IEnumerable<Robot> robots, Coordinates roomSize) =>
        Enumerable.Range(0, int.MaxValue)
            .Where(time => robots.Move(time, roomSize).MaybeXMaxTree())
            .Select(time => (robots.Move(time, roomSize).ToList(), time));
    private static bool MaybeXMaxTree(this IEnumerable<Robot> robots) =>
        robots.GetGroupSizes().Max() >= robots.Count() / 3;
    private static IEnumerable<int> GetGroupSizes(this IEnumerable<Robot> robots)
    {
         HashSet<Coordinates> pending = robots.Select(robot => robot.Position).ToHashSet();
         while (pending.Count > 0)
         {
             Coordinates pivot = pending.First();
             pending.Remove(pivot);

             var add = new Queue<Coordinates>();
             add.Enqueue(pivot);
             
             int groupSize = 0;
             while (add.TryDequeue(out var current))
             {
                 groupSize++;

                 Coordinates[] neighbours = new[]
                 {
                     current with { X = current.X - 1 },
                     current with { X = current.X + 1 },
                     current with { Y = current.Y - 1 },
                     current with { Y = current.Y + 1 },
                 };
                 foreach (Coordinates next in neighbours.Where(pending.Remove)) add.Enqueue(next);
             }
             yield return groupSize;
         }
    }
    private static void Print(this IEnumerable<Robot> robots, int time, Coordinates roomSize)
    {
        char[][] map = Enumerable.Range(0, roomSize.Y)
            .Select(_ => Enumerable.Repeat('.', roomSize.X).ToArray()).ToArray();
        robots.ToList().ForEach(robot => map[robot.Position.Y][robot.Position.X] = '#');
        
        Console.WriteLine();
        Console.WriteLine($"Time: {time}");
        map.Select(row => new string(row)).ToList().ForEach(Console.WriteLine);
    }
    private static int GetSafetyFactor(this IEnumerable<Robot> robots, Coordinates roomSize) =>
        robots.ToQuadrantCounts(roomSize).Aggregate(1, (safety, count) => safety * count);
    private static IEnumerable<int> ToQuadrantCounts(this IEnumerable<Robot> robots, Coordinates roomSize) => new[]
    {
        robots.Count(robot => robot.ToQuadrant(roomSize) == (1, 1)),
        robots.Count(robot => robot.ToQuadrant(roomSize) == (1, -1)),
        robots.Count(robot => robot.ToQuadrant(roomSize) == (-1, 1)),
        robots.Count(robot => robot.ToQuadrant(roomSize) == (-1, -1)),
    };
    private static (int horizontal, int vertical) ToQuadrant(this Robot robot, Coordinates roomSize) =>
        ( Math.Sign(robot.Position.X - roomSize.X/2), 
            Math.Sign(robot.Position.Y - roomSize.Y/2) );
    private static IEnumerable<Robot> Move(this IEnumerable<Robot> robots, int time, Coordinates roomSize) =>
        robots.Select(robot => robot.Move(time, roomSize));
    private static Robot Move(this Robot robot, int time, Coordinates roomSize) =>
        robot with { Position = robot.Position.Move(robot.Velocity, time, roomSize) };
    private static Coordinates Move(this Coordinates position, Coordinates velocity, int time, Coordinates roomSize) =>
        new( position.X.Move(velocity.X, time, roomSize.X ), position.Y.Move(velocity.Y, time, roomSize.Y) );
    private static int Move(this int position, int velocity, int time, int roomSize) =>
        ((position + velocity * time) % roomSize + roomSize) % roomSize;
    private static IEnumerable<Robot> ReadRobots(this TextReader text) =>
        text.ReadLines()
            .Select(Common.ParseInts)
            .Select(list => new Robot(new(list[0], list[1]), new(list[2], list[3])));
    record struct Coordinates(int X, int Y);
    record Robot(Coordinates Position, Coordinates Velocity); 
}