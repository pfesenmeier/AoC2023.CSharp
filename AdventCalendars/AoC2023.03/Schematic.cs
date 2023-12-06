namespace Aoc2023._03;

public record Digit(string Value, bool IsTouchingSymbol, bool IsLastDigit, (int x, int y)?
        Gear = null);

public class Schematic
{
    private readonly int NumRows;
    private readonly int NumCols;

    public string[,] Map { get; }
    public List<Digit> ParsedMap { get; set; } = [];
    public List<(int Value, List<(int x, int y)?>? Gears)> Numbers { get; set; } = [];
    public Dictionary<(int x, int y), List<int>> Gears { get; set; } = [];

    public Schematic(IEnumerable<string> input)
    {
        NumRows = input.Count();
        NumCols = input.First().Length;
        Map = new string[NumRows, NumCols];

        for (var i = 0; i < NumRows; i++)
        {
            for (var j = 0; j < NumCols; j++)
            {
                Map[i, j] = input.ElementAt(i)[j].ToString();
            }
        }
    }

    public void ParseMap()
    {
        for (var i = 0; i < NumRows; i++)
        {
            for (var j = 0; j < NumCols; j++)
            {
                var value = GetValue(i, j);
                if (value is not null && int.TryParse(value, out _))
                {
                    ParsedMap.Add(new Digit(
                                value,
                                IsTouchingSymbol(i, j, out (int x, int y)? gear),
                                IsLastDigit(i, j),
                                gear
                                ));
                }
            }
        }
    }

    public void SetGears()
    {
        foreach (var Number in Numbers)
        {
            foreach (var gear in Number.Gears ?? [])
            {
                if (gear is (int x, int y) g)
                {
                    if (!Gears.TryGetValue(g, out var value))
                    {
                        Gears.Add(g, [Number.Value]);
                    }
                    else if (!value.Contains(Number.Value))
                    {
                        value.Add(Number.Value);
                    }
                }
            }
        }
    }

    public int AddUpGears() =>
        Gears.Select(entry =>
                entry.Value.Count == 2 ?
                  entry.Value.Aggregate((a, b) => a * b)
                  : 0
                ).Sum();

    public void SetNumbers()
    {
        List<Digit> digitBuffer = [];
        foreach (var digit in ParsedMap)
        {
            digitBuffer.Add(digit);

            if (digit.IsLastDigit)
            {
                if (digitBuffer.Any(d => d.IsTouchingSymbol))
                {
                    var gears = digitBuffer.Select(d =>
                            d.Gear).Where(g => g != null).ToList();
                    var value = string.Concat(digitBuffer.Select(d => d.Value));
                    Numbers.Add((int.Parse(value), gears));
                }
                digitBuffer.Clear();
            }
        }
    }

    private bool IsTouchingSymbol(int x, int y, out (int x, int y)? gear)
    {
        gear = null;
        foreach (var neighbor in GetNeighborsMatrix(x, y))
        {
            var value = GetValue(neighbor.x, neighbor.y);
            if (value is not null and not ".")
            {
                var isNum = int.TryParse(value, out _);
                if (!isNum)
                {
                    if (value is "*")
                    {
                        gear = (neighbor.x, neighbor.y);
                    }

                    return true;
                }
            }
        }

        return false;
    }

    private bool IsLastDigit(int x, int y)
    {
        var value = GetValue(x, y + 1);

        return value is null || !int.TryParse(value, out _);
    }

    private static (int x, int y)[,] GetNeighborsMatrix(int x, int y) =>
         new (int x, int y)[3, 3] {
            { (x - 1, y + 1), (x, y + 1), (x + 1, y + 1) },
            { (x - 1, y), (x, y), (x + 1, y) },
            { (x - 1, y - 1), (x, y - 1), (x + 1, y - 1) }
        };

    private string? GetValue(int x, int y)
        => y >= NumRows || x >= NumCols || x < 0 || y < 0 ? null : Map[x, y];
}
