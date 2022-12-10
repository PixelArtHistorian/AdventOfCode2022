using System.Text;

SolvePuzzleOne("Input\\Day10.txt");
SolvePuzzleTwo("Input\\Day10.txt", 3);

void SolvePuzzleOne(string inputFilePath)
{
    int totalClocks = 0;
    int registryValue = 1;
    List<int> significantClocks = CreateSequence(20, 40, 6);
    List<int> significantSignalStrengths = new List<int>();
    File.ReadLines(Path.Combine(Environment.CurrentDirectory, inputFilePath)).ToList().ForEach(line =>
    {
        string[] commandElements = line.Split(" ");

        if (commandElements[0].StartsWith("noop"))
        {
            totalClocks++;
            RecordSignalStrength(totalClocks, registryValue, significantClocks, significantSignalStrengths);
        }
        else
        {
            int increase = int.Parse(commandElements[1]);
            for (int i = 0; i < 2; i++)
            {
                totalClocks++;
                RecordSignalStrength(totalClocks, registryValue, significantClocks, significantSignalStrengths);
            }
            registryValue += increase;
        }
    });
    Console.WriteLine($"{significantSignalStrengths.Sum()}");
}

void SolvePuzzleTwo(string inputFilePath, int spriteSize)
{
    int spriteLength = spriteSize;
    int rowClock = 0;
    int registryValue = 1;
    int rowEndIndicator = 40;
    StringBuilder renderedRow = new StringBuilder();
    List<string> crtOutput = new List<string>();
    File.ReadLines(Path.Combine(Environment.CurrentDirectory, inputFilePath)).ToList().ForEach(line =>
    {
        string[] commandElements = line.Split(" ");

        if (commandElements[0].StartsWith("noop"))
        {
            renderedRow.Append(RenderPixels(rowClock, registryValue, spriteLength));
            rowClock++;
            if (rowEndIndicator == rowClock)
            {
                crtOutput.Add(renderedRow.ToString());
                renderedRow = new StringBuilder();
                rowClock = 0;
            }
        }
        else
        {
            int increase = int.Parse(commandElements[1]);
            for (int i = 0; i < 2; i++)
            {
                renderedRow.Append(RenderPixels(rowClock, registryValue, spriteLength));
                rowClock++;
                if (rowEndIndicator == (rowClock))
                {
                    crtOutput.Add(renderedRow.ToString());
                    renderedRow = new StringBuilder();
                    rowClock = 0;
                }
            }
            registryValue += increase;
        }
    });
    foreach(var row in crtOutput)
    {
        Console.WriteLine(row);
    }
}

char RenderPixels(int clockValue, int spriteMiddlePosition, int spriteDimension)
{
    if(Math.Abs(clockValue - spriteMiddlePosition) <= spriteDimension / 2)
    {
        return '#';
    }
    return '.';
}

List<int> CreateSequence(int start, int interval, int length)
{
    List<int> sequence = new List<int>();
    sequence.Add(start);
    for (int i = 1; i < length; i++)
    {
        sequence.Add(start + interval * i);
    }
    return sequence;
}

void RecordSignalStrength(int totalClocks, int registryValue, List<int> significantClocks, List<int> significantSignalStrengths)
{
    if (significantClocks.Contains(totalClocks))
    {
        significantSignalStrengths.Add(totalClocks * registryValue);
    }
}