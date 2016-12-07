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
		}));
});

Task("Stop-Octopus-Server")
    .IsDependentOn("Create-Octopus-Instance")
    .Description("Stops the Octopus server so its database can be restored")	
    .Does(() =>
{
    StartPowershellFile("Scripts/OctopusServer.ps1", new PowershellSettings()
        .SetFormatOutput()
        .SetLogOutput()
		.WithArguments(args=>
		{
			args.Append("Action","StopService");
			args.Append("InstanceName",OctopusInstance);
		}));
});

Task("Restore-Test-DB")
	.IsDependentOn("Stop-Octopus-Server")
	.Description("Restores the Octopus database to a clean state so it can later one use the Octopus Import feature to import all the test data")
	.Does(() =>{
		ExecuteSqlFile("./Scripts/RestoreCleanDatabase.sql", new SqlQuerySettings()
		{
			Provider = "MsSql",
			ConnectionString = ConnectionString
		});
	});

Task("Import-Octopus-Backup")
    .IsDependentOn("Restore-Test-DB")
    .Description("Imports the Octopus backup that's on source control with the project")	
    .Does(() =>
{
    StartPowershellFile("Scripts/OctopusServer.ps1", new PowershellSettings()
        .SetFormatOutput()
        .SetLogOutput()
		.WithArguments(args=>
		{
			args.Append("Action","ImportBackup");
			args.Append("Password",OctopusPassword);
			args.Append("InstanceName",OctopusInstance);
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

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Run-Unit-Tests");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);