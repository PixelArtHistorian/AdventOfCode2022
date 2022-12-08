SolvePuzzleOne("Input\\Day7.txt", 100000);
SolvePuzzleTwo("Input\\Day7.txt", 30000000, 70000000);
void SolvePuzzleOne(string inputFilePath, int maxDirectorySize)
{
    List<AdventDirectory> undersizedDirectories = FindUndersizedDirectories(BuildRootDirectory(inputFilePath), maxDirectorySize);
    Console.WriteLine(undersizedDirectories.Sum(d => d.GetDirectorySize()));
}

void SolvePuzzleTwo(string inputFilePath, int updateSize, int availableSpace)
{
    AdventDirectory root = BuildRootDirectory(inputFilePath);
    int freeSpace = availableSpace - root.GetDirectorySize();
    List<AdventDirectory> oversizedDirectorries = FindOverSizedDirectories(root, Math.Abs(updateSize - freeSpace));
    Console.WriteLine(oversizedDirectorries.Min(d => d.GetDirectorySize()));
}

AdventDirectory BuildRootDirectory(string inptuFilePath)
{
    AdventDirectory currentDirectory = new AdventDirectory("/", null, new List<AdventDirectory>(), new List<AdventFile>());

    File.ReadLines(Path.Combine(Path.Combine(Environment.CurrentDirectory, inptuFilePath))).ToList().ForEach(line =>
    {
        var lineElements = line.Split(" ");
        if (lineElements[0].StartsWith("$"))
        {
            if (lineElements[1].StartsWith("cd"))
            {
                if (lineElements[2].StartsWith(".."))
                {
                    currentDirectory = currentDirectory.ParentDirectory;
                }
                else if (!lineElements[2].StartsWith("/"))
                {
                    currentDirectory = currentDirectory.ChildDirectories.First(x => x.Name.StartsWith(lineElements[2]));
                }
            }
        }
        else if (lineElements[0].StartsWith("dir"))
        {
            currentDirectory.ChildDirectories?
            .Add(new AdventDirectory(lineElements[1], currentDirectory, new List<AdventDirectory>(), new List<AdventFile>()));
        }
        else
        {
            currentDirectory.Files?
            .Add(new AdventFile(lineElements[1], int.Parse(lineElements[0])));
        }
    });
    return FindRootDirectory(currentDirectory);
}

List<AdventDirectory> FindUndersizedDirectories(AdventDirectory currentDirectory, int size)
{
    List<AdventDirectory> undersizedDirectories = new List<AdventDirectory>();
    foreach (var directory in currentDirectory.ChildDirectories)
    {
        if (directory.GetDirectorySize() < size)
        {
            undersizedDirectories.Add(directory);
        }
        undersizedDirectories.AddRange(FindUndersizedDirectories(directory, size));

    }
    return undersizedDirectories;
}

List<AdventDirectory> FindOverSizedDirectories(AdventDirectory currentDirectory, int size)
{
    List<AdventDirectory> overSizedDirectories = new List<AdventDirectory>();
    foreach (var directory in currentDirectory.ChildDirectories)
    {
        if (directory.GetDirectorySize() > size)
        {
            overSizedDirectories.Add(directory);
        }
        overSizedDirectories.AddRange(FindOverSizedDirectories(directory, size));

    }
    return overSizedDirectories;
}

AdventDirectory FindRootDirectory(AdventDirectory currentDirectory)
{
    if (currentDirectory.ParentDirectory == null)
    {
        return currentDirectory;
    }
    else
    {
        AdventDirectory parentDirectory = currentDirectory.ParentDirectory;
        return FindRootDirectory(parentDirectory);
    }
}

class AdventDirectory
{
    public string Name { get; set; }
    public AdventDirectory? ParentDirectory { get; set; }
    public List<AdventDirectory> ChildDirectories { get; set; }
    public List<AdventFile> Files { get; set; }
    public AdventDirectory(string name, AdventDirectory? parentDirectory, List<AdventDirectory> childDirectories, List<AdventFile> files)
    {
        Name = name;
        ParentDirectory = parentDirectory;
        ChildDirectories = childDirectories;
        Files = files;
    }

    public int GetDirectorySize()
    {
        return Files.Sum(f => f.Size) + ChildDirectories.Sum(d => d.GetDirectorySize());
    }
}

public class AdventFile
{
    public string Name { get; set; }
    public int Size { get; set; }
    public AdventFile(string name, int size)
    {
        Name = name;
        Size = size;
    }
}