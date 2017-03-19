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
var Version = Argument("Version","");
var ManifestPath = Argument("ManifestPath","");

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

// Define directories.
var buildDir = Directory("./Octoposh/bin/") + Directory(configuration);

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Update-Module-Manifest")
    .Description("Updates the module mainfest")   
    .Does(() =>
{
    StartPowershellFile("Scripts/UpdateModuleManifest.ps1", new PowershellSettings()
        .SetFormatOutput()
        .SetLogOutput()
        .WithArguments(args=>
        {
            args.Append("Version","Version");
            args.Append("ManifestPath","ManifestPath");            
        }));
});

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Update-Module-Manifest");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);