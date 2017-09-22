#tool nuget:?package=NUnit.ConsoleRunner&version=3.7.0
#tool nuget:?package=Cake.CoreCLR&version=0.21.1
#addin "Cake.Powershell"

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var ConfigFile = Argument("ConfigFile","");
var BinaryVersion = Argument("BinaryVersion","");
var RemoveOctopusInstanceAtBeggining = Argument("RemoveOctopusInstanceAtBeggining", false);
var CreateOctopusInstance = Argument("CreateOctopusInstance", false);
var RemoveOctopusInstanceAtEnd = Argument("RemoveOctopusInstanceAtEnd", false);
var GenerateTestData = Argument("GenerateTestData", false);

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

// Define directories.
//The compiled module will be sent to modulePublishDir
var modulePublishDir = MakeAbsolute(Directory("./Octoposh/Publish/")).FullPath;
var testDataGeneratorPublishDir = MakeAbsolute(Directory("./Octoposh.TestDataGenerator/Publish/")).FullPath;
var testOutputDir = MakeAbsolute(Directory("./Octoposh.Tests/Publish")).FullPath;

var pathsToClean = new string[]{modulePublishDir,testDataGeneratorPublishDir,testOutputDir};

var ManifestPath = Directory(modulePublishDir) + Directory("Octoposh.psd1");

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")    
    .Does(() =>
{
    //Add the path to clean to pathstoClean
    foreach(var dir in pathsToClean){
      CleanDirectory(dir);  
    }
});

Task("Restore-NuGet-Packages")
    .IsDependentOn("Clean")
    .Does(() =>
{    
    NuGetRestore("Octoposh.sln");
});

Task("Replace-Test-Project-App-Settings")   
    .IsDependentOn("Restore-NuGet-Packages")
    .Description("Replace appconfig files on projects so they are run against the Octopus Instance that's being created by this build.")
    .Does(() =>
{    
    StartPowershellFile("Octoposh.Tests/Scripts/replaceAppSettings.ps1", new PowershellSettings()
        .SetFormatOutput()
        .SetLogOutput()
        .WithArguments(args=>
        {           
            args.Append("ConfigFile",ConfigFile);           
        }));

    StartPowershellFile("Octoposh.TestDataGenerator/Scripts/replaceAppSettings.ps1", new PowershellSettings()
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
        Information("Running MSBuild for Octoposh.csproj...");
        MSBuild("Octoposh/Octoposh.csproj", settings =>
        settings.SetConfiguration(configuration)
            .WithProperty("OutDir", modulePublishDir));

        //Build Tests
        Information("Running MSBuild for Octoposh.Tests.csproj...");
        MSBuild("Octoposh.Tests/Octoposh.Tests.csproj", settings =>
        settings.SetConfiguration(configuration));

        //Build website
        var testDataGeneratorSettings = new DotNetCorePublishSettings
        {
            Framework = "netcoreapp2.0",
            Configuration = "Release",
            OutputDirectory = testDataGeneratorPublishDir
        };

        Information("Running dotnet publish for Octoposh.TestDataGenerator.csproj...");
        DotNetCorePublish("./Octoposh.TestDataGenerator/Octoposh.TestDataGenerator.csproj", testDataGeneratorSettings);
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
            args.Append("Version",BinaryVersion);
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

Task("Run-TestDatagenerator")
    .IsDependentOn("Create-Octopus-Instance")
    .WithCriteria(GenerateTestData == true)
    .Description("Runs the TestDataGenerator console to create all the Octopus resources needed for the tests to run ")    
    .Does(() =>
{    
    StartPowershellFile("Scripts/OctopusServer.ps1", new PowershellSettings()
        .SetFormatOutput()
        .SetLogOutput()
        .WithArguments(args=>
        {
            args.Append("Action","GenerateTestData");           
            args.Append("ConfigFile",ConfigFile);
        }));    
});

Task("Start-Octopus-Server")
    .IsDependentOn("Run-TestDatagenerator")
    .Description("Make sure the Octopus Server is started before running tests")   
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
        NoResults = false, //Needs to be "true" for NUnit to create TestResult.xml
        WorkingDirectory = testOutputDir //TestResult.xml will be dropped under "WorkingDirectory"
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
    //.IsDependentOn("Run-Unit-Tests"); 
    .IsDependentOn("Remove-Octopus-Instance-At-End"); 

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);