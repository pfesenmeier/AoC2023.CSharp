using System;
using System.Linq;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.Execution;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;

class Build : NukeBuild
{
    public static int Main() => Execute<Build>();

    readonly string DaysDir = "Days";

    [PathVariable]
    readonly Tool DotNet;

    [Parameter("Day. Defaults to Next Uncompleted Day")]
    int? Day;

    [Parameter("Year")]
    readonly int Year = 2023;

    Target SetDay => _ => _
        .DependsOn(EnsureDaysExists)
        .OnlyWhenDynamic(() => Day is null)
        .Executes(() =>
    {
        var currentYear = RootDirectory / Year.ToString();

        var fileNames = currentYear.GetFiles().Select(file => file.Name).Select(name => int.Parse(name));

        Day = fileNames.Count() is 0 ? 0 : fileNames.Max() + 1;
    });

    Target EnsureDaysExists => _ => _
        .Executes(() => (RootDirectory / DaysDir).CreateDirectory());

    Target StartDay => _ => _
        .DependsOn(SetDay)
        .Executes(() =>
        {
            var projectDirectory = RootDirectory / DaysDir / Year.ToString() / Day.ToString();
            projectDirectory.CreateDirectory();

            var srcDir = projectDirectory / "src";
            var testDir = projectDirectory / "test";
            DotNet($"new console -o {srcDir}");
            DotNet($"new xunit -o {testDir}");
            DotNet($"add {testDir} reference {srcDir}");
        });
}
