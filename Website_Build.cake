#tool nuget:?package=NUnit.ConsoleRunner&version=3.4.0
#addin "Cake.Powershell"

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Debug");

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

// Define directories.
//The compiled Website will be sent to publishDir. The Webjovs will be sent to publishDir + Path of the web job.
var publishDir = Directory("./Octoposh.Web/Publish/");

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")    
    .Does(() =>
{
    CleanDirectory(publishDir);
});

Task("Restore-NuGet-Packages")
    .IsDependentOn("Clean")
    .Does(() =>
{
    Information("Running dotnet restore for DownloadsTracker.csproj...");
    DotNetCoreRestore("DownloadsTracker/DownloadsTracker.csproj");

    Information("Running dotnet restore for Octoposh.Web.csproj...");
    DotNetCoreRestore("Octoposh.Web/Octoposh.Web.csproj");    
});

Task("Build and Publish")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
    {

    //Build website
    var webSiteSettings = new DotNetCorePublishSettings
     {
         Framework = "netcoreapp2.0",
         Configuration = "Release",
         OutputDirectory = publishDir
     };

     //Build downloadsTracker webJob
     var downloadsTrackerJobSettings = new DotNetCorePublishSettings
     {
         Framework = "netcoreapp2.0",
         Configuration = "Release",
         OutputDirectory = Directory(publishDir + Directory("App_Data/jobs/triggered/DownloadsTracker"))
     };    

    Information("Running dotnet publish for DownloadsTracker.csproj...");
    DotNetCorePublish("./Octoposh.Web/Octoposh.Web.csproj", webSiteSettings);

    Information("Running dotnet publish for Octoposh.Web.csproj...");
    DotNetCorePublish("./DownloadsTracker/DownloadsTracker.csproj", downloadsTrackerJobSettings);
    });

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Build and Publish");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);