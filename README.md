# Druaga Maze Generator (C# and VB.NET Implementation)

## Overview
This project replicates the maze generation algorithm from the classic arcade game, *The Tower of Druaga*. Implemented as a C# console application, it allows users to input a numerical seed ranging from -128 to 255 via the command line. Additionally, the VB.NET module `DruagaMaze.vb` is provided, with functionality identical to the C# version.

This dual-language implementation are designed to:
- provide game developers with a map-building solution that can be integrated into new projects
- demonstrate algorithm implementation and code optimization for programming enthusiasts
- offer a nostalgic experience for fans of classic games

[The original work](https://github.com/naoya2k/drumaze) is written in C, by a skillful Japanese developer [@naoya2k](https://github.com/naoya2k). Despite the language barrier when reading the comments in the C code, which were written all in Japanese, the essence of the algorithm is extracted to the fullest with the help of online translation tools.

For more details of the algorithm, see the `reference.c` file translated from Japanese into English.

## Features

1. **Dual-Language Support**
    - C# Version: A console application that leverages modern C# features to achieve high-performance and maintainable maze generation.
    - VB.NET Version: An alternative for developers familiar with Visual Basic syntax, with the same functionality as the C# version.
2. **Seed-Based Maze Generation**: Specify a seed value between -128 and 255 to generate a unique maze, suitable for creating diverse game levels.
3. **Easy Integration**: The modular algorithm design makes it easy to integrate into various projects that require maze generation.
4. **Educational Value**: Demonstrates the implementation of complex algorithms in two languages, facilitating learning in algorithm design and cross-language development.
5. **Nostalgic Appeal**: Replicates the classic maze from the game, evoking nostalgic memories for players.

## Installation and Usage
1. Prepare the .NET SDK 8 and a corresponding IDE (VS2022 or VS Code).
2. Clone and navigate to the project directory:
``` bash
git clone https://github.com/Pac-Dessert1436/DruagaMazeGenerator.git
cd DruagaMazeGenerator
```
3. Detailed usage as follows:
    - C# Version: Compile the project and run the executable. You can specify a seed value via the command line, for example:
    ``` bash
    dotnet run 50
    ```
    If no parameter is provided, the default seed value is 0. 
    - VB.NET Version: Add the `DruagaMaze.vb` file to your VB.NET project. You can call the following functions or properties to generate a maze: 
        - `DruagaMazeLayout` Property: Generates the maze layout based on the seed value. 
        - `Generate` Function: Generates maze data based on the seed. 
        - `BuildLayout` Function: Converts maze data into a visual layout. 
        - `Demonstrate` Function: Prints the maze layout for a specified seed.

## Contribution
Contributions to this project are welcome! You can participate through the standard GitHub process, including forking the repository, creating a new branch, and submitting a Pull Request.

## License
This project is licensed under the MIT License. For details, see the [LICENSE](LICENSE) file.