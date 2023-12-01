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

    [Parameter("Year")]
    readonly int Year = 2023;

    string ProgramTemplate() => $$"""
using Solution = System.Func<string, string>;

string projectDir = Environment.SpecialFolder.UserProfile + @"\{{RootDirectory.GetRelativePathTo(DayPath())}}";
string sampleInput = projectDir + @"\sample.txt"; 
string puzzleInput = projectDir + @"\input.txt";

Solution Part1 = (string input) => 
{
    return "TODO";
};

Solution Part2 = (string input) => 
{
    return "TODO";
};

Console.WriteLine($"Part 1 Sample");
Console.WriteLine(Part1(sampleInput));
Console.WriteLine($"Part 1 Solution");
Console.WriteLine(Part1(puzzleInput));

Console.WriteLine($"Part 2 Sample");
Console.WriteLine(Part2(sampleInput));
Console.WriteLine($"Part 2 Solution");
Console.WriteLine(Part2(puzzleInput));
""";

    AbsolutePath YearPath() => AdventCalendars / Year.ToString();
    AbsolutePath DayPath() => YearPath() / Day.ToString();

    Target SetDay => _ => _
        .DependsOn(EnsureYearExists)
        .OnlyWhenDynamic(() => Day is null)
        .Executes(() =>
    {
        var fileNames = YearPath().GetDirectories().Select(file => file.Name).Select(name => int.Parse(name));

        Day = fileNames.Count() is 0 ? 1 : fileNames.Max() + 1;
    });

    Target EnsureYearExists => _ => _
        .Executes(() => (AdventCalendars / Year.ToString()).CreateDirectory());

    Target EnsureDayExists => _ => _
        .DependsOn(SetDay)
        .Executes(() => DayPath().CreateDirectory());

    Target StartDay => _ => _
        .DependsOn(EnsureDayExists)
        .Executes(() =>
        {
            Log.Debug($"Templating Day {Day}");

            var srcDir = DayPath() / $"Day{Day}";
            var testDir = DayPath() / $"Day{Day}.Test";
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
           var programPath = DayPath() / "src" / "Program.cs";
           programPath.WriteAllText(ProgramTemplate());

           (DayPath() / "input.txt").TouchFile();
           (DayPath() / "sample.txt").TouchFile();
       });
}
