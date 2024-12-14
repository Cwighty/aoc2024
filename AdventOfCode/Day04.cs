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
        // build grid
        // stop on each X
        // check right, down, left, up, diagnal for XMAS
        // if found, increment count
    }


    public int Solve2()
    {
        
    }

}
