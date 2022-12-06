using System.Text;

SolvePuzzle1(CreateCrateStacks());
SolvePuzzle2(CreateCrateStacks());


static void SolvePuzzle1(List<Stack<char>> crateStacks)
{
    File.ReadLines(Path.Combine(Environment.CurrentDirectory, "Input\\Day5Moves.txt")).ToList().ForEach(x =>
    {
        string[] moves = x.Split(' ');
        int cratesToMove = int.Parse(moves[0]);
        int originStack = int.Parse(moves[1]) - 1;
        int destinationStack = int.Parse(moves[2]) - 1;
        for (int i = 0; i < cratesToMove; i++)
        {
            char crate = crateStacks[originStack].Pop();
            crateStacks[destinationStack].Push(crate);
        }
    });

    StringBuilder stringBuilder = new StringBuilder();
    foreach (var stack in crateStacks)
    {
        stringBuilder.Append(stack.First());
    };
    Console.WriteLine($"{stringBuilder.ToString()}");
}

static void SolvePuzzle2(List<Stack<char>> crateStacks)
{
    File.ReadLines(Path.Combine(Environment.CurrentDirectory, "Input\\Day5Moves.txt")).ToList().ForEach(x =>
    {
        string[] moves = x.Split(' ');
        int cratesToMove = int.Parse(moves[0]);
        int originStack = int.Parse(moves[1]) - 1;
        int destinationStack = int.Parse(moves[2]) - 1;

        Stack<char> crates = new Stack<char>();
        for (int i = 0; i < cratesToMove; i++)
        {
            crates.Push(crateStacks[originStack].Pop());
        }
        crates
        .ToList()
        .ForEach(crate => crateStacks[destinationStack].Push(crate));
    });

    StringBuilder stringBuilder = new StringBuilder();
    foreach (var stack in crateStacks)
    {
        stringBuilder.Append(stack.First());
    };
    Console.WriteLine($"{stringBuilder.ToString()}");
}

static List<Stack<char>> CreateCrateStacks()
{
    List<List<char>> cratesInput = new List<List<char>>()
    {
        new List<char>(),
        new List<char>(),
        new List<char>(),
        new List<char>(),
        new List<char>(),
        new List<char>(),
        new List<char>(),
        new List<char>(),
        new List<char>(),
    };
    File.ReadLines(Path.Combine(Environment.CurrentDirectory, "Input\\Day5Crates.txt")).ToList().ForEach(x =>
    {
        for (int i = 1; i < 36; i = i + 4)
        {
            cratesInput[(i - 1) / 4].Add(x[i]);
        }
    });

    List<Stack<char>> crateStacks = new List<Stack<char>>();

    foreach (var input in cratesInput)
    {
        Stack<char> stack = new Stack<char>();
        var cratesToStack = input.OrderByDescending(x => input.IndexOf(x))
            .Where(x => x != ' ')
            .ToList();
        cratesToStack.ForEach(x => stack.Push(x));
        crateStacks.Add(stack);
    }

    return crateStacks;
}