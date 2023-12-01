using System;
using System.Linq;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.Execution;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Utilities.Collections;
using Serilog;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;

class Build : NukeBuild
{
    public static int Main() => Execute<Build>();

    readonly AbsolutePath AdventCalendars = RootDirectory / "AdventCalendars";

    [PathVariable]
    readonly Tool DotNet;

    [Parameter("Day. Defaults to Next Uncompleted Day")]
    int? Day;

    string ProjectName => $"AoC{Year}." + Day?.ToString("D2");
    AbsolutePath ProjectPath => AdventCalendars / ProjectName;

    [Parameter("Year")]
    readonly int Year = 2023;

    string ProgramTemplate() => $$"""
using Solution = System.Func<string, string>;

string projectDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\Code\Aoc2023.CSharp\{{RootDirectory.GetRelativePathTo(ProjectPath)}}";
string sampleInput = projectDir + @"\sample.txt"; 
string puzzleInput = projectDir + @"\input.txt";

Solution Part1 = (string input) => 
{
    var lines = File.ReadAllLines(input);
    return "TODO";
};

Solution Part2 = (string input) => 
{
    var lines = File.ReadAllLines(input);
    return "TODO";
};

Console.WriteLine("Part 1 Sample");
Console.WriteLine(Part1(sampleInput));
Console.WriteLine("Part 1 Solution");
Console.WriteLine(Part1(puzzleInput));

Console.WriteLine("Part 2 Sample");
Console.WriteLine(Part2(sampleInput));
Console.WriteLine("Part 2 Solution");
Console.WriteLine(Part2(puzzleInput));
""";

    Target EnsureAdventCalendarsExist => _ => _
        .Executes(() => AdventCalendars.CreateDirectory());

    Target SetDay => _ => _
        .DependsOn(EnsureAdventCalendarsExist)
        .OnlyWhenDynamic(() => Day is null)
        .Executes(() =>
    {
        Day = AdventCalendars.GetDirectories().Count() is 0
            ? 1
            : AdventCalendars.GetDirectories().Select(file => file.Name).Max(name =>
            {
                if (int.TryParse(string.Join("", name.TakeLast(2)), out var result))
                {
                    return result;
                }
                // skip test projects
                return -1;
            }) + 1;

        Log.Information($"Setting up Day {Day}");
    });

    Target StartDay => _ => _
        .DependsOn(SetDay)
        .Executes(() =>
        {
            var srcDir = ProjectPath;
            var testDir = AdventCalendars / $"{ProjectName}.Test";
            DotNet($"new console -o {srcDir}");
            DotNet($"new xunit -o {testDir}");
            DotNet($"add {testDir} reference {srcDir}");
            DotNet($"sln add {srcDir}");
            DotNet($"sln add {testDir}");
        });

    Target TemplateProgram => _ => _
       .TriggeredBy(StartDay)
       .Executes(() =>
       {
           (ProjectPath / "Program.cs").WriteAllText(ProgramTemplate());
           (ProjectPath / "sample.txt").TouchFile();
           (ProjectPath / "input.txt").TouchFile();
       });
}
