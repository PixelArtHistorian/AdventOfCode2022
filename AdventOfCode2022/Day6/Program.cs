SolvePuzzle(4);
SolvePuzzle(14);

static void SolvePuzzle(int markerLenght)
{
    Queue<char> marker = new Queue<char>();

    string buffer = File.ReadAllText(Path.Combine(Path.Combine(Environment.CurrentDirectory, "Input\\Day6.txt")));
    for (int i = 0; i < buffer.Length; i++)
    {
        var character = buffer[i];
        if (!marker.Contains(character))
        {
            marker.Enqueue(character);
            if (marker.Count == markerLenght)
            {
                Console.WriteLine($"Found marker {i + 1}");
                break;
            }
        }
        else
        {
            do
            {
                marker.Dequeue();
            } 
            while (marker.Contains(character));
            marker.Enqueue(character);
        }
    }
}
