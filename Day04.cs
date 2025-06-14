using System.Numerics;
using System.Security.AccessControl;
using System.Text.Json.Serialization.Metadata;
using System.Text.RegularExpressions;

namespace Advent_of_Code;

public static class Day04
{
    public static void Run(StringReader In)
    {
        var matrix = In.ReadLines().ToList();
        int rows = matrix.Count;
        int cols = matrix[0].Length;

        int count = matrix.GetAllStrings(rows, cols).Sum(s => s.CountWord("XMAS"));
        int xcount = matrix.GetAllXes(rows, cols).Count(x => ValidXs.Contains(x));
        
        Console.WriteLine($"XMAS count: {count}");
        Console.WriteLine($"X-MAS count: {xcount}");
    }

    private static string[] ValidXs = ["MASMAS", "MASSAM", "SAMMAS", "SAMSAM"];

    private static IEnumerable<string> GetAllXes(this List<string> matrix, int rows, int cols) =>
        GetXCenters(rows, cols).Select(center => matrix.GetX(center.row, center.col));

    private static string GetX(this List<string> matrix, int row, int col) => new string([
        matrix[row - 1][col - 1], matrix[row][col], matrix[row + 1][col + 1],
        matrix[row - 1][col + 1], matrix[row][col], matrix[row + 1][col - 1]]);
    private static IEnumerable<(int row, int col)> GetXCenters(int rows, int cols) =>
        Enumerable.Range(1, rows - 2).SelectMany(row => Enumerable.Range(1, cols - 2).Select(col => (row, col)));
    
    private static int CountWord(this string s, string word) => Regex.Matches(s, Regex.Escape(word)).Count;
    private static IEnumerable<string> GetAllStrings(this IEnumerable<string> matrix, int rows, int cols) =>
    matrix.Rows().Concat(matrix.Columns(cols)).Concat(matrix.Diagonals(rows, cols)).Concat(matrix.AntiDiagonals(rows, cols)).TowWay();

    private static IEnumerable<string> Rows(this IEnumerable<string> matrix) => matrix;
    private static IEnumerable<string> Columns(this IEnumerable<string> matrix, int cols) => 
        Enumerable.Range(0, cols).Select(i => new string(matrix.Select(row => row[i]).ToArray()));

    private static IEnumerable<string> Diagonals(this IEnumerable<string> matrix, int rows, int cols) =>
        Enumerable.Range(0, cols).Select(col => matrix.Diagonal(0, col, cols))
            .Concat(Enumerable.Range(1, rows-1).Select(rpw => matrix.Diagonal(rows, 0, cols)));
    
    private static IEnumerable<string> TowWay(this IEnumerable<string> strings) =>
        strings.SelectMany(s => new[] { s, new string(s.Reverse().ToArray()) });
    private static IEnumerable<string> AntiDiagonals(this IEnumerable<string> matrix, int rows, int cols) =>
        matrix.Reverse().Diagonals(rows, cols);
    private static string Diagonal(this IEnumerable<string> matrix, int startRow, int startCol, int cols) =>
    new string(matrix.Skip(startRow).Take(cols - startCol).Select((row, i) => row[startCol + i]).ToArray());
    
    public static string Description => "--- Day 4: Ceres Search ---\n" +
                                        "\"Looks like the Chief's not here. Next!\" One of The Historians pulls out a device and pushes the only button on it. " +
                                        "After a brief flash, you recognize the interior of the Ceres monitoring station!\n" +
                                        "\nAs the search for the Chief continues, a small Elf who lives on the station tugs on your shirt; she'd like to know if you could help her with her word search (your puzzle input). " +
                                        "She only has to find one word: XMAS.\n\nThis word search allows words to be horizontal, vertical, diagonal, written backwards, or even overlapping other words. " +
                                        "It's a little unusual, though, as you don't merely need to find one instance of XMAS - you need to find all of them. Here are a few ways XMAS might appear, where irrelevant characters have been replaced with .:\n" +
                                        "\n\n..X...\n.SAMX.\n.A..A.\nXMAS.S\n.X....\nThe actual word search will be full of letters instead. For example:\n\nMMMSXXMASM\nMSAMXMSMSA\nAMXSXMAAMM\nMSAMASMSMX\nXMASAMXAMM\nXXAMMXXAMA\nSMSMSASXSS\nSAXAMASAAA\nMAMMMXMMMM\nMXMXAXMASX\n" +
                                        "In this word search, XMAS occurs a total of 18 times; here's the same word search again, but where letters not involved in any XMAS have been replaced with .:\n" +
                                        "\n....XXMAS." +
                                        "\n.SAMXMS..." +
                                        "\n...S..A..." +
                                        "\n..A.A.MS.X" +
                                        "\nXMASAMX.MM" +
                                        "\nX.....XA.A" +
                                        "\nS.S.S.S.SS" +
                                        "\n.A.A.A.A.A" +
                                        "\n..M.M.M.MM" +
                                        "\n.X.X.XMASX" +
                                        "\nTake a look at the little Elf's word search. How many times does XMAS appear?";
    
}