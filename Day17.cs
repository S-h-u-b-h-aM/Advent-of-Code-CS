namespace Advent_of_Code;

public static class Day17
{
    public static void Run(StringReader In)
    {
        Machine machine = In.ReadMachine();
        string output = string.Join(",", machine.Run());
        Console.WriteLine(output);
    }

    private static IEnumerable<byte> Run(this Machine machine)
    {
        while (machine.IP < machine.Memory.Length)
        {
            (byte? output, machine) = machine.Step();
            if (output.HasValue) yield return output.Value;
        }
    }

    private static (byte? output, Machine machine) Step(this Machine m) =>
        m.FetchInstruction() switch
        {
            (0, int op) => (null, m with {A = m.A >> op, IP = m.IP + 2 }),
            (1, int op) => (null, m with {B = m.B ^ op, IP = m.IP + 2 }),
            (2, int op) => (null, m with {B = op & 0x7, IP = m.IP + 2 }),
            (3, int op) => (null, m with {IP = m.A == 0 ? m.IP + 2 : op }),
            (4, _) => (null, m with {B = m.B ^ m.C, IP = m.IP + 2 }),
            (5, int op) => ((byte)(op & 0x7), m with {IP = m.IP + 2 }),
            (6, int op) => (null, m with {B = m.A >> op, IP = m.IP + 2 }),
            (7, int op) => (null, m with {C = m.A >> op, IP = m.IP + 2 }),
            _ => throw new InvalidOperationException("Invalid opcode"),
        };

    private static (int opcode, int operand) FetchInstruction(this Machine machine) =>
        (machine.GetOpCode(), machine.GetOperand());
    private static int GetOpCode(this Machine machine) => machine.Memory[machine.IP];
    private static int GetOperand(this Machine machine) =>
        (machine.GetOpCode(), machine.Memory[machine.IP + 1]) switch
        {
            (1, byte operand) => operand,
            (3, byte operand) => operand,
            (4, _) => 0,
            (_, byte operand) when operand <= 3 => operand,
            (_, 4) => machine.A,
            (_, 5) => machine.B,
            (_, 6) => machine.C,
            _ => throw new InvalidOperationException("Invalid operand")
        };
    private static IEnumerable<(string field, int[] values)> ParseInput(this TextReader text) =>
        text.ReadLines()
            .Select(line => line.Split(": "))
            .Where(parts => parts.Length == 2)
            .Select(parts => (
                field: parts[0],
                values: parts[1].Split(',').Select(int.Parse).ToArray()));

    private static Machine ReadMachine(this TextReader text) =>
        text.ParseInput().Aggregate
            (
                new Machine(0, 0, 0, 0, Array.Empty<byte>()),
                (machine, fields) => fields.field switch
                {
                    "Register A" => machine with { A = fields.values[0] },
                    "Register B" => machine with { B = fields.values[0] },
                    "Register C" => machine with { C = fields.values[0] },
                    _ => machine with { Memory = fields.values.Select(v => (byte)v).ToArray() },
                }
            );
    record Machine(int A, int B, int C, int IP, byte[] Memory);
}