var monkeys = CreateMonkeyList("Input\\Day11.txt");
PlayRound(monkeys, 10000);
Console.WriteLine($"{CalculateMonkeyBusiness(monkeys.Values)}");


IDictionary<string, Monkey> CreateMonkeyList(string inputFile)
{
    Dictionary<string, Monkey> monkeys = new Dictionary<string, Monkey>();
    string monkeyName = "";
    Queue<long> startingItems = new Queue<long>();
    Func<long, long>? operation = null;
    Func<long, bool>? test = null;
    List<string> targetMonkeys = new List<string>();
    long simianCounter = 0;
    long worryManagementIndex = 1;

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
                startingItems.Enqueue(long.Parse(item));
            }
        }
        if (elements[0].Trim().StartsWith("Operation"))
        {
            var operationElements = elements[1].Split(" ");
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
                    var coefficient = long.Parse(operationElements[5]);
                    operation = x =>
                    {
                        return x * coefficient;
                    };
                }
            }
            else
            {
                var increase = long.Parse(operationElements[5]);
                operation = x =>
                {
                    return x + increase;
                };
            }
        }
        if (elements[0].Trim().StartsWith("Test"))
        {
            var testElements = elements[1].Split(" ");
            long divisor = long.Parse(testElements[3]);
            test = x =>
            {
                return x % divisor == 0;
            };
            worryManagementIndex *= divisor;
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
            startingItems = new Queue<long>();
            operation = null;
            test = null;
            targetMonkeys = new List<string>();
            simianCounter = 0;
        }
    });
    for (long i = 0; i < monkeys.Count(); i++)
    {
        monkeys[$"Monkey-{i}"].WorryManagementIndex = worryManagementIndex;
    }
    return monkeys;
}

long CalculateMonkeyBusiness(IEnumerable<Monkey> monkeys)
{
    var topMonkeys = monkeys.OrderByDescending(x => x.Inspections).Take(2).Select(x => x.Inspections).ToList();
    return (long)topMonkeys[0] * (long)topMonkeys[1];
}

void PlayRound(IDictionary<string, Monkey> monkeys, long numberOfRounds)
{
    for (long i = 0; i < numberOfRounds; i++)
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
        long thrownItem;
        string targetMonkey = monkey.ThrowItem(out thrownItem);
        monkeys[targetMonkey].StartingItems.Enqueue(thrownItem);
    }
}


class Monkey
{
    public string Name { get; }
    public Queue<long> StartingItems { get; }
    Func<long, long> Operation { get; }
    Func<long, bool> Test { get; }
    public List<string> TargetMonkeys { get; }
    public long Inspections { get; private set; }
    public long WorryManagementIndex { get; set; } = 1;
    public Monkey(string name,
        Queue<long> startingItems,
        Func<long, long> operation,
        Func<long, bool> test,
        List<string> targetMonkeys)
    {
        Name = name;
        StartingItems = startingItems;
        Operation = operation;
        Test = test;
        TargetMonkeys = targetMonkeys;
        Inspections = 0;
    }

    public string ThrowItem(out long itemToThrow)
    {
        long item = StartingItems.Dequeue();
        item = InspectItem(item);
        item = ReduceWorry(item);
        Inspections++;
        itemToThrow = item;

        if (Test(itemToThrow))
        {
            return TargetMonkeys.First();
        }
        return TargetMonkeys.Last();
    }

    private long InspectItem(long item)
    {
        item = Operation(item);
        return item;
    }

    private long ReduceWorry(long item)
    {
        //return item / 3;
        return item % WorryManagementIndex;
    }
}