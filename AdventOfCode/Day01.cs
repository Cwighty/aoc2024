namespace AdventOfCode;

public class Day01 : BaseDay
{
    private readonly string[] _input;

    public Day01()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override ValueTask<string> Solve_1() => new($"{Solve()}");

    public override ValueTask<string> Solve_2() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 2 {Solve2()}");

    public int Solve()
    {
        var left = new List<int>();
        var right = new List<int>();
        foreach (var line in _input)
        {
            var nums = line.Split("   ");
            left.Add(int.Parse(nums[0]));
            right.Add(int.Parse(nums[1]));
        }

        left.Sort();
        right.Sort();

        var sum = 0;
        for (int i = 0; i < left.Count(); i++)
        {
            var diff = Math.Abs(left[i] - right[i]);
            sum += diff;
        }

        return sum;
    }

    public int Solve2()
    {
        var left = new List<int>();
        var right = new Dictionary<int, int>();
        foreach (var line in _input)
        {
            var nums = line.Split("   ");
            left.Add(int.Parse(nums[0]));
            var rnum = int.Parse(nums[1]);
            if (right.ContainsKey(rnum))
            {
                right[rnum] += 1;
            }
            else
            {
                right.Add(rnum, 1);
            }
        }

        var sum = 0;
        for (int i = 0; i < left.Count(); i++)
        {
            var lnum = left[i];
            if (right.ContainsKey(lnum))
            {
                sum += (lnum * right[lnum]);
            }
        }

        return sum;
    }
}
