namespace Advent_of_Code;

public static class Day08
{
    public static void Run(StringReader In)
    {
        char[][] map = In.ReadLines()
            .Select(row => row.ToCharArray())
            .ToArray();
        int numAntinodes = map.GetAntennaSets().GetAntinodes(map, GetNonResonatingAntinodes).Count();
        int numResonationAntinodes = map.GetAntennaSets().GetAntinodes(map, GetResonatingAntinodes).Count();
        Console.WriteLine($"Non-resonating antenna antinodes: {numAntinodes}");
        Console.WriteLine($"    Resonating an antinodes: {numResonationAntinodes}");
    }
    
    private static IEnumerable<Position> GetAntinodes(this IEnumerable<AntennaSet> antennaSets, char[][] map, AntinodeGenerator antinodeGenerator) =>
        antennaSets.SelectMany(set => set.GetAntinodes(map, antinodeGenerator)).Distinct();
    private static IEnumerable<Position> GetAntinodes(this AntennaSet antennaSet, char[][] map, AntinodeGenerator antinodeGenerator) =>
        antennaSet.Antennas.GetPairs().SelectMany(pair => pair.GetAntinodes(map, antinodeGenerator)).Distinct();
    private static IEnumerable<Position> GetAntinodes(this (Position p1, Position p2) positionPair, char[][] map, 
        AntinodeGenerator antinodeGenerator)
    {
        int rowDiff = positionPair.p1.Row - positionPair.p2.Row;
        int colDiff = positionPair.p1.Col - positionPair.p2.Col;
        
        // For Part 1
        // Position a = new( positionPair.p1.Row + rowDiff, positionPair.p1.Col + colDiff);
        // Position b = new ( positionPair.p2.Row - rowDiff, positionPair.p2.Col - colDiff);
        //
        // if (map.Contains(a)) yield return a;
        // if (map.Contains(b)) yield return b;
        
        return antinodeGenerator(map, positionPair.p1, rowDiff, colDiff)
            .Concat(antinodeGenerator(map, positionPair.p2, -rowDiff, -colDiff));
    }

    private static IEnumerable<Position> GetResonatingAntinodes(char[][] map, Position antennaPos, int rowDiff, int colDiff)
    {
        Position position = antennaPos;
        while (map.Contains(position))
        {
            yield return position;
            position = new(position.Row + rowDiff, position.Col + colDiff);
        }
    }
    private static IEnumerable<Position> GetNonResonatingAntinodes(char[][] map, Position antenna, int rowDiff, int colDiff)
    {
        Position position = new(antenna.Row + rowDiff, antenna.Col + colDiff);
        if (map.Contains(position)) yield return position;
    }
    delegate IEnumerable<Position> AntinodeGenerator(char[][] map, Position antennaPos, int rowDiff, int colDiff);
    private static bool Contains(this char[][] map, Position position) =>
        position.Row >= 0 && position.Row < map.Length &&
        position.Col >= 0 && position.Col < map[0].Length;
    
    private static IEnumerable<(Position p1, Position p2)> GetPairs(this List<Antenna> antennas) =>
        antennas.SelectMany( (p1, index) => antennas[ (index+1).. ].Select(p2 => (p1.Position, p2.Position)));
    
    private static IEnumerable<AntennaSet> GetAntennaSets(this char[][] map) =>
        map.GetAntennas().GroupBy(antenna => antenna.Frequency,
            (frequency, antennas) => new AntennaSet(frequency, antennas.ToList()));
    private static IEnumerable<Antenna> GetAntennas(this char[][] map) =>
        map.GetContent()
            .Where(cell => cell.content != '.')
            .Select(cell => new Antenna(cell.content, cell.position));
    private static IEnumerable<(char content, Position position)> GetContent(this char[][] map) =>
        from row in Enumerable.Range(0, map.Length)
        from column in Enumerable.Range(0, map[row].Length)
        select (content: map[row][column], position: new Position(row, column));
    
    record AntennaSet(char Frequency, List<Antenna> Antennas);
    record struct Position(int Row, int Col);
    record struct Antenna(char Frequency, Position Position);
}