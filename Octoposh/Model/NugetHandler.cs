
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management.Automation;
using NuGet;

namespace Octoposh.Model
{
    internal static class NugetHandler
    {
        //THIS http://blog.nuget.org/20130520/Play-with-packages.html
        private static string PackageID = "OctopusTools";
        private static string repositoryURL = OctoposhEnvVariables.NugetRepositoryURL;
        private static IPackageRepository Repo = PackageRepositoryFactory.Default.CreateRepository(repositoryURL);

        private static string GetPackageVersion(string version = "Latest")
        {
            if (version == "Latest")
            {
                Console.WriteLine($"Resolving latest version of the package [{PackageID}] in [{repositoryURL}]");
                var latestVersion = Repo.FindPackage(PackageID).Version.ToFullString();

                Console.WriteLine($"Latest version found was {latestVersion}");
                return latestVersion;
            }
            else
            {
                Console.WriteLine($"Looking for version [{version}] of the package [{PackageID}] in [{repositoryURL}]");
                return Repo.FindPackage(PackageID,SemanticVersion.Parse(version)).Version.ToFullString();
            }
        }
         
        public static void DownloadVersion(string version, string basePath)
        {
            //Initialize the package manager
            try
            {
                var packageVersion = GetPackageVersion(version);
                var downloadPath = Path.Combine(basePath, string.Concat("OctopusTools.", packageVersion));

                PackageManager packageManager = new PackageManager(Repo, downloadPath);

                //Download and unzip the package
                Console.WriteLine($"Downloading [{PackageID}] version [{packageVersion}] to [{downloadPath}]");
                packageManager.InstallPackage(PackageID,SemanticVersion.Parse(packageVersion));

                //By default octo.exe gets downloaded in downloadPath\PackageId.PackageVersion\Tools\Octo.exe . So we are moving it to downloadpath\Octo.exe and deleting everything else.
                var exeOriginalPath = Path.Combine(downloadPath, string.Concat(PackageID, ".", packageVersion), "tools", "Octo.exe");
                var exeDestinationPath = Path.Combine(downloadPath, "Octo.exe");

                Console.WriteLine($"Octo.exe downloaded to [{exeOriginalPath}]. Copying it to [{exeDestinationPath}]");
                File.Copy(exeOriginalPath,exeDestinationPath,true);

                Console.WriteLine("Deleting extra files from package extraction");
                Directory.Delete(Path.Combine(downloadPath, string.Concat(PackageID, ".", packageVersion)),true);
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}