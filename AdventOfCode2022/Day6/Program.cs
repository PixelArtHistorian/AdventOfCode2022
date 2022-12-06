SolvePuzzle1();


static void SolvePuzzle1()
{
    Queue<char> marker = new Queue<char>();

    string buffer = File.ReadAllText("C:\\Users\\Alberto\\Desktop\\Development\\AdventOfCode2022\\Input\\Day6.txt");

    foreach(char character in buffer)
    {
        if (!marker.Contains(character))
        {
            marker.Enqueue(character);
            if(marker.Count == 4)
            {
                Console.WriteLine($"Found marker {buffer.IndexOf(character) + 1}");
            }
        }
        else
        {
            char repeatedCharacter;
            do
            {
                repeatedCharacter = marker.Dequeue();
            } 
            while (repeatedCharacter != character);
        }
    }


}
