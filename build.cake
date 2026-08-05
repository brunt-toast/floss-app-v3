var target = Argument("target", "FastRunGui");
var configuration = Argument("configuration", "Release");

Task("InstallSdk").Does(() =>
{
    IEnumerable<FilePath> sdkFiles = GetFiles("./sdk/*.json").Distinct();

    foreach (FilePath sdkFile in sdkFiles)
    {
        if (IsRunningOnWindows())
        {
            StartProcess("pwsh", $"-ExecutionPolicy Bypass -File ./script/dotnet-install.ps1 --jsonfile {sdkFile}");
        }
        else
        {
            StartProcess("bash", $"./script/dotnet-install.sh --jsonfile {sdkFile}");
        }
    }
});

Task("Restore")
    .Does(() =>
    {
        DotNetRestore(".");
    });

Task("RestoreWorkloads")
    .Does(() =>
    {
        DotNetWorkloadRestore("./floss-app-v3.slnx");
    });

Task("FastRunGui")
    .Does(() =>
    {
        DotNetRun("./src/BlazorHybrid/BlazorHybrid.csproj", new DotNetRunSettings
        {
            Configuration = configuration,
            Framework = "net10.0-windows10.0.19041.0"
        });
    });

Task("RunGui")
    .IsDependentOn("InstallSdk")
    .IsDependentOn("Restore")
    .IsDependentOn("RestoreWorkloads")
    .IsDependentOn("FastRunGui")
    .Does(() => { });

Task("FastRunTests")
    .Does(() =>
    {
        var projects = GetFiles("test/**/*.csproj");

        foreach (var proj in projects)
        {
            DotNetTest(proj.FullPath);
        }
    });

Task("RunTests")
    .IsDependentOn("InstallSdk")
    .IsDependentOn("Restore")
    .IsDependentOn("RestoreWorkloads")
    .IsDependentOn("FastRunTests")
    .Does(() => { });

Task("GenerateCoverage")
    .IsDependentOn("RunTests")
    .Does(() =>
    {
        ReportGenerator(new GlobPattern("**/coverage.cobertura.xml"), Directory("./coveragereport"), new ReportGeneratorSettings
        {
            ReportTypes = [ReportGeneratorReportType.Html],
        });
    });

Task("Benchmark")
    .IsDependentOn("InstallSdk")
    .Does(() =>
    {
        var projects = GetFiles("bench/**/*.csproj");
        foreach (var proj in projects)
        {
            StartProcess("dotnet", new ProcessSettings
            {
                Arguments = $"run {proj} -c Release -- --filter \"{benchmarkFilter}\"",
            });
        }
    });

RunTarget(target);
