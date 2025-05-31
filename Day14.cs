namespace Advent_of_Code;

public static class Day14
{
    public static void Run(string inputText)
    {
        var robots = new StringReader(inputText).ReadRobots().ToList();
        Coordinates roomSize = new(101, 103);
        int time = 100;
        
        int safetyFactor = robots.Move(time, roomSize).GetSafetyFactor(roomSize);
        Console.WriteLine($"Safety factor: {safetyFactor}");
        
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