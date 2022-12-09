SolvePuzzle("Input\\Day9.txt", 2);
SolvePuzzle("Input\\Day9.txt", 10);

void SolvePuzzle(string inputFilePath, int numberOfKnots)
{
    List<Tuple<int, int>> tailPositions = new List<Tuple<int, int>>();
    Rope rope = new Rope(numberOfKnots);
    File.ReadLines(Path.Combine(Path.Combine(Environment.CurrentDirectory, inputFilePath))).ToList().ForEach(line =>
    {
        var elements = line.Split(" ");
        Direction direction = (Direction)Enum.Parse(typeof(Direction), elements[0]);
        int distance = int.Parse(elements[1]);
        for (int i = 1; i <= distance; i++)
        {
            rope.SetKnotsPosition(direction);
            if (!tailPositions.Contains(rope.Knots.Last()))
            {
                tailPositions.Add(rope.Knots.Last());
            }
        }
    });
    Console.WriteLine(tailPositions.Count());
}

class Rope
{
    public List<Tuple<int, int>> Knots { get; private set; }
    public Rope(int numberOfKnots)
    {
        Knots = new List<Tuple<int, int>>();
        for (int i = 0; i < numberOfKnots; i++)
        {
            Knots.Add(new Tuple<int, int>(0, 0));
        }
    }
    public void SetKnotsPosition(Direction direction)
    {
        Knots[0] = CreateNextHeadPosition(Knots[0], direction);
        for (int i = 1; i < Knots.Count(); i++)
        {
            if (!AreAdjacent(Knots[i], Knots[i - 1]))
            {
                if (Knots[i].Item1 == Knots[i - 1].Item1 || Knots[i].Item2 == Knots[i - 1].Item2)
                {
                    Knots[i] = CreateNextLinearPosition(Knots[i], Knots[i - 1]);
                }
                else
                {
                    Knots[i] = CreateNextDiagonalPosition(Knots[i], Knots[i - 1]);
                }
            }
        }
    }

    private Tuple<int, int> CreateNextHeadPosition(Tuple<int, int> knot, Direction direction)
    {
        Tuple<int, int> nextPosition = knot;
        switch (direction)
        {
            case Direction.R:
                nextPosition = new Tuple<int, int>(knot.Item1 + 1, knot.Item2);
                break;
            case Direction.D:
                nextPosition = new Tuple<int, int>(knot.Item1, knot.Item2 - 1);
                break;
            case Direction.L:
                nextPosition = new Tuple<int, int>(knot.Item1 - 1, knot.Item2);
                break;
            case Direction.U:
                nextPosition = new Tuple<int, int>(knot.Item1, knot.Item2 + 1);
                break;
            default:
                break;
        }
        return nextPosition;
    }

    private Tuple<int, int> CreateNextLinearPosition(Tuple<int, int> knot, Tuple<int, int> knotToReach)
    {
        Tuple<int, int> nextPosition = knot;
        if (knotToReach.Item2 > knot.Item2)
        {
            nextPosition = new Tuple<int, int>(knot.Item1, knot.Item2 + 1);
        }
        else if (knotToReach.Item2 < knot.Item2)
        {
            nextPosition = new Tuple<int, int>(knot.Item1, knot.Item2 - 1);
        }
        else if (knotToReach.Item1 < knot.Item1)
        {
            nextPosition = new Tuple<int, int>(knot.Item1 - 1, knot.Item2);
        }
        else if (knotToReach.Item1 > knot.Item1)
        {
            nextPosition = new Tuple<int, int>(knot.Item1 + 1, knot.Item2);
        }
        return nextPosition;
    }

    private Tuple<int, int> CreateNextDiagonalPosition(Tuple<int, int> knot, Tuple<int, int> knotToReach)
    {
        Tuple<int, int> nextPosition = knot;
        if (knotToReach.Item1 > knot.Item1 && knotToReach.Item2 > knot.Item2)
        {
            nextPosition = new Tuple<int, int>(knot.Item1 + 1, knot.Item2 + 1);
        }
        else if (knotToReach.Item1 > knot.Item1 && knotToReach.Item2 < knot.Item2)
        {
            nextPosition = new Tuple<int, int>(knot.Item1 + 1, knot.Item2 - 1);
        }
        else if (knotToReach.Item1 < knot.Item1 && knotToReach.Item2 < knot.Item2)
        {
            nextPosition = new Tuple<int, int>(knot.Item1 - 1, knot.Item2 - 1);
        }
        else if (knotToReach.Item1 < knot.Item1 && knotToReach.Item2 > knot.Item2)
        {
            nextPosition = new Tuple<int, int>(knot.Item1 - 1, knot.Item2 + 1);
        }
        return nextPosition;
    }

    private bool AreAdjacent(Tuple<int, int> knot, Tuple<int, int> knotToReach)
    {
        return (Math.Abs(knot.Item1 - knotToReach.Item1) <= 1 && Math.Abs(knot.Item2 - knotToReach.Item2) <= 1);
    }
}

enum Direction
{
    R,
    D,
    L,
    U,
}