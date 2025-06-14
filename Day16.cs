namespace Advent_of_Code;
public static class Day16
{
    public static void Run(string inputText)
    {
        char[][] maze = new StringReader(inputText).ReadMaze();
        int stepCost = 1, turnCost = 1000;
        
        int cheapestPath = maze.FindCheapestPath(stepCost, turnCost);
        Console.WriteLine(cheapestPath);
    }

    private static int FindCheapestPath(this char[][] maze, int stepCost, int turnCost)
    {
        State startNode = new(maze.GetStartPosition(), new Direction(0, 1));
        
        Dictionary<State, int> costs = new(){ [startNode] = 0 };
        HashSet<State> visited = [];
        PriorityQueue<State, int> queue = new([(startNode, 0)]);

        while (queue.Count > 0)
        {
            State current = queue.Dequeue();
            if (maze.IsEnd(current.Position)) return costs[current];
            if (!visited.Add(current)) continue;
            int cost = costs[current];
            (State State, int cost)[] neighbours = 
            [
                (current.StepForward(), cost + stepCost),
                (current.TurnLeft(), cost + turnCost),
                (current.TurnRight(), cost + turnCost),
            ];
            foreach ((State state, int cost) neighbour in neighbours)
            {
                if (!maze.IsEmpty(neighbour.state.Position)) continue;
                if (costs.TryGetValue(neighbour.state, out int currentCost) && neighbour.cost >= currentCost) continue;
                
                costs[neighbour.state] = neighbour.cost;
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
    
    record State(Point Position, Direction Orientation);
    record struct Direction(int RowStep, int ColStep);
    record struct Point(int Row, int Col);
}