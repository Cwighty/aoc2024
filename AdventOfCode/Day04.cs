using System.Text.RegularExpressions;
namespace AdventOfCode;

public class Day04 : BaseDay
{
    private readonly string[] _input;

    public Day04()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override ValueTask<string> Solve_1() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 1 {Solve1()}");

    public override ValueTask<string> Solve_2() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 2 {Solve2()}");

    int Solve1()
    {

        List<List<char>> grid = new();
        foreach (var line in _input)
        {
            var row = new List<char>();
            row.AddRange(line);
            grid.Add(row);
        }

        int total = 0;
        for (int r = 0; r < grid[0].Count(); r++)
        {
            for (int c = 0; c < grid.Count(); c++)
            {
                var letter = grid[r][c];
                if (letter == 'X')
                    total += countXmasInGrid(r, c, grid);
            }
        }

        return total;
    }

    private int countXmasInGrid(int x, int y, List<List<char>> grid)
    {
        var directions = new List<(int x, int y)>{
        (0,1), (0,-1), (1, 0), (-1, 0),
          (1, 1), (1, -1), (-1, 1), (-1, -1)
        };

        int count = 0;
        string word = "XMAS";
        foreach (var direction in directions)
        {
            for (int i = 1; i < word.Length; i++)
            {
                var dx = x + (direction.x * i);
                var dy = y + (direction.y * i);

                if (dx >= grid[0].Count() || dx < 0)
                    break;
                if (dy >= grid.Count() || dy < 0)
                    break;

                var nextLetter = grid[dx][dy];
                if (nextLetter != word[i])
                    break;

                if (nextLetter == word[i] && i == word.Length - 1)
                    count++;
            }
        }

        return count;
    }


    public int Solve2()
    {
        List<List<char>> grid = new();
        foreach (var line in _input)
        {
            var row = new List<char>();
            row.AddRange(line);
            grid.Add(row);
        }

        int total = 0;
        for (int r = 0; r < grid[0].Count(); r++)
        {
            for (int c = 0; c < grid.Count(); c++)
            {
                var letter = grid[r][c];
                if (letter == 'A')
                {
                    if (xMasMarksTheSpot(r, c, grid))
                        total += 1;
                }
            }
        }

        return total;
    }

    private bool xMasMarksTheSpot(int x, int y, List<List<char>> grid)
    {
        var directions = new List<(int x, int y)>{
          (1, 1), (-1, -1), (1, -1), (-1, 1),
        };

        int correctDiags = 0;
        int loop = 0;
        char topCorner = ' ';
        char bottomCorner;
        foreach (var direction in directions)
        {
            var dx = x + (direction.x);
            var dy = y + (direction.y);

            if (dx >= grid[0].Count() || dx < 0)
                break;
            if (dy >= grid.Count() || dy < 0)
                break;

            if (loop % 2 == 0)
            {
                topCorner = grid[dx][dy];
            }
            else
            {
                bottomCorner = grid[dx][dy];
                if (isOpposite(topCorner, bottomCorner))
                    correctDiags++;
            }
            loop++;
        }

        if (correctDiags == 2)
            return true;

        return false;
    }

    private bool isOpposite(char topCorner, char bottomCorner)
    {
        if (topCorner == 'M' && bottomCorner == 'S')
            return true;
        if (topCorner == 'S' && bottomCorner == 'M')
            return true;

        return false;
    }
}
