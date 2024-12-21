using System.Text.RegularExpressions;
namespace AdventOfCode;

public class Day07 : BaseDay
{
    private readonly string[] _input;

    public Day07()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override ValueTask<string> Solve_1() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 1 {Solve1()}");

    public override ValueTask<string> Solve_2() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 2 {Solve2()}");

    public Dictionary<long, HashSet<long>> dp = new();

    public long Operate(long a, long b, Func<long, long, long> op)
    {
        return op(a, b);
    }

    public List<Func<long, long, long>> Operations = new(){
      (long a, long b) => a + b,
      (long a, long b) => a * b,
      (long a, long b) => long.Parse(a.ToString() + b.ToString()),
    };

    long Solve1()
    {
        long validSum = 0;
        foreach (var line in _input)
        {
            var target = long.Parse(line.Split(':').First());
            var nums = line.Split(':')[1].Trim().Split(' ').Select(n => long.Parse(n)).ToList();

            dp[0] = new() { nums.First() };

            for (int i = 1; i < nums.Count(); i++)
            {
                var prev = dp[i - 1];
                var curr = nums[i];

                var next = new HashSet<long>();
                foreach (var num in prev)
                {
                    foreach (var op in Operations.Take(2))
                    {
                        next.Add(op(num, curr));
                    }
                }

                dp[i] = next;
            }

            if (dp[nums.Count() - 1].Contains(target))
            {
                validSum += target;
            }
        }

        return validSum;
    }

    public long Solve2()
    {
        long validSum = 0;
        foreach (var line in _input)
        {
            var target = long.Parse(line.Split(':').First());
            var nums = line.Split(':')[1].Trim().Split(' ').Select(n => long.Parse(n)).ToList();

            dp[0] = new() { nums.First() };

            for (int i = 1; i < nums.Count(); i++)
            {
                var prev = dp[i - 1];
                var curr = nums[i];

                var next = new HashSet<long>();
                foreach (var num in prev)
                {
                    foreach (var op in Operations)
                    {
                        next.Add(op(num, curr));
                    }
                }

                dp[i] = next;
            }

            if (dp[nums.Count() - 1].Contains(target))
            {
                validSum += target;
            }
        }

        return validSum;
    }

}


