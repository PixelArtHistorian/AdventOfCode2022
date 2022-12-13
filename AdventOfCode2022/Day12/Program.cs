using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Xml.Linq;

GridTile start;
GridTile end;
var grid = CreateGrid("Input\\Example.txt", out start, out end);

DjikstraSearch(start, end);
List<GridTile> shortestPath = new List<GridTile>();
BuildShortestPath(shortestPath, end);
PrintGrid(grid, start.GetConnectedTiles());

void DjikstraSearch(GridTile start, GridTile end)
{
    HashSet<GridTile> connectedTiles = start.GetConnectedTiles();
    HashSet<GridTile> visitedTiles = new HashSet<GridTile>();
    foreach (var tile in grid)
    {
        tile.DistanceFromStart = int.MaxValue;
    }
    start.DistanceFromStart = 0;
    GridTile exploredTile = new GridTile(0, 0, 0);
    while (!connectedTiles.All(x=>visitedTiles.Contains(x)))
    {
        exploredTile = grid.OrderBy(x => x.DistanceFromStart)
            .Where(x => !visitedTiles.Contains(x))
            .First();
        visitedTiles.Add(exploredTile);
        foreach (var accesibleTile in exploredTile.AccessibleTiles)
        {
            accesibleTile.PrecedingTile = exploredTile;
            accesibleTile.DistanceFromStart = 1 + exploredTile.DistanceFromStart;
        }
    }
}

void BuildShortestPath(List<GridTile> path , GridTile tile)
{
    if (tile.PrecedingTile == null)
    {
        return;
    }
    path.Add(tile.PrecedingTile);
    BuildShortestPath(path, tile.PrecedingTile);
}
List<GridTile> CreateGrid(string inputFilePath, out GridTile start, out GridTile end)
{
    List<GridTile> grid = new List<GridTile>();
    start = new GridTile(0, 0, 0);
    end = new GridTile(0, 0, 0); ;
    int rowCounter = 0;
    int columnCounter = 0;
    var rows = File.ReadLines(inputFilePath);

    foreach (var row in rows)
    {
        foreach (char c in row)
        {
            GridTile gridTile;
            if (c == 'S')
            {
                gridTile = new GridTile(columnCounter, rowCounter, (int)'a' - 96);
                start = gridTile;
            }
            else if (c == 'E')
            {
                gridTile = new GridTile(columnCounter, rowCounter, (int)'z' - 96);
                end = gridTile;
            }
            else
            {
                gridTile = new GridTile(columnCounter, rowCounter, (int)c - 96);
            }
            grid.ForEach(tile =>
            {
                tile.AddIfAccessible(gridTile);
            });
            grid.Add(gridTile);
            columnCounter++;
        }
        rowCounter++;
        columnCounter = 0;
    }

    return grid;
}

void PrintGrid(List<GridTile> grid, HashSet<GridTile> gridTiles)
{
    int width = grid.Max(j => j.X) + 1;
    StringBuilder stringBuilder = new StringBuilder();
    for (int j = 0; j < grid.Count; j++)
    {
        if (j % width == 0 && j > 0)
        {
            stringBuilder.AppendLine();
        }
        if (gridTiles.Contains(grid[j]))
        {
            stringBuilder.Append("1");   
        }
        else if (!gridTiles.Contains(grid[j]))
        {
            stringBuilder.Append("0");
        }
    }
    Console.WriteLine(stringBuilder.ToString());
}


class GridTile
{
    public int X { get; }
    public int Y { get; }
    public int Height { get; }
    public List<GridTile> AccessibleTiles { get; }
    public GridTile? PrecedingTile { get; set; }
    public int DistanceToDestination { get; set; }
    public int DistanceFromStart { get; set; }
    public GridTile(int x, int y, int height)
    {
        X = x;
        Y = y;
        Height = height;
        AccessibleTiles = new List<GridTile>();
    }
    public bool AddIfAccessible(GridTile gridTile)
    {
        if (Math.Abs(gridTile.Height - Height) > 1)
        {
            return false;
        }
        if (!(Math.Abs(gridTile.X - X) + Math.Abs(gridTile.Y - Y) == 1))
        {
            return false;
        }
        AccessibleTiles.Add(gridTile);
        return true;
    }

    public void SetDistanceToDestination(GridTile destination)
    {
        DistanceToDestination = (X - destination.X * X - destination.X) + (Y - destination.Y * Y - destination.Y);
    }

    public HashSet<GridTile> GetConnectedTiles()
    {
        List<GridTile> tilesConnectedToStart = AccessibleTiles.ToList();
        foreach(var tile in AccessibleTiles)
        {    
            tilesConnectedToStart.AddRange(tile.GetConnectedTiles());
        }
        return tilesConnectedToStart.ToHashSet();
    }
}