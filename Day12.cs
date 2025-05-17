namespace Advent_of_Code;

public static class Day12
{
    public static void Run(string inputText)
    {
        var map = new StringReader(inputText).ReadMap();
        var regions = map.GetRegions().ToList();
        
        int totalCost = regions.Sum(region => region.Cost(map));
        int discountedCost = regions.Sum(region => region.DiscountedCost(map)); 
        
        Console.WriteLine($"     Total cost: {totalCost}");
        Console.WriteLine($"Discounted cost: {discountedCost}");
    }

    private static int DiscountedCost(this List<(int row, int col)> region, char[][] map) =>
        region.DiscountedPerimeter(map) * region.Area();
    private static int DiscountedPerimeter(this List<(int row, int col)> region, char[][] map) =>
        region.Perimeter(map) - region.Sum(point => point.CountContinuingFences(map));
    private static bool HasContinuousFence(
        this (int row, int col) point,
        (int rowDiff, int colDiff) preceding,
        (int rowDiff, int collDiff) outside,
        char[][] map) =>
        !point.IsNeighbour(point.Move(outside), map) &&
        point.IsNeighbour(point.Move(preceding), map) &&
        !point.IsNeighbour(point.Move(preceding).Move(outside), map);
    
    private static (int row, int col) Move(this (int row, int col) point, (int rowDiff, int colDiff) direction) =>
        (point.row + direction.rowDiff, point.col + direction.colDiff);

    private static bool IsNeighbour(this (int row, int col) point, (int rowDiff, int colDiff) other, char[][] map) =>
        map.Contains(other) && map.At(other) == map.At(point);
    private static bool ContinuesFenceLeftward(this (int row, int col) point, char[][] map) =>
        point.HasContinuousFence(_preceding[(int)Dir.LEFT], _outside[(int)Dir.LEFT], map);
    
    private static bool ContinuesFenceUpward(this (int row, int col) point, char[][] map) =>
        point.HasContinuousFence(_preceding[(int)Dir.UP], _outside[(int)Dir.UP], map);
    
    private static bool ContinuesFenceRightward(this (int row, int col) point, char[][] map) =>
        point.HasContinuousFence(_preceding[(int)Dir.RIGHT], _outside[(int)Dir.RIGHT], map);

    private static bool ContinuesFenceDownward(this (int row, int col) point, char[][] map) =>
        point.HasContinuousFence(_preceding[(int)Dir.DOWN], _outside[(int)Dir.DOWN], map);

    private static int CountContinuingFences(this (int row, int col) point, char[][] map) =>
        new[]
        {
            point.ContinuesFenceLeftward(map), point.ContinuesFenceUpward(map),
            point.ContinuesFenceRightward(map), point.ContinuesFenceDownward(map)
        }.Count(x => x);
    private static int Cost(this List<(int row, int col)> region, char[][] map) =>
        region.Area() * region.Perimeter(map);

    private static int Area(this List<(int row, int col)> region) => region.Count;

    private static int Perimeter(this List<(int row, int col)> region, char[][] map) =>
        region.Sum(point => 4 - point.GetNeighbours(map).Count());
    
    private static IEnumerable<List<(int row, int col)>> GetRegions(this char[][] map)
    {
        var pending = map.GetAllCoordinates().ToHashSet();

        while (pending.Count > 0)
        {
            var pivot = pending.First();
            pending.Remove(pivot);
            
            var add = new Queue<(int row, int col)>();
            add.Enqueue(pivot);
            
            var region = new List<(int row, int col)>();

            while (add.TryDequeue(out (int row, int col) current))
            {
                region.Add(current);
                current.GetNeighbours(map).ForEach(next => { if (pending.Remove(next)) add.Enqueue(next); });
            }
            yield return region;
        }
    }
    private static IEnumerable<(int row, int col)> GetNeighbours(this (int row, int col) point, char[][] map) =>
        new[]
            {
                (point.row - 1, point.col), (point.row + 1, point.col),
                (point.row, point.col - 1), (point.row, point.col + 1),
            }
            .Where(map.Contains)
            .Where(neighbour => map.At(neighbour) == map.At(point));

    private static IEnumerable<(int row, int col)> GetAllCoordinates(this char[][] map) =>
        from row in Enumerable.Range(0, map.Length)
        from col in Enumerable.Range(0, map[row].Length)
        select (row, col);
    
    private static char At(this char[][] map, (int row, int col) point) => map[point.row][point.col];
    
    private static bool Contains(this char[][] map, (int row, int col) point) =>
        point.row >= 0 && point.row < map.Length &&
        point.col >= 0 && point.col < map[point.row].Length;
    
    private static char[][] ReadMap(this TextReader text) =>
        text.ReadLines().Select(line => line.ToCharArray()).ToArray();

    private static List<(int rowDiff, int colDiff)> _preceding = 
    [ 
        (0, 1),     // Leftward 
        (1, 0),     // Upward
        (0, -1),    // Rightward
        (-1, 0)     // Downward
    ];

    private static List<(int rowDiff, int colDiff)> _outside =
    [
        (1, 0),     // Leftward
        (0, -1),    // Upward,
        (-1, 0),    // Rightward
        (0, 1),     // Downward
    ];
    private enum Dir
    {
        LEFT,
        UP,
        RIGHT,
        DOWN
    }
}