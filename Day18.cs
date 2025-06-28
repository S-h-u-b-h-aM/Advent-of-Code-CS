namespace Advent_of_Code;

public static class Day18
{
    public static void Run(StringReader In)
    {
        var fallingBytes = In.ReadPoints().ToList();
        var (width, height, startTime) = (71, 71, 1024);
        var maze = fallingBytes.Take(startTime).ToMaze(width, height);
        var shortestPath = maze.GetShortestPath();
        Console.WriteLine(shortestPath);
    }

    private static int GetShortestPath(this Maze maze)
    {
        (Point start, Point end) = (maze.GetStart(), maze.GetEnd());
        PriorityQueue<Point, int> queue = new([(start, 0)]);
        HashSet<Point> visited = [];
        while (queue.TryDequeue(out Point current, out int steps))
        {
            if (!visited.Add(current)) continue;
            if (current == end) return steps;
            foreach (Point neighbour in maze.GetNeighbours(current))
            {
                queue.Enqueue(neighbour, steps + 1);
            }
        }
        throw new InvalidOperationException("No path found");
    }
    private static IEnumerable<Point> GetNeighbours(this Maze maze, Point p) =>
        new[]
        {
            new Point(p.X - 1, p.Y), new Point(p.X + 1, p.Y),
            new Point(p.X, p.Y - 1), new Point(p.X, p.Y + 1),
        }
        .Where(maze.IsAvailable);
    private static bool IsAvailable(this Maze maze, Point p) => 
        maze.Contains(p) && !maze.Obstacles.Contains(p);
    private static bool Contains(this Maze maze, Point p) =>
        p.X >= 0 && p.X < maze.Width && p.Y >= 0 && p.Y < maze.Height;
    
    private static Point GetStart(this Maze maze) => new(0, 0);
    private static Point GetEnd(this Maze maze) => new(maze.Width - 1, maze.Height - 1);
    
    private static Maze ToMaze(this IEnumerable<Point> obstacles, int width, int height) =>
        new (width, height, obstacles.ToHashSet());
    private static IEnumerable<Point> ReadPoints(this TextReader text) =>
        text.ReadLines()
            .Select(Common.ParseIntsNoSign)
            .Select(line => new Point(line[0], line[1]));
    
    record Maze(int Width, int Height, HashSet<Point> Obstacles);
    record struct Point(int X, int Y);
}