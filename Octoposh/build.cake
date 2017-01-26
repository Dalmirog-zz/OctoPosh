#tool nuget:?package=NUnit.ConsoleRunner&version=3.4.0
#addin "Cake.SqlTools"
#addin "Cake.Powershell"

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Debug");
var CreateInstance = Argument("CreateInstance","");
var RemoveInstanceAtBeggining = Argument("RemoveInstanceAtBeggining","");
var RemoveInstanceAtEnd = Argument("RemoveInstanceAtEnd","");
var ConfigFile = Argument("ConfigFile","");

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

// Define directories.
var buildDir = Directory("./Octoposh/bin/") + Directory(configuration);

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")    
    .Does(() =>
{
    CleanDirectory(buildDir);
});

Task("Restore-NuGet-Packages")
    .IsDependentOn("Clean")
    .Does(() =>
{
    NuGetRestore("Octoposh.sln");
});

Task("Replace-Test-Project-App-Settings")   
    .IsDependentOn("Restore-NuGet-Packages")
    .Description("Replace app.config settings on test project so they are run against the Octopus Instance that's being created by this build.")    
    .Does(() =>
{
    StartPowershellFile("Octoposh.Tests/Scripts/replaceAppSettings.ps1", new PowershellSettings()
        .SetFormatOutput()
        .SetLogOutput()
        .WithArguments(args=>
        {           
            args.Append("ConfigFile",ConfigFile);           
        }));
});

Task("Build")
    .IsDependentOn("Replace-Test-Project-App-Settings")
    .Does(() =>
    {
        // Use MSBuild
        MSBuild("Octoposh.sln", settings =>
        settings.SetConfiguration(configuration));

    });

Task("Remove-Octopus-Instance-At-Beggining")    
    .IsDependentOn("Build")
    .Description("Removes the Octopus Test instance")   
    .Does(() =>
{
    StartPowershellFile("Scripts/OctopusServer.ps1", new PowershellSettings()
        .SetFormatOutput()
        .SetLogOutput()
        .WithArguments(args=>
        {
            args.Append("Action","RemoveInstance");
            args.Append("RemoveInstance",RemoveInstanceAtBeggining);
            args.Append("ConfigFile",ConfigFile);       
        }));
});

Task("Create-Octopus-Instance") 
    .IsDependentOn("Remove-Octopus-Instance-At-Beggining")
    .Description("Creates the test Octopus instance. If an instance with that name already exists, it won't be re-created but its data will be restored sing the backup in Source Control") 
    .Does(() =>
{
    StartPowershellFile("Scripts/OctopusServer.ps1", new PowershellSettings()
        .SetFormatOutput()
        .SetLogOutput()
        .WithArguments(args=>
        {
            args.Append("Action","CreateInstance");
            args.Append("CreateInstance",CreateInstance);           
            args.Append("ConfigFile",ConfigFile);           
        }));
});

Task("Import-Octopus-Backup")
    .IsDependentOn("Create-Octopus-Instance")
    .Description("Imports the Octopus backup that's on source control with the project")    
    .Does(() =>
{
    StartPowershellFile("Scripts/OctopusServer.ps1", new PowershellSettings()
        .SetFormatOutput()
        .SetLogOutput()
        .WithArguments(args=>
        {
            args.Append("Action","ImportBackup");           
            args.Append("ConfigFile",ConfigFile);
        }));
});

Task("Start-Octopus-Server")
    .IsDependentOn("Import-Octopus-Backup")
    .Description("Starts the Octopus server so the tests can run against it")   
    .Does(() =>
{
    StartPowershellFile("Scripts/OctopusServer.ps1", new PowershellSettings()
        .SetFormatOutput()
        .SetLogOutput()
        .WithArguments(args=>
        {
            args.Append("Action","StartService");
        }));
});


Task("Run-Unit-Tests")
    .IsDependentOn("Start-Octopus-Server")
    .Does(() =>
{
    NUnit3("./Octoposh.Tests/bin/" + configuration + "/*.Tests.dll", new NUnit3Settings {        
        NoResults = true    
    });
});

Task("Remove-Octopus-Instance-At-End")
    .IsDependentOn("Run-Unit-Tests")
    .Description("Removes the Octopus Test instance")   
    .Does(() =>
{
    StartPowershellFile("Scripts/OctopusServer.ps1", new PowershellSettings()
        .SetFormatOutput()
        .SetLogOutput()
        .WithArguments(args=>
        {
            args.Append("Action","RemoveInstance");
            args.Append("RemoveInstance",RemoveInstanceAtEnd);
            args.Append("ConfigFile",ConfigFile);
        }));
});

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Remove-Octopus-Instance-At-End");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);