using System.Collections.Generic;
using System.Linq;

SolvePuzzleOne("Input\\Day8.txt");
SolvePuzzleTwo("Input\\Day8.txt");

void SolvePuzzleOne(string inputFilePath)
{
    var forest = GenerateForest(inputFilePath);
    Console.WriteLine(forest.Count() - CountHiddenTrees(forest.ToList()));
}

void SolvePuzzleTwo(string inputFilePath)
{
    var forest = GenerateForest(inputFilePath);
    Console.WriteLine(FindBestViewingFactor(forest));
}

List<Tree> GenerateForest(string inputFilePath)
{
    List<Tree> forest = new List<Tree>();
    List<string> inputLines = File.ReadLines(Path.Combine(Path.Combine(Environment.CurrentDirectory, inputFilePath))).ToList();
    for(int y = 0; y < inputLines.Count(); y++)
    {
        string line = inputLines[y];
        for (int x = 0; x < line.Length; x++)
        {
            bool isOnEdge = (x == 0 || x == line.Length - 1) || (y == 0 || y == inputLines.Count() - 1);
            forest.Add(new Tree(int.Parse(line[x].ToString()), x, y, isOnEdge));
        }
    }
    return forest;
}


int CountHiddenTrees(List<Tree> forest)
{
    int hiddenTrees = 0;

    foreach(Tree tree in forest.Where(tree => !tree.IsOnEdge))
    {
        var left = forest.Where(x => x.Y == tree.Y && x.X < tree.X).ToList();
        var right = forest.Where(x => x.Y == tree.Y && x.X > tree.X).ToList();
        var up = forest.Where(x => x.X == tree.X && x.Y < tree.Y).ToList();
        var down = forest.Where(x => x.X == tree.X && x.Y > tree.Y).ToList();
        if (IsHidden(tree, left) && IsHidden(tree, right) && IsHidden(tree, up) && IsHidden(tree, down))
        {
            hiddenTrees++;
        }
    }
    return hiddenTrees;
}
bool IsHidden(Tree tree, List<Tree> line)
{
    bool isHidden = false;
    if (tree.Height <= line.First().Height)
    {
        return true;
    }
    else if(line.Count > 1) 
    {
        isHidden = IsHidden(tree, line.Skip(1).ToList()); ;
    }
    return isHidden;
}

int FindBestViewingFactor(List<Tree> forest)
{
    int bestViewingFactor = 0;
    foreach (Tree tree in forest.Where(tree => !tree.IsOnEdge))
    {
        var left = forest.Where(x => x.Y == tree.Y && x.X < tree.X).OrderByDescending(x => x .X).ToList();
        var right = forest.Where(x => x.Y == tree.Y && x.X > tree.X).OrderBy(x => x.X).ToList();
        var up = forest.Where(x => x.X == tree.X && x.Y < tree.Y).OrderByDescending(x => x.Y).ToList();
        var down = forest.Where(x => x.X == tree.X && x.Y > tree.Y).OrderBy(x => x.Y).ToList();
        int viewingFactor = FindLineOfSightLenght(tree, left) * FindLineOfSightLenght(tree, right) * FindLineOfSightLenght(tree, up) * FindLineOfSightLenght(tree, down);
        if (bestViewingFactor < viewingFactor)
        {
            bestViewingFactor = viewingFactor;
        }
    }
    return bestViewingFactor;
}

int FindLineOfSightLenght(Tree tree, List<Tree> line)
{
    int lineOfSightLenght = 1;
    if(line.Count > 1 && tree.Height > line.First().Height)
    {
        lineOfSightLenght += FindLineOfSightLenght(tree, line.Skip(1).ToList());
    }
    return lineOfSightLenght;
}


class Tree
{
    public int Height { get; }
    public int X { get; }
    public int Y { get; }
    public bool IsOnEdge { get; }

    public Tree(int height, int x, int y, bool isOnEdge)
    {
        Height = height;
        X = x;
        Y = y;
        IsOnEdge = isOnEdge;
    }
}