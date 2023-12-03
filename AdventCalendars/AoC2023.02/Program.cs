using Dumpify;
using Solution = System.Func<string, string>;

string projectDir = Environment
  .GetFolderPath(Environment.SpecialFolder.UserProfile)
  + @"\Code\Aoc2023.CSharp\AdventCalendars\AoC2023.02";
string sampleInput = projectDir + @"\sample.txt";
string puzzleInput = projectDir + @"\input.txt";

Solution Part1 = (string input) => File.ReadAllLines(input)
        .Select((line, i) => new
        {
            GameNo = i + 1,
            Hands = line
                     .Split(':', ';')
                     .Skip(1)
                     .Select(hand => hand.Trim()
                                         .Split(", ")
                                         .Select(amtAndColor => new
                                         {
                                             Amount = int.Parse(amtAndColor[0..amtAndColor.IndexOf(' ')]),
                                             Color = amtAndColor[(amtAndColor.IndexOf(' ') + 1)..]
                                         })
                             )
        }).Where(game => game.Hands.All(cubes => cubes.All(cube => cube switch
        {
            { Color: "red", Amount: <= 12 } => true,
            { Color: "green", Amount: <= 13 } => true,
            { Color: "blue", Amount: <= 14 } => true,
            _ => false,
        })))
        .Sum(game => game.GameNo)
        .ToString();

Solution Part2 = (string input) => File.ReadAllLines(input)
            .Select((line, i) => new
            {
                GameNo = i + 1,
                Hands = line
                             .Split(':', ';')
                             .Skip(1)
                             .Select(hand => hand.Trim()
                                                 .Split(", ")
                                                 .Select(amtAndColor => new
                                                 {
                                                     Amount = int.Parse(amtAndColor[0..amtAndColor.IndexOf(' ')]),
                                                     Color = amtAndColor[(amtAndColor.IndexOf(' ') + 1)..],
                                                 })
                                                 )
            }).Select(game =>
            {
                int minColor(string color) => game.Hands.Select(hand => hand.Where(cube => cube.Color == color).Select(cube => cube.Amount)).SelectMany(game => game).Max();

                return new
                {
                    game.GameNo,
                    MinRed = minColor("red"),
                    MinGreen = minColor("green"),
                    MinBlue = minColor("blue"),
                };
            })
            .Select(game => game.MinRed * game.MinGreen * game.MinBlue)
            .Dump()
            .Sum()
            .ToString();

Func<string> main = args switch
{
["-1s"] => () => Part1(sampleInput),
["-1i"] => () => Part1(puzzleInput),
["-2s"] => () => Part2(sampleInput),
["-2i"] => () => Part2(puzzleInput),
    _ => () => Part1(sampleInput)
};

Console.WriteLine(main());
