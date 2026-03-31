var target = Argument("target", "FastRunGui");
var configuration = Argument("configuration", "Release");

Task("InstallSdk").Does(() =>
{
    if (IsRunningOnWindows())
    {
        StartProcess("pwsh", "-Command \"Invoke-WebRequest -Uri https://dot.net/v1/dotnet-install.ps1 -OutFile ./dotnet-install.ps1\"");
        StartProcess("pwsh", "-ExecutionPolicy Bypass -File ./dotnet-install.ps1 --jsonfile ./sdk.json");
        StartProcess("pwsh", "-Command \"Remove-Item -Path ./dotnet-install.ps1\"");
    }
    else
    {
        StartProcess("sh", "-c \"curl -sSL 'https://dot.net/v1/dotnet-install.sh' > ./dotnet-install.sh.tmp\"");

        if (Context.Tools.Resolve("gpg") != null)
        {
            StartProcess("sh", "-c \"curl -sSL 'https://dot.net/v1/dotnet-install.asc' > ./dotnet-install.asc.tmp\"");
            StartProcess("sh", "-c \"curl -sSL 'https://dot.net/v1/dotnet-install.sig' > ./dotnet-install.sig.tmp\"");
            StartProcess("sh", "-c \"gpg --import ./dotnet-install.asc.tmp\"");
            int gpgExitCode = StartProcess("sh", "-c \"gpg --verify ./dotnet-install.sig.tmp ./dotnet-install.sh.tmp\"");
            if (gpgExitCode != 0)
            {
                throw new CakeException("The dotnet install script failed the GPG integrity check.");
            }

            StartProcess("sh", "-c \"rm ./dotnet-install.asc.tmp\"");
            StartProcess("sh", "-c \"rm ./dotnet-install.sig.tmp\"");
        }

        StartProcess("sh", "-c \"chmod +x ./dotnet-install.sh.tmp\"");
        StartProcess("sh", "-c \"./dotnet-install.sh.tmp --jsonfile ./sdk.json\"");

        StartProcess("sh", "-c \"rm ./dotnet-install.sh.tmp\"");
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

RunTarget(target);
