using System.ComponentModel;

var monkeys = CreateMonkeyList("Input\\Example.txt");
PlayRound(monkeys, 10000);
Console.WriteLine($"{CalculateMonkeyBusiness(monkeys.Values)}");


IDictionary<string, Monkey> CreateMonkeyList(string inputFile)
{
    Dictionary<string, Monkey> monkeys = new Dictionary<string, Monkey>();
    string monkeyName = "";
    Queue<ItemToThrow> startingItems = new Queue<ItemToThrow>();
    Func<ItemToThrow, ItemToThrow>? operation = null;
    Func<ItemToThrow, bool>? test = null;
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
                startingItems.Enqueue(new ItemToThrow(int.Parse(item)));
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
                        x.SquareWorries();
                        return x;
                    };
                }
                else
                {
                    var coefficient = int.Parse(operationElements[5]);
                    operation = x =>
                    {
                        x.MultiplyWorriesBy(coefficient);
                        return x;
                    };
                }
            }
            else
            {
                var increase = int.Parse(operationElements[5]);
                operation = x =>
                {
                    x.AddWorries(increase);
                    return x;
                };
            }
        }
        if (elements[0].Trim().StartsWith("Test"))
        {
            var testElements = elements[1].Split(" ");
            int divisor = int.Parse(testElements[3]);
            test = x =>
            {
                return x.IsDivisibleBy(divisor);
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
            startingItems = new Queue<ItemToThrow>();
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
        if (i == 0 || i == 19 || i == 999)
        {
            Console.WriteLine($"{monkeys["Monkey-0"].Inspections}, {monkeys["Monkey-1"].Inspections}, {monkeys["Monkey-2"].Inspections}, {monkeys["Monkey-3"].Inspections}");
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
        ItemToThrow thrownItem;
        string targetMonkey = monkey.ThrowItem(out thrownItem);
        monkeys[targetMonkey].StartingItems.Enqueue(thrownItem);
    }
}


class Monkey
{
    public string Name { get; }
    public Queue<ItemToThrow> StartingItems { get; }
    Func<ItemToThrow, ItemToThrow> Operation { get; }
    Func<ItemToThrow, bool> Test { get; }
    public List<string> TargetMonkeys { get; }
    public int Inspections { get; private set; }
    public Monkey(string name,
        Queue<ItemToThrow> startingItems,
        Func<ItemToThrow, ItemToThrow> operation,
        Func<ItemToThrow, bool> test,
        List<string> targetMonkeys)
    {
        Name = name;
        StartingItems = startingItems;
        Operation = operation;
        Test = test;
        TargetMonkeys = targetMonkeys;
        Inspections = 0;
    }

    public string ThrowItem(out ItemToThrow itemToThrow)
    {
        ItemToThrow item = StartingItems.Dequeue();
        item = InspectItem(item);
        //item = ReduceWorry(item);
        itemToThrow = item;

        if (Test(item))
        {
            return TargetMonkeys.First();
        }
        return TargetMonkeys.Last();
    }

    private ItemToThrow InspectItem(ItemToThrow item)
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
class ItemToThrow
{
    private List<int> Worries { get; set; } = new List<int>();

    public ItemToThrow(int number)
    {
        Worries.Add(number);
    }

    public bool IsDivisibleBy(int divisor)
    {
        if(Worries.Count > 1)
        {
            int divisionCounter = 1;
            foreach(var worry in Worries)
            {
                for (int i = 0; i < worry; i++)
                {
                    if(divisionCounter == divisor)
                    {
                        divisionCounter = 0;
                    }
                    divisionCounter++;
                }
            }
            return divisionCounter == divisor;
        }
        return Worries.First() % divisor == 0;
    }

    public void AddWorries(int worry)
    {
        if(int.MaxValue - Worries.Last() < worry)
        {
            Worries.Add(worry);
        }
        else
        {
            Worries[Worries.Count() - 1] += worry;
        }
    }

    public void SquareWorries()
    {
        if(Worries.Count > 1)
        {
            for (int i = 0; i < Worries.Count; i++)
            {
                int increment = Worries[i];
                for (int j = 0; j < increment - 1; j++)
                {
                    AddWorries(increment);
                }
            }
        }
        Worries[0] *= Worries[0];
    }

    public void MultiplyWorriesBy(int coefficient)
    {
        for (int i = 0; i < Worries.Count; i++)
        {
            int increment = Worries[i];
            for (int j = 0; j < coefficient - 1; j++)
            {
                AddWorries(increment);
            }
        }
    }
}