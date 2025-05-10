namespace Advent_of_Code;

public static class Day09
{
    public static void Run(string inputText)
    {
        List<Fragment> disc = new StringReader(inputText).ReadDisc().ToList();
        long checksum = disc.Compact(MoveBlocks).Sum(GetChecksum);
        long fileMovingChecksum = disc.Compact(MoveFiles).Sum(GetChecksum);
        Console.WriteLine($"Block moving checksum: {checksum}");
        Console.WriteLine($" File moving checksum: {fileMovingChecksum}");
    }
    
    // pos               + (pos + 1)         + .... +  (pos + len - 1)
    // (pos + len - 1)   + (pos + len - 2)   + .... +  pos
    // ------------------------------------------------------------------
    // (2*pos + len - 1) + (2*pos + len - 1) + .... +  (2*pos + len - 1)
    // len * (2*pos + len - 1) / 2
    private static long GetChecksum(this FileFragment file) => 
        (long)file.FileID * file.Length * (2 * file.Position + file.Length - 1) / 2;

    delegate int MoveConstraint(FileFragment file);
    private static int MoveBlocks(FileFragment file) => 0;
    private static int MoveFiles(FileFragment file) => file.Length;

    private static IEnumerable<FileFragment> Compact(this IEnumerable<Fragment> disc) => disc.Compact(MoveBlocks);
    private static IEnumerable<FileFragment> Compact(this IEnumerable<Fragment> disc, MoveConstraint constraint)
    {
        var files = disc.OfType<FileFragment>().OrderByDescending(file => file.Position);
        IEnumerable<Gap> gaps = disc.OfType<Gap>().OrderBy(gap => gap.Position);

        foreach (FileFragment file in files)
        {
            int pendingBlocks = file.Length;
            List<Gap> remainingGaps = [];  
            foreach (Gap gap in gaps.Where(gap => gap.Position < file.Position))
            {
                int move = Math.Min(pendingBlocks, gap.Length);

                if (move < constraint.Invoke(file))
                {
                    remainingGaps.Add(gap);
                    continue;
                }
                
                if (move > 0) yield return file with { Position = gap.Position, Length = move };
                if (gap.Length > move) remainingGaps.Add(new(Position: gap.Position + move, Length: gap.Length -move));
                pendingBlocks -= move;
            }
            gaps = remainingGaps;
            if (pendingBlocks > 0) yield return file with { Length = pendingBlocks };
        }
    }
    private static IEnumerable<Fragment> ReadDisc(this TextReader textReader)
    {
        int position = 0;
        foreach (Fragment fragment in textReader.ReadSpec().Select(tuple => tuple.ToFragment(position)))
        {
            yield return fragment;
            position += fragment.Length;
        }
    }
    private static Fragment ToFragment(this (int? fileID, int blocks) tuple, int position) =>
        tuple.fileID is int fileID ? new FileFragment(fileID, position, tuple.blocks)
        : new Gap(position, tuple.blocks); 
    
    private static IEnumerable<(int? fileID, int blocks)> ReadSpec(this TextReader textReader) =>
        (textReader.ReadLine() ?? string.Empty)
        // .Select(c => (int.Parse(c.ToString()), c - '0')); // copilot genreated 
        .Select(c => (int)(c - '0'))
        .Select((int value, int i) => (i % 2 == 0 ? (int?)(i / 2) : null, value));

    abstract record Fragment(int Position, int Length);
    record FileFragment(int FileID, int Position, int Length) : Fragment(Position, Length);
    record Gap(int Position, int Length) : Fragment(Position, Length);
}