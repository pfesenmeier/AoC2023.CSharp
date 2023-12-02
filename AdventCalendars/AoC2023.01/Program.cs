using Num = (int value, string text);
using Solution = System.Func<string, string>;
string projectDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\Code\Aoc2023.CSharp\AdventCalendars\AoC2023.01";
string sampleInput = projectDir + @"\sample.txt";
string puzzleInput = projectDir + @"\input.txt";

Num[] numbers = [(1, "one"), (2, "two"), (3, "three"), (4, "four"), (5, "five"), (6, "six"), (7, "seven"), (8, "eight"), (9, "nine")];

Solution Part2 = (string input) =>
    File.ReadAllLines(input)
    .Select(line =>
    {
        var firstDigitIndex = line.ToList().FindIndex(c => int.TryParse(c.ToString(), out int _));
        var lastDigitIndex = line.ToList().FindLastIndex(c => int.TryParse(c.ToString(), out int _));

        var wordIndices = numbers
          .Select(number => (line.IndexOf(number.text), number))
          .Where(result => result.Item1 != -1)
          .ToList();;

        var lastWordIndices = numbers
          .Select(number => (line.LastIndexOf(number.text), number))
          .Where(result => result.Item1 != -1)
          .ToList();;

        wordIndices.AddRange(lastWordIndices);


        if (firstDigitIndex is not -1)
        {
            wordIndices.Add((firstDigitIndex, (int.Parse(line[firstDigitIndex].ToString()), "")));
        }

        if (lastDigitIndex is not -1)
        {
            wordIndices.Add((lastDigitIndex, (int.Parse(line[lastDigitIndex].ToString()), "")));
        }

        wordIndices = [.. wordIndices.OrderBy(w => w.Item1)];

        var combo = "" + wordIndices.First().number.value + wordIndices.Last().number.value;

        var result = int.Parse(combo);

        return int.Parse(combo);
    }).Sum().ToString();

Solution Part1 = (string input) =>
    File.ReadAllLines(input)
    .Select(line =>
    {
        var first = line.ToList().FindIndex(c => int.TryParse(c.ToString(), out int _));
        var last = line.ToList().FindLastIndex(c => int.TryParse(c.ToString(), out int _));

        var combo = "" + line[first] + line[last];

        return int.Parse("" + line[first] + line[last]);

    }).Sum().ToString();

// Console.WriteLine("Part 1 Sample");
// Console.WriteLine(Part1(sampleInput));
// Console.WriteLine("Part 1 Solution");
// Console.WriteLine(Part1(puzzleInput));

// Console.WriteLine("Part 2 Sample");
// Console.WriteLine(Part2(sampleInput));
Console.WriteLine("Part 2 Solution");
Console.WriteLine(Part2(puzzleInput));
