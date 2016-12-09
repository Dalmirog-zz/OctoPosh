#tool nuget:?package=NUnit.ConsoleRunner&version=3.4.0
#addin "Cake.SqlTools"
#addin "Cake.Powershell"

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Debug");
var OctopusInstance = Argument("OctopusInstance","");
var ConnectionString = Argument("ConnectionString","");
var OctopusAdmin = Argument("OctopusAdmin","");
var OctopusPassword = Argument("OctopusPassword","");
var CreateInstance = Argument("CreateInstance","");
var RemoveInstance = Argument("RemoveInstance","");
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

Task("Build")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
{
      // Use MSBuild
      MSBuild("Octoposh.sln", settings =>
        settings.SetConfiguration(configuration));

});

Task("Create-Octopus-Instance")	
	.IsDependentOn("Build")
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
			args.Append("InstanceName",OctopusInstance);
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
			args.Append("InstanceName",OctopusInstance);
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
			args.Append("InstanceName",OctopusInstance);
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

Task("Remove-Octopus-Instance")
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
			args.Append("RemoveInstance",RemoveInstance);
			args.Append("InstanceName",OctopusInstance);	
			args.Append("ConfigFile",ConfigFile);		
		}));
});

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Remove-Octopus-Instance");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);