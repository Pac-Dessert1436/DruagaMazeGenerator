namespace DruagaMazeGenerator;

public static class Program
{
    public static int[,] GetDruagaMazeLayout(int seed)
    {
        return seed >= -128 && seed <= 255 ?
            DruagaMaze.Generate((byte)seed).BuildLayout() :
            throw new ArgumentException("Seed value out of range; must be within [-128, 255].");
    }

    public static void Demonstrate(int seed)
    {
        int[,] layout = GetDruagaMazeLayout(seed);
        int floor = ((sbyte)seed + 60) % 60 + 1;
        Console.WriteLine($"FLOOR {floor} (seed={(sbyte)seed})");
        for (int y = 0; y < layout.GetLength(0); y++)
        {
            for (int x = 0; x < layout.GetLength(1); x++)
            {
                Console.Write(layout[y, x] == 1 ? '@' : ' ');
            }
            Console.WriteLine();
        }
    }

    internal static void Main(string[] args)
    {
        try
        {
            int seed = 0;
            if (args.Length == 1)
                seed = int.Parse(args[0]);
            else if (args.Length > 0)
            {
                throw new ArgumentException(
                    "Invalid argument count. Please enter a seed value within [-128, 255].");
            }
            Demonstrate(seed);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            Console.WriteLine("Seed for the maze generation defaults to 0.");
            Console.WriteLine(new string('-', 40));
            Demonstrate(0);  // Fall back to 0 on failure
        }
    }
}
