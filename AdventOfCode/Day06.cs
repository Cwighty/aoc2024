using System.Text.RegularExpressions;
namespace AdventOfCode;

public class Day06 : BaseDay
{
    private readonly string[] _input;

    public Day06()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override ValueTask<string> Solve_1() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 1 {Solve1()}");

    public override ValueTask<string> Solve_2() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 2 {Solve2()}");

    int Solve1()
    {
        var map = new Map(_input.ToList());

        bool guardBlocked = true;
        while (guardBlocked)
        {
            var currPos = map.GuardPosition;
            var currDir = map.GuardDirection;
            Position nextObstacle = map.GuardDirection switch
            {
                Direction.Up => map.Obstructions.OrderByDescending(o => o.Row).FirstOrDefault(o => o.Column == currPos.Column && o.Row < currPos.Row),
                Direction.Down => map.Obstructions.OrderBy(o => o.Row).FirstOrDefault(o => o.Column == currPos.Column && o.Row > currPos.Row),
                Direction.Left => map.Obstructions.OrderByDescending(o => o.Column).FirstOrDefault(o => o.Row == currPos.Row && o.Column < currPos.Column),
                Direction.Right => map.Obstructions.OrderBy(o => o.Column).FirstOrDefault(o => o.Row == currPos.Row && o.Column > currPos.Column),
                _ => throw new InvalidOperationException("Invalid direction"),
            };

            Position nextPosition;
            if (nextObstacle == null)
            {
                guardBlocked = false;
                nextPosition = map.GuardDirection switch
                {
                    Direction.Up => new(0, currPos.Column),
                    Direction.Down => new(map.Height - 1, currPos.Column),
                    Direction.Right => new(currPos.Row, map.Width - 1),
                    Direction.Left => new(currPos.Row, 0),
                    _ => throw new InvalidOperationException("Invalid direction"),
                };
            }
            else
            {
                nextPosition = map.GuardDirection switch
                {
                    Direction.Up => new(nextObstacle.Row + 1, currPos.Column),
                    Direction.Down => new(nextObstacle.Row - 1, currPos.Column),
                    Direction.Right => new(currPos.Row, nextObstacle.Column - 1),
                    Direction.Left => new(currPos.Row, nextObstacle.Column + 1),
                    _ => throw new InvalidOperationException("Invalid direction"),
                };
            }


            map.MoveTo(nextPosition);
            map.Turn();
        }

        // save map to text file
        var visited = map.Visited.ToList();
        var mapArray = new char[map.Height, map.Width];
        for (int r = 0; r < map.Height; r++)
        {
            for (int c = 0; c < map.Width; c++)
            {
                mapArray[r, c] = map.Obstructions.Contains(new(r, c)) ? '#' : '.';
                mapArray[r, c] = visited.Contains(new(r, c)) ? 'X' : mapArray[r, c];
            }
        }

        using var sw = new StreamWriter("map.txt");
        for (int r = 0; r < map.Height; r++)
        {
            for (int c = 0; c < map.Width; c++)
            {
                sw.Write(mapArray[r, c]);
            }
            sw.WriteLine();
        }

        return map.Visited.Count();
    }

    public int Solve2()
    {
        var map = new Map(_input.ToList());
        var visitedObstacles = new List<(Position, Direction)>();

        bool guardBlocked = true;
        while (guardBlocked)
        {
            var currPos = map.GuardPosition;
            var currDir = map.GuardDirection;
            Position nextObstacle = map.GuardDirection switch
            {
                Direction.Up => map.Obstructions.OrderByDescending(o => o.Row).FirstOrDefault(o => o.Column == currPos.Column && o.Row < currPos.Row),
                Direction.Down => map.Obstructions.OrderBy(o => o.Row).FirstOrDefault(o => o.Column == currPos.Column && o.Row > currPos.Row),
                Direction.Left => map.Obstructions.OrderByDescending(o => o.Column).FirstOrDefault(o => o.Row == currPos.Row && o.Column < currPos.Column),
                Direction.Right => map.Obstructions.OrderBy(o => o.Column).FirstOrDefault(o => o.Row == currPos.Row && o.Column > currPos.Column),
                _ => throw new InvalidOperationException("Invalid direction"),
            };

            Position nextPosition;
            if (nextObstacle == null)
            {
                guardBlocked = false;
                nextPosition = map.GuardDirection switch
                {
                    Direction.Up => new(0, currPos.Column),
                    Direction.Down => new(map.Height - 1, currPos.Column),
                    Direction.Right => new(currPos.Row, map.Width - 1),
                    Direction.Left => new(currPos.Row, 0),
                    _ => throw new InvalidOperationException("Invalid direction"),
                };
            }
            else
            {
                visitedObstacles.Add((nextObstacle, map.GuardDirection));
                nextPosition = map.GuardDirection switch
                {
                    Direction.Up => new(nextObstacle.Row + 1, currPos.Column),
                    Direction.Down => new(nextObstacle.Row - 1, currPos.Column),
                    Direction.Right => new(currPos.Row, nextObstacle.Column - 1),
                    Direction.Left => new(currPos.Row, nextObstacle.Column + 1),
                    _ => throw new InvalidOperationException("Invalid direction"),
                };
            }


            map.MoveTo(nextPosition);
            map.Turn();
        }

        var potentialLoopPoints = new List<Position>();

        for (int i = 2; i < visitedObstacles.Count(); i++)
        {
            var c1 = visitedObstacles[i - 2];
            var c2 = visitedObstacles[i - 1];
            var c3 = visitedObstacles[i];
            var corners = new List<(Position, Direction)>()
            {
              c1,c2,c3
            };

            // all corners unique directions
            var directions = corners.Select(c => c.Item2).Distinct();
            if (directions.Count() < 3)
                continue;

            var p1 = ObstacleToGuardPos(c1.Item1, c1.Item2);
            var p2 = ObstacleToGuardPos(c2.Item1, c2.Item2);
            var p3 = ObstacleToGuardPos(c3.Item1, c3.Item2);

            // missing direction
            List<int> dirs = new() { 0, 1, 2, 3 };
            var missingDir = dirs.Except(directions.Select(d => (int)d));

            // missing location should be the points that don't appear twice
            // need to addjust to be guard positions, since the obstacles are outlineing the rectangle
            var rows = new[] { p1.Row, p2.Row, p3.Row };
            var cols = new[] { p1.Column, p2.Column, p3.Column };
            int missingRow = rows.GroupBy(r => r).Where(g => g.Count() == 1).Single().Key;
            int missingCol = cols.GroupBy(c => c).Where(g => g.Count() == 1).Single().Key;

            var p4 = new Position(missingRow, missingCol);
            var c4 = GuardPosToObstacle(p4, (Direction)missingDir.First());

            // test that theres not obstacles from p3 to p4 and from p4 to p1
            var obstaclesSet = new HashSet<Position>(map.Obstructions);
            var p3ToP4Set = new HashSet<Position>(PointsBetween(p3, p4));
            if (p3ToP4Set.Intersect(obstaclesSet).Count() > 0)
                continue;
            var p4ToP1Set = new HashSet<Position>(PointsBetween(p4, p1));
            if (p4ToP1Set.Intersect(obstaclesSet).Count() > 0)
                continue;
            potentialLoopPoints.Add(c4);
        }

        var visited = map.Visited.ToList();
        var mapArray = new char[map.Height, map.Width];
        for (int r = 0; r < map.Height; r++)
        {
            for (int c = 0; c < map.Width; c++)
            {
                mapArray[r, c] = map.Obstructions.Contains(new(r, c)) ? '#' : '.';
                mapArray[r, c] = potentialLoopPoints.Contains(new(r, c)) ? 'O' : mapArray[r, c];
            }
        }

        using var sw = new StreamWriter("map2.txt");
        for (int r = 0; r < map.Height; r++)
        {
            for (int c = 0; c < map.Width; c++)
            {
                sw.Write(mapArray[r, c]);
            }
            sw.WriteLine();
        }

        return potentialLoopPoints.Count();
    }

    public List<Position> PointsBetween(Position curr, Position next)
    {
        var points = new List<Position>();

        if (curr.Row == next.Row) // Same row
        {
            int start = Math.Min(curr.Column, next.Column);
            int end = Math.Max(curr.Column, next.Column);

            for (int col = start + 1; col < end; col++)
            {
                points.Add(new Position(curr.Row, col));
            }
        }
        else if (curr.Column == next.Column) // Same column
        {
            int start = Math.Min(curr.Row, next.Row);
            int end = Math.Max(curr.Row, next.Row);

            for (int row = start + 1; row < end; row++)
            {
                points.Add(new Position(row, curr.Column));
            }
        }

        return points;
    }

    private Position ObstacleToGuardPos(Position obs, Direction dir)
    {
        Position newPos = dir switch
        {
            Direction.Up => new(obs.Row + 1, obs.Column),
            Direction.Down => new(obs.Row - 1, obs.Column),
            Direction.Right => new(obs.Row, obs.Column - 1),
            Direction.Left => new(obs.Row, obs.Column + 1),
            _ => throw new InvalidOperationException("Invalid direction"),

        };

        return newPos;
    }

    private Position GuardPosToObstacle(Position guard, Direction dir)
    {
        Position newPos = dir switch
        {
            Direction.Up => new(guard.Row - 1, guard.Column),
            Direction.Down => new(guard.Row + 1, guard.Column),
            Direction.Right => new(guard.Row, guard.Column + 1),
            Direction.Left => new(guard.Row, guard.Column - 1),
            _ => throw new InvalidOperationException("Invalid direction"),

        };

        return newPos;
    }
}

public record Position(int Row, int Column);

public enum Direction
{
    Up,
    Right,
    Down,
    Left
}

public class Map
{
    public int Width { get; init; }
    public int Height { get; init; }
    public List<Position> Obstructions { get; init; }
    public HashSet<Position> Visited { get; init; } = new();
    public Position GuardPosition { get; private set; }
    public Direction GuardDirection { get; private set; }

    public Map(List<string> input)
    {
        List<Position> obstructions = new();
        Position guardPosition = new(0, 0);
        Direction guardDirection = Direction.Up;

        for (int r = 0; r < input.Count(); r++)
        {
            for (int c = 0; c < input[r].Count(); c++)
            {
                var cell = input[r][c];
                if (cell == '#')
                    obstructions.Add(new(r, c));
                if (cell == '^')
                {
                    guardPosition = new(r, c);
                    guardDirection = Direction.Up;
                }
                if (cell == '>')
                {
                    guardPosition = new(r, c);
                    guardDirection = Direction.Right;
                }
                if (cell == 'V')
                {
                    guardPosition = new(r, c);
                    guardDirection = Direction.Down;
                }
                if (cell == '<')
                {
                    guardPosition = new(r, c);
                    guardDirection = Direction.Left;
                }
            }
        }

        Obstructions = obstructions;
        GuardDirection = guardDirection;
        GuardPosition = guardPosition;
        Visited.Add(guardPosition);
        Height = input.Count();
        Width = input[0].Count();
    }

    public void Turn()
    {
        if (GuardDirection == Direction.Up)
        {
            GuardDirection = Direction.Right;
            return;
        }
        if (GuardDirection == Direction.Right)
        {
            GuardDirection = Direction.Down;
            return;
        }
        if (GuardDirection == Direction.Down)
        {
            GuardDirection = Direction.Left;
            return;
        }
        if (GuardDirection == Direction.Left)
        {
            GuardDirection = Direction.Up;
            return;
        }
    }

    public void MoveTo(Position pos)
    {
        for (int r = GuardPosition.Row; r <= pos.Row; r++)
        {
            Visited.Add(new(r, GuardPosition.Column));
        }
        for (int r = pos.Row; r <= GuardPosition.Row; r++)
        {
            Visited.Add(new(r, GuardPosition.Column));
        }
        for (int c = GuardPosition.Column; c <= pos.Column; c++)
        {
            Visited.Add(new(GuardPosition.Row, c));
        }
        for (int c = pos.Column; c <= GuardPosition.Column; c++)
        {
            Visited.Add(new(GuardPosition.Row, c));
        }
        GuardPosition = pos;
    }
}

