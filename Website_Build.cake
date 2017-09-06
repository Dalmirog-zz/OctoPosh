#tool nuget:?package=NUnit.ConsoleRunner&version=3.4.0
#addin "Cake.Bower"

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

Task("Restore-Packages")
    .IsDependentOn("Clean")
    .Does(() =>
{
    Information("Restoring Nuget Packages...");
    DotNetCoreRestore("DownloadsTracker/DownloadsTracker.csproj");
    DotNetCoreRestore("Octoposh.Web/Octoposh.Web.csproj");

    Information("Restoring Bower dependencies...");
    Bower.Install(s => s.UseWorkingDirectory("./Octoposh.Web"));    
});

Task("Build and Publish")
    .IsDependentOn("Restore-Packages")
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

    Information("Building Octoposh.Web...");
    DotNetCorePublish("./Octoposh.Web/Octoposh.Web.csproj", webSiteSettings);

    Information("Building DownloadsTracker...");
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