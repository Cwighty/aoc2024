
namespace AdventOfCode;

public class Day02 : BaseDay
{
    private readonly string[] _input;

    public Day02()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override ValueTask<string> Solve_1() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 1 {Solve1()}");

    public override ValueTask<string> Solve_2() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 2 {Solve2()}");

    int Solve1()
    {
        int safeCount = 0;
        foreach (var report in _input)
        {
            var levels = report.Split(" ").Select(i => int.Parse(i)).ToList();

            if (testLevels(levels))
                safeCount++;
        }
        return safeCount;
    }

    private bool testLevels(List<int> levels)
    {
        int? prev = null;
        bool safe = true;
        for (int i = 1; i < levels.Count; i++)
        {
            var diff = levels[i - 1] - levels[i];
            var absDiff = Math.Abs(diff);
            if (absDiff > 3 || absDiff < 1)
                safe = false;
            if (prev.HasValue && ((diff > 0 && prev < 0) || (diff < 0 && prev > 0)))
                safe = false;

            if (!safe)
                break;

            prev = diff;
        }

        return safe;
    }

    public int Solve2()
    {
        int safeCount = 0;
        foreach (var report in _input)
        {
            var levels = report.Split(" ").Select(i => int.Parse(i)).ToList();

            int? prev = null;
            bool safe = true;
            int levelsCount = levels.Count();
            for (int i = 1; i < levelsCount; i++)
            {
                var diff = levels[i - 1] - levels[i];
                var absDiff = Math.Abs(diff);
                if ((absDiff > 3 || absDiff < 1) || (prev.HasValue && ((diff > 0 && prev < 0) || (diff < 0 && prev > 0))))
                {
                    safe = false;
                    if (i - 2 >= 0)
                    {
                        var prevItems = levels.ToList();
                        prevItems.RemoveAt(i - 2);
                        safe = updateAltTimeline(safe, testLevels(prevItems));
                    }
                    if (i - 1 >= 0)
                    {
                        var prevItems = levels.ToList();
                        prevItems.RemoveAt(i - 1);
                        safe = updateAltTimeline(safe, testLevels(prevItems));
                    }
                    var currItems = levels.ToList();
                    currItems.RemoveAt(i);
                    safe = updateAltTimeline(safe, testLevels(currItems));
                    if (i + 1 < levelsCount)
                    {
                        var nextItems = levels.ToList();
                        nextItems.RemoveAt(i + 1);
                        safe = updateAltTimeline(safe, testLevels(nextItems));
                    }
                }

                if (!safe)
                    break;

                prev = diff;
            }

            if (safe)
                safeCount++;
        }
        return safeCount;
    }

    public bool updateAltTimeline(bool currTimeline, bool safe)
    {
        if (!currTimeline)
            currTimeline = safe;

        return currTimeline;
    }
}
