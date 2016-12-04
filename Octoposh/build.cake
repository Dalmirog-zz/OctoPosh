#tool nuget:?package=NUnit.ConsoleRunner&version=3.4.0
#addin "Cake.SqlTools"
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

Task("Stop-Octopus-Server")
    .IsDependentOn("Build")
    .Description("Stops the Octopus server so its database can be restored")	
    .Does(() =>
{
    StartPowershellFile("Scripts/OctopusServer.ps1", new PowershellSettings()
        .SetFormatOutput()
        .SetLogOutput()
		.WithArguments(args=>
		{
			args.Append("Action","StopService");
		}));
});

Task("Restore-Test-DB")
	.IsDependentOn("Stop-Octopus-Server")
	.Description("Restores the Octopus database to a clean state so it can later one use the Octopus Import feature to import all the test data")
	.Does(() =>{
		ExecuteSqlFile("./Scripts/RestoreCleanDatabase.sql", new SqlQuerySettings()
		{
			Provider = "MsSql",
			ConnectionString = "Server=(local)\\SQLEXPRESS;Database=OctoposhTest;Integrated Security=SSPI"
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

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Run-Unit-Tests");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);