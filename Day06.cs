using System.Diagnostics;

namespace Advent_of_Code;

public static class Day06
{
    public static void Run(StringReader In)
    {
        char[][] map = In.ReadLines()
            .Select(row => row.ToCharArray())
            .ToArray();
        
        Stopwatch pathTime = Stopwatch.StartNew();
        int pathLength = map.FindPath().Count();
        pathTime.Stop();
        
        Stopwatch obstructionTime = Stopwatch.StartNew();
        int obstructionPoints = map.GetObstructionPoints().Count();
        obstructionTime.Stop();
        
        Console.WriteLine($"        Path length: {pathLength} in ({pathTime.ElapsedMilliseconds} ms)");
        Console.WriteLine($" Obstruction points: {obstructionPoints} in ({obstructionTime.ElapsedMilliseconds} ms)");
    }
    
    private static IEnumerable<Point> GetObstructionPoints(this char[][] map) =>
        map.GetObstructionPoints( map.FindStartingPosition() );
    // private static IEnumerable<Point> GetObstructionPoints(this char[][] map, Position start) =>
    //     map.FindPath().Except( [map.FindStartingPosition().Point] )
    //         .Where(point => point.CausesLoop(map, start));

    private static IEnumerable<Point> GetObstructionPoints(this char[][] map, Position start)
    {
        HashSet<Position> seen = [];
        HashSet<Point> checkedObstructions = [start.Point];
        foreach (Position current in map.Walk(start))
        {
            seen.Add(current);
            Point ahead = current.MoveForward().Point;
            
            if (!map.Contains(ahead) || map.IsObstruction(ahead)) continue;
            if (!checkedObstructions.Add(ahead)) continue;
            if (!ahead.CausesLoop(map, current.TurnRight(), seen)) continue;
            yield return ahead;
        }
    }
    private static bool CausesLoop(this Point obstruction, char[][] map, Position start, HashSet<Position> seenBefore)
    {
        HashSet<Position> seen = [];
        Position current = start;
        while (map.Contains(current.Point) && !seenBefore.Contains(current) && seen.Add(current))
        {
            current = current.Step(map, obstruction);
        }
        return map.Contains(current.Point);
    }
    private static IEnumerable<Point> FindPath(this char[][] map) =>
        map.Walk(map.FindStartingPosition())
            .Select(position => position.Point)
            .Distinct();
    
    private static IEnumerable<Position> Walk(this char[][] map, Position startPosition)
    {
        for (Position position = startPosition; map.Contains(position.Point); position = position.Step(map))
        {
            yield return position;
        }
    }
    
    private static Position Step(this Position position, char[][] map, Point obstruction) =>
        position.MoveForward() is Position forward && !map.IsObstruction(forward.Point, obstruction) ? forward 
            : position.TurnRight();
    private static Position Step(this Position position, char[][] map) => 
        position.MoveForward() is Position forward && !map.IsObstruction(forward.Point) ? forward 
            : position.TurnRight();

    private static Point Step(this Point point, char direction) =>
        direction switch
        {
            '^' => new Point(point.Row - 1, point.Column),
            '>' => new Point(point.Row, point.Column + 1),
            'v' => new Point(point.Row + 1, point.Column),
            _ => new Point(point.Row, point.Column - 1),
        };
    
    private static bool IsObstruction(this char[][] map, Point point, Point obstruction) =>
        map.Contains(point) && (map.At(point) == '#' || point == obstruction);
    private static bool IsObstruction(this char[][] map, Point point) =>
        map.Contains(point) && map.At(point) == '#';
    
    private static bool Contains(this char[][] map, Point point) =>
        point.Row >= 0 && point.Row < map.Length && 
        point.Column >= 0 && point.Column < map[point.Row].Length;
    
    private static Position MoveForward(this Position position) =>
        position with { Point = position.Point.Step(position.Orientation) };

    private static Position TurnRight(this Position position) =>
        position with { Orientation = position.Orientation.TurnRight() };
    private static char TurnRight(this char orientation) =>
        Orientations[(Orientations.IndexOf(orientation) + 1) % Orientations.Length];

    private static Position FindStartingPosition(this char[][] map) =>
        map.AllPoints()
            .Where(point => Orientations.Contains(map.At(point)))
            .Select(point => new Position(point, map.At(point)))
            .FirstOrDefault() is { } position ? position : throw new InvalidOperationException("Map must contain a starting orientation (^ > v <)");
            // .First();
    
    private static IEnumerable<Point> AllPoints(this char[][] map) =>
        from row in Enumerable.Range(0, map.Length)
        from column in Enumerable.Range(0, map[row].Length)
        select new Point(row, column);
    private static char At(this char[][] map, Point point) => map[point.Row][point.Column];
    private static readonly string Orientations = "^>v<"; 
    private record struct Point(int Row, int Column);
    private record struct Position(Point Point, char Orientation);
}