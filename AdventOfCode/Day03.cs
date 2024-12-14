using System.Text.RegularExpressions;
namespace AdventOfCode;

public class Day03 : BaseDay
{
    private readonly string[] _input;

    public Day03()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override ValueTask<string> Solve_1() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 1 {Solve1()}");

    public override ValueTask<string> Solve_2() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 2 {Solve2()}");

    int Solve1()
    {
        string pattern = @"mul\((\d+\,\d+)\)";
        int total = 0;

        foreach (var input in _input)
        {
            var matches = Regex.Matches(input, pattern);
            foreach (var match in matches)
            {
                var values = match.ToString().Split("(")[1].Split(")")[0].Split(",");
                var a = int.Parse(values[0]);
                var b = int.Parse(values[1]);
                total += a * b;
            }
        }

        return total;
    }


    public int Solve2()
    {
        string pattern = @"\bdo\(\)|\bdon't\(\)|mul\((\d+),(\d+)\)";

        var allMatches = new List<Match>();
        foreach (var input in _input)
        {
            var matches = Regex.Matches(input, pattern);
            allMatches.AddRange(matches);
        }

        int total = 0;
        var doit = true;
        foreach (var match in allMatches)
        {
            if (match.ToString().StartsWith("mul"))
            {
                if (doit)
                {
                    var values = match.ToString().Split("(")[1].Split(")")[0].Split(",");
                    var a = int.Parse(values[0]);
                    var b = int.Parse(values[1]);
                    total += a * b;
                }
            }
            else if (match.ToString().StartsWith("don"))
            {
                doit = false;
            }
            else
            {
                doit = true;
            }
        }

        return total;
    }

}
