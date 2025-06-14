using System.Collections.Immutable;

namespace Advent_of_Code;
public static class Day16
{
    public static void Run(string inputText)
    {
        char[][] maze = new StringReader(inputText).ReadMaze();
        int stepCost = 1, turnCost = 1000;
        
        var (cheapestPath, paths) = maze.FindCheapestPath(stepCost, turnCost);
        int pathLengths = paths.Distinct().Count();
        
        Console.WriteLine($"     Cheapest path: {cheapestPath}");
        Console.WriteLine($"Total path lengths: {pathLengths}");
    }
    private static ReachedState ToReachedState(this Point point) => new(0, new[] { point }.ToImmutableList());

    private static ReachedState Add(this ReachedState reached, Point point, int cost) =>
        new(cost, reached.FootSteps.Add(point));
    private static ReachedState MergeWith(this ReachedState reached, ReachedState? other) =>
        other is null ? reached
        : reached.Cost < other.Cost ? reached
        : reached with { FootSteps = reached.FootSteps.AddRange(other.FootSteps) };
    private static ReachedState FindCheapestPath(this char[][] maze, int stepCost, int turnCost)
    {
        State startNode = new(maze.GetStartPosition(), new Direction(0, 1));
        
        Dictionary<State, ReachedState> reach = new() { [startNode] = startNode.Position.ToReachedState() };
        HashSet<State> visited = [];
        PriorityQueue<State, int> queue = new([(startNode, 0)]);

        while (queue.Count > 0)
        {
            State current = queue.Dequeue();
            if (maze.IsEnd(current.Position)) return reach[current];
            if (!visited.Add(current)) continue;
            (int cost, _) = reach[current];
            (State State, int cost)[] neighbours = 
            [
                (current.StepForward(), cost + stepCost),
                (current.TurnLeft(), cost + turnCost),
                (current.TurnRight(), cost + turnCost),
            ];
            foreach ((State state, int cost) neighbour in neighbours)
            {
                if (!maze.IsEmpty(neighbour.state.Position)) continue;
                if (reach.TryGetValue(neighbour.state, out ReachedState? reachedNeighbour) && neighbour.cost > reachedNeighbour.Cost) continue;
                
                reach[neighbour.state] = reach[current]
                    .Add(neighbour.state.Position, neighbour.cost)
                    .MergeWith(reachedNeighbour);
                
                queue.Enqueue(neighbour.state, neighbour.cost);
            }
        }
        throw new InvalidOperationException("No path found");
    }
    private static Point Move(this Point point, Direction direction) =>
        new(point.Row + direction.RowStep, point.Col + direction.ColStep);

    private static State StepForward(this State node) => 
        node with { Position = node.Position.Move(node.Orientation) };

    private static State TurnLeft(this State state) =>
        state with { Orientation = new(-state.Orientation.ColStep, state.Orientation.RowStep)};
    private static State TurnRight(this State state) =>
        state with { Orientation = new(state.Orientation.ColStep, -state.Orientation.RowStep)};
    private static IEnumerable<Point> FindAll(this char[][] maze, char content) =>
        from row in Enumerable.Range(0, maze.Length)
        from col in Enumerable.Range(0, maze[row].Length)
        where maze[row][col] == content
        select new Point(row, col);
    private static Point GetStartPosition(this char[][] maze) => maze.FindAll('S').First();
    private static char At(this char[][] maze, Point point) => maze[point.Row][point.Col];
    private static bool IsEmpty(this char[][] maze, Point point) => maze[point.Row][point.Col] != '#';
    private static bool IsEnd(this char[][] maze, Point point) => maze[point.Row][point.Col] == 'E';
    private static char[][] ReadMaze(this TextReader textReader) =>
        textReader.ReadLines().Select(line => line.ToCharArray()).ToArray();
    record ReachedState(int Cost, ImmutableList<Point> FootSteps);  // Part 2
    record State(Point Position, Direction Orientation);
    record struct Direction(int RowStep, int ColStep);
    record struct Point(int Row, int Col);
}