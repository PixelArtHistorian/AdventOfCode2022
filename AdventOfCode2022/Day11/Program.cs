var monkeys = CreateMonkeyList("Input\\Example.txt");
PlayRound(monkeys, 20);
Console.WriteLine($"{CalculateMonkeyBusiness(monkeys.Values)}");


IDictionary<string, Monkey> CreateMonkeyList(string inputFile)
{
    Dictionary<string, Monkey> monkeys = new Dictionary<string, Monkey>();
    string monkeyName = "";
    Queue<int> startingItems = new Queue<int>();
    Func<int, int>? operation = null;
    Func<int, bool>? test = null;
    List<string> targetMonkeys = new List<string>();
    int simianCounter = 0;

    File.ReadLines(inputFile).ToList().ForEach(line =>
    {
        if (!string.IsNullOrWhiteSpace(line))
        {
            simianCounter++;
        }
        var elements = line.Split(':');
        if (elements[0].Trim().StartsWith("Monkey"))
        {
            monkeyName = elements[0].Replace(' ', '-');
        }
        if (elements[0].Trim().StartsWith("Starting items"))
        {
            var items = elements[1].Trim().Split(", ");
            foreach (var item in items)
            {
                startingItems.Enqueue(int.Parse(item));
            }
        }
        if (elements[0].Trim().StartsWith("Operation"))
        {
            var operationElements = elements[1].Split(" ");
            int increase = 0;
            if (operationElements[4].StartsWith("*"))
            {
                if (operationElements[5].StartsWith("old"))
                {
                    operation = x =>
                    {
                        return x * x;
                    };
                }
                else
                {
                    increase = int.Parse(operationElements[5]);
                    operation = x =>
                    {
                        return x * increase;
                    };
                }
            }
            else
            {
                increase = int.Parse(operationElements[5]);
                operation = x =>
                {
                    return x + increase;
                };
            }
        }
        if (elements[0].Trim().StartsWith("Test"))
        {
            var testElements = elements[1].Split(" ");
            int divisor = int.Parse(testElements[3]);
            test = x =>
            {
                return x % divisor == 0;
            };
        }
        if (elements[0].Trim().StartsWith("If true"))
        {
            var conditionelements = elements[1].Split(" ");
            targetMonkeys.Add($"Monkey-{conditionelements[4]}");
        }
        if (elements[0].Trim().StartsWith("If false"))
        {
            var conditionelements = elements[1].Split(" ");
            targetMonkeys.Add($"Monkey-{conditionelements[4]}");
        }

        if (simianCounter == 6)
        {
            monkeys.Add(monkeyName, new Monkey(monkeyName, startingItems, operation, test, targetMonkeys));
            monkeyName = "";
            startingItems = new Queue<int>();
            operation = null;
            test = null;
            targetMonkeys = new List<string>();
            simianCounter = 0;
        }
    });
    return monkeys;
}

int CalculateMonkeyBusiness(IEnumerable<Monkey> monkeys)
{
    var topMonkeys = monkeys.OrderByDescending(x => x.Inspections).Take(2).Select(x => x.Inspections).ToList();
    return topMonkeys[0] * topMonkeys[1];
}

void PlayRound(IDictionary<string, Monkey> monkeys, int numberOfRounds)
{
    for (int i = 0; i < numberOfRounds; i++)
    {
        foreach (var monkey in monkeys.Values)
        {
            PlayTurn(monkey, monkeys);


        }
        if (i == numberOfRounds)
        {
            break;
        }
    }

}

void PlayTurn(Monkey monkey, IDictionary<string, Monkey> monkeys)
{
    while (monkey.StartingItems.Count() > 0)
    {
        int thrownItem;
        string targetMonkey = monkey.ThrowItem(out thrownItem);
        monkeys[targetMonkey].StartingItems.Enqueue(thrownItem);
    }
}


class Monkey
{
    public string Name { get; }
    public Queue<int> StartingItems { get; }
    Func<int, int> Operation { get; }
    Func<int, bool> Test { get; }
    public List<string> TargetMonkeys { get; }
    public int Inspections { get; private set; }
    public Monkey(string name,
        Queue<int>
        startingItems,
        Func<int, int> operation,
        Func<int, bool> test,
        List<string> targetMonkeys)
    {
        Name = name;
        StartingItems = startingItems;
        Operation = operation;
        Test = test;
        TargetMonkeys = targetMonkeys;
        Inspections = 0;
    }

    public string ThrowItem(out int itemToThrow)
    {
        int item = StartingItems.Dequeue();
        item = InspectItem(item);
        item = ReduceWorry(item);
        itemToThrow = item;

        if (Test(item))
        {
            return TargetMonkeys.First();
        }
        return TargetMonkeys.Last();
    }

    private int InspectItem(int item)
    {
        item = Operation(item);
        Inspections++;
        return item;
    }

    private int ReduceWorry(int item)
    {
        return item / 3;
    }
}