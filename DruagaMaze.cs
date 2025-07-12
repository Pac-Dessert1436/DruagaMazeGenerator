namespace DruagaMazeGenerator;

public static class DruagaMaze
{
    private const int MAZE_WIDTH = 18;
    private const int MAZE_HEIGHT = 9;

    private enum WallDirection : byte
    {
        Up = 0,
        Right = 1,
        Down = 2,
        Left = 3
    }

    internal static byte[,] Generate(byte seed)
    {
        byte[,] mapData = new byte[MAZE_HEIGHT, MAZE_WIDTH];
        byte currentSeed = seed;
        byte prev = (byte)(seed & 1);

        for (int x = MAZE_WIDTH - 2; x >= 0; x--)
        {
            for (int y = 0; y < MAZE_HEIGHT - 1; y++)
            {
                (int currX, int currY) = (x, y);

                if (CheckAndMarkPole(mapData, currX, currY))
                    continue;

                WallDirection wDir;
                do
                {
                    wDir = NextDirection(ref currentSeed, ref prev);
                } while (!MakeWall(mapData, ref currX, ref currY, wDir));
            }
        }
        return mapData;
    }

    private static bool CheckAndMarkPole(byte[,] mapData, int x, int y)
    {
        if (x < 0 || x >= MAZE_WIDTH - 1 || y < 0 || y >= MAZE_HEIGHT - 1)
            return true;

        return (mapData[y, x] & 1) != 0;
    }

    private static bool MakeWall(byte[,] mapData, ref int x, ref int y, WallDirection wDir)
    {
        mapData[y, x] |= 1;

        if (CheckWall(mapData, x, y, wDir))
            return false;

        switch (wDir)
        {
            case WallDirection.Up:
                mapData[y, x] |= 2;
                y--;
                break;
            case WallDirection.Right:
                if (x < MAZE_WIDTH - 1)
                    mapData[y, x + 1] |= 4;
                x++;
                break;
            case WallDirection.Down:
                if (y < MAZE_HEIGHT - 1)
                    mapData[y + 1, x] |= 2;
                y++;
                break;
            case WallDirection.Left:
                mapData[y, x] |= 4;
                x--;
                break;
        }
        return CheckAndMarkPole(mapData, x, y);
    }

    private static bool CheckWall(byte[,] mapData, int x, int y, WallDirection wDir)
    {
        switch (wDir)
        {
            case WallDirection.Up:
                return (mapData[y, x] & 2) != 0;
            case WallDirection.Right:
                if (x >= MAZE_WIDTH - 1)
                    return true;
                return (mapData[y, x + 1] & 4) != 0;
            case WallDirection.Down:
                if (y >= MAZE_HEIGHT - 1)
                    return true;
                return (mapData[y + 1, x] & 2) != 0;
            case WallDirection.Left:
                return (mapData[y, x] & 4) != 0;
            default:
                return true;
        }
    }

    private static WallDirection NextDirection(ref byte seed, ref byte prev)
    {
        byte r1 = (byte)(((seed >> 7) ^ (seed >> 4)) & 0x01);
        byte r2 = (byte)(r1 ^ 1);
        seed = (byte)((seed << 1) | r2);
        byte result = (byte)(((prev << 1) | r2) & 0x03);
        prev = r2;
        return (WallDirection)result;
    }

    internal static int[,] BuildLayout(this byte[,] mapData)
    {
        int rows = (MAZE_HEIGHT + 1) * 2;
        int cols = (MAZE_WIDTH + 1) * 2;
        int[,] result = new int[rows, cols];

        // Define the 2x2 blocks for each Unicode character
        List<int[,]> blocks = new List<int[,]>
        {
            new int[,] {{0, 0}, {0, 1}},  // Block 0: "▗"
            new int[,] {{0, 1}, {0, 1}},  // Block 1: "▐"
            new int[,] {{0, 0}, {1, 1}},  // Block 2: "▄"
            new int[,] {{0, 1}, {1, 1}},  // Block 3: "▟"
            new int[,] {{0, 0}, {0, 0}}   // Block 4: Empty
        };

        // Build top border (row 0 in character grid)
        for (int x = 0; x <= MAZE_WIDTH; x++)
        {
            int[,] block = (x == 0) ? blocks[0] : blocks[2];
            for (int r = 0; r < 2; r++)
            {
                for (int c = 0; c < 2; c++)
                {
                    result[r, x * 2 + c] = block[r, c];
                }
            }
        }

        // Build maze rows (character grid rows 1 to MAZE_HEIGHT)
        for (int y = 0; y < MAZE_HEIGHT; y++)
        {
            int rowStart = (y + 1) * 2;  // Start row in integer array

            // Left border block ("▐")
            for (int r = 0; r < 2; r++)
            {
                for (int c = 0; c < 2; c++)
                {
                    result[rowStart + r, c] = blocks[1][r, c];
                }
            }

            // Maze cells
            for (int x = 0; x < MAZE_WIDTH; x++)
            {
                byte currVal = (byte)(mapData[y, x] >> 1);
                if (y == MAZE_HEIGHT - 1) currVal |= 2;  // Force bottom wall
                if (x == MAZE_WIDTH - 1) currVal |= 1;   // Force right wall

                int blockIndex = currVal & 3;
                int[,] block = blockIndex < 4 ? blocks[blockIndex] : blocks[4];
                int colStart = (x + 1) * 2;

                for (int r = 0; r < 2; r++)
                {
                    for (int c = 0; c < 2; c++)
                    {
                        result[rowStart + r, colStart + c] = block[r, c];
                    }
                }
            }
        }
        return result;
    }
}