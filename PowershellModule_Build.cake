#tool nuget:?package=NUnit.ConsoleRunner&version=3.7.0
#tool nuget:?package=Cake.CoreCLR&version=0.21.1
#addin "Cake.Powershell"

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Debug");
var ConfigFile = Argument("ConfigFile","");
var Version = Argument("Version","");
var RemoveOctopusInstanceAtBeggining = Argument("RemoveOctopusInstanceAtBeggining", false);
var CreateOctopusInstance = Argument("CreateOctopusInstance", false);
var RemoveOctopusInstanceAtEnd = Argument("RemoveOctopusInstanceAtEnd", false);

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

// Define directories.
var buildDir = Directory("./Octoposh/bin/") + Directory(configuration) + Directory("Octoposh");
var ManifestPath = Directory(buildDir) + Directory("Octoposh.psd1");

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
        //Build PS Module     
        MSBuild("Octoposh/Octoposh.csproj", settings =>
        settings.SetConfiguration(configuration));
        
        //Build Tests
        MSBuild("Octoposh.Tests/Octoposh.Tests.csproj", settings =>
        settings.SetConfiguration(configuration));        
    });

Task("Update-Module-Manifest")
    .IsDependentOn("Build")
    .Description("Updates the module mainfest")   
    .Does(() =>
{    
    StartPowershellFile("Scripts/UpdateModuleManifest.ps1", new PowershellSettings()
        .SetFormatOutput()
        .SetLogOutput()
        .WithArguments(args=>
        {
            args.Append("Version",Version);
            args.Append("ManifestPath",ManifestPath);            
        }));    
});

Task("Remove-Octopus-Instance-At-Beggining")    
    .IsDependentOn("Update-Module-Manifest")
    .WithCriteria(RemoveOctopusInstanceAtBeggining == true)
    .Description("Removes the Octopus Test instance")   
    .Does(() =>
{    
    StartPowershellFile("Scripts/OctopusServer.ps1", new PowershellSettings()
        .SetFormatOutput()
        .SetLogOutput()
        .WithArguments(args=>
        {
            args.Append("Action","RemoveInstance");            
            args.Append("ConfigFile",ConfigFile);       
        }));

});

Task("Create-Octopus-Instance") 
    .IsDependentOn("Remove-Octopus-Instance-At-Beggining")
    .WithCriteria(CreateOctopusInstance == true)
    .Description("Creates the test Octopus instance. If an instance with that name already exists, it won't be re-created but its data will be restored sing the backup in Source Control") 
    .Does(() =>
{
    
    StartPowershellFile("Scripts/OctopusServer.ps1", new PowershellSettings()
        .SetFormatOutput()
        .SetLogOutput()
        .WithArguments(args=>
        {
            args.Append("Action","CreateInstance");            
            args.Append("ConfigFile",ConfigFile);           
        }));
    
});

Task("Import-Octopus-Backup")
    .IsDependentOn("Create-Octopus-Instance")
    .WithCriteria(CreateOctopusInstance == true)
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
    .WithCriteria(RemoveOctopusInstanceAtEnd == true)
    .Description("Removes the Octopus Test instance")   
    .Does(() =>
{
    
    StartPowershellFile("Scripts/OctopusServer.ps1", new PowershellSettings()
        .SetFormatOutput()
        .SetLogOutput()
        .WithArguments(args=>
        {
            args.Append("Action","RemoveInstance");            
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