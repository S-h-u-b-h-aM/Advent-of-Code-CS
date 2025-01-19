using Advent_of_Code;

public static class Program
{
    public static void Main(string[] args)
    {
        // Console.WriteLine("3  4\n4  3\n2  5\n1  3\n3  9\n3  3");
        // Day01.Execute();
        string testData = "mul(2,4)%mul[3,7]!@^do_not_mul(5,5)+mul(32,64]then(mul(11,8)mul(8,5))";
        Day03.Run(testData);
    }
}