using System.Reflection;
using Advent_of_Code;

public static class Program
{
    public static void Main(string[] args)
    {
        string filename = Text2ClassMap.Last().Key; // The last filename in Test Data. 
        string filepath = Path.Join(GetResourceDirectory(), filename);
        string testData = File.ReadAllText(filepath);
        StringReader inputText = new StringReader(testData);
        Text2ClassMap[filename].Invoke(inputText);
    }

    private static string GetDatafromTextFile(string filepath)
    {
        string text = File.ReadAllText(filepath);
        return text;
    }

    private static Dictionary<string, Action<StringReader>> Text2ClassMap => new()
    {
        ["Day01.txt"] = Day01.Run,
        ["Day02.txt"] = Day02.Run,
        ["Day03.txt"] = Day03.Run,
        ["Day04.txt"] = Day04.Run,
        ["Day05.txt"] = Day05.Run,
        ["Day06.txt"] = Day06.Run,
        ["Day07.txt"] = Day07.Run,
        ["Day08.txt"] = Day08.Run,
        ["Day09.txt"] = Day09.Run,
        ["Day10.txt"] = Day10.Run,
        ["Day11.txt"] = Day11.Run,
        ["Day12.txt"] = Day12.Run,
        ["Day13.txt"] = Day13.Run,
        ["Day14.txt"] = Day14.Run,
        ["Day15.txt"] = Day15.Run,
        ["Day16.txt"] = Day16.Run,
        ["Day17.txt"] = Day17.Run,
        ["Day18.txt"] = Day18.Run,
        ["Day19.txt"] = Day19.Run
    };

    private static string GetResourceDirectory()
    {
        // return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        string assemblyPath = Assembly.GetExecutingAssembly().Location;

        // Get the directory of the assembly
        string assemblyDir = Path.GetDirectoryName(assemblyPath)!;

        // Navigate back to the project directory
        string projectDir = Path.GetFullPath(Path.Combine(assemblyDir, @"..\..\.."));
        
        string testDataDir = Path.GetFullPath(Path.Combine(projectDir, "TestData"));
        return testDataDir;
    }
    private static void ReadCmdArgs(string[] args)
    {
        if (args.Length < 1)
            return;
    }
}