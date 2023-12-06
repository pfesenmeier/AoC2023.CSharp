using Dumpify;
using Solution = System.Func<string, string>;

string projectDir =
    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)
    + @"\Code\Aoc2023.CSharp\AdventCalendars\AoC2023.04";
string sampleInput = projectDir + @"\sample.txt";
string puzzleInput = projectDir + @"\input.txt";

Solution Part1 = (string input) =>
        File.ReadAllLines(input)
        .Select(line => new
        {
            WinningNumbers = line[(line.IndexOf(':') + 1)..(line.IndexOf('|') - 1)]
                                    .Split(Array.Empty<char>(),
                                            StringSplitOptions.TrimEntries
                                            | StringSplitOptions.RemoveEmptyEntries)
                                   .Select(int.Parse),
            MyNumbers = line[(line.IndexOf('|') + 2)..]
                                    .Split(Array.Empty<char>(),
                                            StringSplitOptions.TrimEntries
                                            | StringSplitOptions.RemoveEmptyEntries)
                                    .Select(int.Parse)
        })
        .Select(card => card.MyNumbers.Select(
                    num => card.WinningNumbers.Contains(num) ? 1 : 0).Sum())
        .Select(numWinners => numWinners is 0 ? 0 : Math.Pow(2, numWinners - 1))
        .Sum()
        .Dump()
        .ToString();

Solution Part2 = (string input) =>
        File.ReadAllLines(input)
        .Select(line => new
        {
            WinningNumbers = line[(line.IndexOf(':') + 1)..(line.IndexOf('|') - 1)]
                                    .Split(Array.Empty<char>(),
                                            StringSplitOptions.TrimEntries
                                            | StringSplitOptions.RemoveEmptyEntries)
                                   .Select(int.Parse),
            MyNumbers = line[(line.IndexOf('|') + 2)..]
                                    .Split(Array.Empty<char>(),
                                            StringSplitOptions.TrimEntries
                                            | StringSplitOptions.RemoveEmptyEntries)
                                    .Select(int.Parse)
        })
        .Select((card, i) => new
        {
            numWinners = card.MyNumbers.Select(
                    num => card.WinningNumbers.Contains(num) ? 1 : 0).Sum(),
            cardNumber = i + 1,
        })
        .Aggregate(new Dictionary<int, int>(), (totals, card) =>
                {
                    // add original
                    if (!totals.TryAdd(card.cardNumber, 1))
                    {
                        totals[card.cardNumber]++;
                    }

                    // add copies
                    var numCopies = totals[card.cardNumber];
                    foreach (var num in Enumerable.Range(card.cardNumber + 1,
                                card.numWinners))
                    {
                        if (!totals.TryAdd(num, numCopies))
                        {
                            totals[num] += numCopies;
                        }
                    }

                    return totals;
                })
        .Sum(entry => entry.Value)
        .Dump()
        .ToString();

Func<string> main = args switch
{
["-1s"] => () => Part1(sampleInput),
["-1i"] => () => Part1(puzzleInput),
["-2s"] => () => Part2(sampleInput),
["-2i"] => () => Part2(puzzleInput),
    _ => () => "Usage: dotnet run -[1,2][i,s]. s for sample, i for input"
};

Console.WriteLine(main());
