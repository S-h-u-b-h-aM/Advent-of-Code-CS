namespace Advent_of_Code;

public static class Day10
{
    public static void Run(StringReader In)
    {
        char[][] map = In.ReadLines()
            .Select(row => row.ToCharArray())
            .ToArray();
        int totalScore = map.GetTrailHeads().Sum(trailhead => trailhead.GetScore(map));
        int totalRatings = map.GetTrailHeads().Sum(trailhead => trailhead.GetRating(map));
        Console.WriteLine($"  Total score: {totalScore}");
        Console.WriteLine($"Total ratings: {totalRatings}");
    }
    private static int GetScore(this Point trailhead, char[][] map) =>
        trailhead.WalkFrom(map).Count(state => map.At(state.point) == '9');
    private static int GetRating(this Point trailhead, char[][] map) => 
        trailhead.WalkFrom(map).Where(state => map.At(state.point) == '9').Sum(state => state.count);
    private static IEnumerable<(Point point, int count)> WalkFrom(this Point trailhead, char[][] map)
    {
        // Breadth First Search
        Dictionary<Point, int> pathsCount = new() { [trailhead] = 1 };
        // HashSet<Point> visited = [];
        Queue<Point> queue = [];
        
        queue.Enqueue(trailhead);
        while (queue.Count > 0)
        {
            Point current = queue.Dequeue();
            // if (!visited.Add(current)) continue;
            yield return (current, pathsCount[current]);

            foreach (Point neighbour in map.GetUphillNeighbours(current))
            {
                if (pathsCount.ContainsKey(neighbour))
                {
                    pathsCount[neighbour] += pathsCount[current];
                }
                else
                {
                    pathsCount[neighbour] = pathsCount[current];
                    queue.Enqueue(neighbour);
                }
            }
        }
    }
    private static IEnumerable<Point> GetUphillNeighbours(this char[][] map, Point point) =>
        new[]
        {
            new Point(point.Row - 1, point.Col), new (point.Row + 1, point.Col),
            new(point.Row, point.Col - 1), new (point.Row, point.Col + 1),
        }.Where(neighbour => map.IsUphill(point, neighbour));
    private static bool IsUphill(this char[][] map, Point a, Point b) =>
        map.Contains(a) && map.Contains(b) && map.At(b) == map.At(a) + 1;
    private static bool Contains(this char[][] map, Point point) =>
        point.Row >= 0 && 
        point.Row < map.Length && 
        point.Col >= 0 &&
        point.Col < map[point.Row].Length;
    
    private static char At(this char[][] map, Point point) => map[point.Row][point.Col];
    
    private static IEnumerable<Point> GetTrailHeads(this char[][] map) =>
        from row in Enumerable.Range(0, map.Length)
        from col in Enumerable.Range(0, map[row].Length)
        where map[row][col] == '0'
        select new Point(row, col);
    
    record Point(int Row, int Col);
}