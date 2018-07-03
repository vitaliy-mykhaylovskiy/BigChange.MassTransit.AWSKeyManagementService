#tool "nuget:?package=NUnit.ConsoleRunner"
var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var sln = "./BigChange.MassTransit.AwsKeyManagementService.sln";
var version = EnvironmentVariable("APPVEYOR_BUILD_VERSION") ?? "0.0.0";

Task("Clean")
    .Does(() =>
{
    var settings = new DotNetCoreCleanSettings
    {
        Configuration = configuration
    };
    
    DotNetCoreClean(sln, settings);
});

Task("Restore-NuGet-Packages")
    .IsDependentOn("Clean")
    .Does(() =>
{
    DotNetCoreRestore(sln, new DotNetCoreRestoreSettings ());
});

Task("Build")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
{
    DotNetCoreBuild(sln, new DotNetCoreBuildSettings
    {
        Configuration = configuration
    });
});

Task("Run-Unit-Tests")
    .IsDependentOn("Build")
    .WithCriteria(false)
    .Does(() =>
{
    var testAssemblies = GetFiles("./test/**/*.csproj");

    Console.WriteLine($"test count: {testAssemblies.Count}");

    foreach(var t in testAssemblies){
        Console.WriteLine(t);
    }
    NUnit3(testAssemblies);
});

Task("Pack-Library")
    .IsDependentOn("Run-Unit-Tests")
    .Does(() =>
{
     var settings = new DotNetCorePackSettings
     {
         Configuration = configuration,
         OutputDirectory = "./artifacts/",
         NoBuild = true,
        ArgumentCustomization = args => args.Append("/p:Version=" + version)
     };

     DotNetCorePack(sln, settings);
});


Task("Upload-Artifacts")
    .WithCriteria(BuildSystem.IsRunningOnAppVeyor)
    .IsDependentOn("Pack-Library")
    .Does(() =>
{
    foreach (var filePath in GetFiles("./artifacts/")) 
    { 
        if (AppVeyor.IsRunningOnAppVeyor)
        {
            AppVeyor.UploadArtifact(filePath, new AppVeyorUploadArtifactsSettings
            {
                DeploymentName = filePath.GetFilename().ToString()
            });
        }
    }
});

Task("Default")
    .IsDependentOn("Upload-Artifacts");

RunTarget(target);