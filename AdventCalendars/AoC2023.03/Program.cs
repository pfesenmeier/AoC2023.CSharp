using Dumpify;
using Aoc2023._03;
using Solution = System.Func<string, string>;

string projectDir =
    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)
    + @"\Code\Aoc2023.CSharp\AdventCalendars\AoC2023.03";
string sampleInput = projectDir + @"\sample.txt";
string puzzleInput = projectDir + @"\input.txt";

Solution Part1 = (string input) =>
{
    var lines = File.ReadAllLines(input);

    var schematic = new Schematic(lines);
    schematic.ParseMap();
    schematic.SetNumbers();
    return schematic.Numbers.Sum(num => num.Value).ToString();
};

Solution Part2 = (string input) =>
{
    var lines = File.ReadAllLines(input).Dump();

    var schematic = new Schematic(lines);
    schematic.ParseMap();
    schematic.SetNumbers();
    schematic.SetGears();
    return schematic.AddUpGears().ToString();
};

Func<string> main = args switch
{
["-1s"] => () => Part1(sampleInput),
["-1i"] => () => Part1(puzzleInput),
["-2s"] => () => Part2(sampleInput),
["-2i"] => () => Part2(puzzleInput),
    _ => () => Part1(sampleInput),
};

Console.WriteLine(main());
