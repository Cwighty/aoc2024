using System.Text.RegularExpressions;
namespace AdventOfCode;

public class Day05 : BaseDay
{
    private readonly string[] _input;

    public Day05()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override ValueTask<string> Solve_1() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 1 {Solve1()}");

    public override ValueTask<string> Solve_2() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 2 {Solve2()}");

    int Solve1()
    {
        var rules = new List<string>();
        var updates = new List<List<int>>();
        foreach (var l in _input)
        {
            if (l.Contains('|'))
                rules.Add(l);
            if (l.Contains(','))
            {
                var updateNums = l.Split(',').Select(int.Parse).ToList();
                updates.Add(updateNums);
            }
        }

        var ruleGraph = buildGraph(rules);

        var validUpdates = new List<List<int>>();

        foreach (var update in updates)
        {
            var valid = true;
            for (int i = 0; i < update.Count() - 1; i++)
            {
                var start = update[i];
                var end = update[i + 1];
                if (!ruleGraph.EdgeExistsBetween(start, end))
                {
                    valid = false;
                    break;
                }
            }
            if (valid)
                validUpdates.Add(update);
        }

        var sumOfMiddle = 0;
        foreach (var update in validUpdates)
        {
            int middleIndex = update.Count() / 2;
            sumOfMiddle += update[middleIndex];
        }

        return sumOfMiddle;
    }

    private Graph buildGraph(List<string> graphLines)
    {
        var g = new Graph();
        foreach (var line in graphLines)
        {
            var nums = line.Split('|').Select(i => int.Parse(i)).ToList();
            g.AddEdge(nums[0], nums[1]);
        }

        return g;
    }

    private Graph buildRelatedGraph(List<string> graphLines, List<int> update)
    {
        var g = new Graph();
        foreach (var line in graphLines)
        {
            var nums = line.Split('|').Select(i => int.Parse(i)).ToList();
            if (update.Contains(nums[0]) && update.Contains(nums[1]))
                g.AddEdge(nums[0], nums[1]);
        }

        return g;
    }


    public int Solve2()
    {
        var rules = new List<string>();
        var updates = new List<List<int>>();
        foreach (var l in _input)
        {
            if (l.Contains('|'))
                rules.Add(l);
            if (l.Contains(','))
            {
                var updateNums = l.Split(',').Select(int.Parse).ToList();
                updates.Add(updateNums);
            }
        }

        var ruleGraph = buildGraph(rules);

        var validUpdates = new List<List<int>>();
        var invalidUpdates = new List<List<int>>();

        foreach (var update in updates)
        {
            var valid = true;
            for (int i = 0; i < update.Count() - 1; i++)
            {
                var start = update[i];
                var end = update[i + 1];
                if (!ruleGraph.EdgeExistsBetween(start, end))
                {
                    valid = false;
                    invalidUpdates.Add(update);
                    break;
                }
            }
            if (valid)
                validUpdates.Add(update);
        }

        var sortedInvalids = new List<List<int>>();

        foreach (var update in invalidUpdates)
        {
            var relatedGraph = buildRelatedGraph(rules, update);
            var sorted = relatedGraph.TopoSort(update);
            sortedInvalids.Add(sorted);
        }

        var sumOfMiddle = 0;
        foreach (var update in sortedInvalids)
        {
            int middleIndex = update.Count() / 2;
            sumOfMiddle += update[middleIndex];
        }

        return sumOfMiddle;
    }
}

public class Graph
{
    public Dictionary<int, List<int>> AdjacencyList = new();

    public void AddEdge(int n1, int n2)
    {
        if (!AdjacencyList.ContainsKey(n1))
        {
            AdjacencyList.Add(n1, new List<int>());
        }

        AdjacencyList[n1].Add(n2);
    }

    public bool EdgeExistsBetween(int n1, int n2)
    {
        if (AdjacencyList.ContainsKey(n1) && AdjacencyList[n1].Contains(n2))
            return true;

        return false;
    }

    public List<int> TopoSort(List<int> update)
    {
        var verts = update.Count();
        var sorted = new List<int>();
        var incomingEdgeCount = new Dictionary<int, int>();

        foreach (var key in update)
            incomingEdgeCount.Add(key, 0);

        foreach (var node in AdjacencyList)
        {
            foreach (var neighbor in node.Value)
            {
                incomingEdgeCount[neighbor]++;
            }
        }

        var queue = new Queue<int>();
        foreach (var node in incomingEdgeCount.Where(kvp => kvp.Value == 0))
            queue.Enqueue(node.Key);

        while (queue.Count() > 0)
        {
            var curr = queue.Dequeue();
            sorted.Add(curr);

            if (AdjacencyList.TryGetValue(curr, out var currNode))
            {
                foreach (var neighbor in currNode)
                {
                    incomingEdgeCount[neighbor]--;
                    if (incomingEdgeCount[neighbor] == 0)
                        queue.Enqueue(neighbor);
                }
            }
        }

        return sorted;
    }
}
