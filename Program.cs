using System.Reflection;
using Advent_of_Code;

public static class Program
{
    public static void Main(string[] args)
    {
        // string filename = "Day01.txt";
        // string filepath = Path.Join(GetResourceDirectory(), filename);
        // var testData = File.ReadAllText(filepath);
        // Day01.Run(testData);

        var text2ClassMap = Text2ClassMap();
        
        
        string filename = "Day02.txt";
        string filepath = Path.Join(GetResourceDirectory(), filename);
        var testData = File.ReadAllText(filepath);
        
        text2ClassMap[filename].Invoke(testData);
    }

    private static string GetDatafromTextFile(string filepath)
    {
        string text = File.ReadAllText(filepath);
        return text;
    }

    private static Dictionary<string, Action<string>> Text2ClassMap()
    {
        Dictionary<string, Action<string>> text2class_map = new();
        text2class_map["Day01.txt"] = Day01.Run;
        text2class_map["Day02.txt"] = Day02.Run;
        text2class_map["Day03.txt"] = Day03.Run;
        
        return text2class_map;
    }

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