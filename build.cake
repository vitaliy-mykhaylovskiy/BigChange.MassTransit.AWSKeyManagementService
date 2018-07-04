#tool "nuget:?package=NUnit.ConsoleRunner"
var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var sln = "./BigChange.MassTransit.AwsKeyManagementService.sln";
var version = EnvironmentVariable("APPVEYOR_BUILD_VERSION") ?? "0.0.0";

IProcess rabbitMqProcess = null;

Setup(context =>
{
    Information("Downloading RabbitMQ v3.7.5");
    var resource = DownloadFile("https://github.com/rabbitmq/rabbitmq-server/releases/download/v3.7.5/rabbitmq-server-windows-3.7.5.zip");

    Information("Unzip RabbitMQ v3.7.5");
    Unzip(resource, "./rabbitmq");

    Information("Starting RabbitMQ v3.7.5");
    rabbitMqProcess = StartAndReturnProcess("./rabbitmq/rabbitmq_server-3.7.5/sbin/rabbitmq-server.bat");
       
    Information("Waiting a second to make sure RabbitMQ is running and has not failed to run");
    if(rabbitMqProcess.WaitForExit(1000))
    {
        throw new Exception($"Failed to start RabbitMQ, Exit code: {rabbitMqProcess.GetExitCode()}");
    }ffffffffcvfcfcffffcfdf
});

Teardown(context =>
{
    rabbitMqProcess?.Kill();
});

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
    .Does(() =>
{
    var settings = new DotNetCoreTestSettings
    {
        Configuration = configuration,
        Logger = "trx",
    };

    var projectFiles = GetFiles("./test/**/*.csproj");
    foreach(var file in projectFiles)
    {
        DotNetCoreTest(file.FullPath, settings);
    }
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
    foreach (var filePath in GetFiles("./artifacts/*")) 
    { 
        AppVeyor.UploadArtifact(filePath, new AppVeyorUploadArtifactsSettings
        {
            DeploymentName = filePath.GetFilename().ToString()
        });
    }
});


Task("Upload-Test-Results")
    .WithCriteria(BuildSystem.IsRunningOnAppVeyor)
    .IsDependentOn("Run-Unit-Tests")
    .Does(() =>
{
    foreach (var filePath in GetFiles("./test/**/TestResults/*.trx")) 
    { 
        AppVeyor.UploadTestResults(filePath, AppVeyorTestResultsType.MSTest);
    }
});

Task("Default")
    .IsDependentOn("Upload-Artifacts")
    .IsDependentOn("Upload-Test-Results");

RunTarget(target);