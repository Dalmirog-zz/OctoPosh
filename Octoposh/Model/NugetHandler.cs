
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
        private static string repositoryURL = "https://packages.nuget.org/api/v2";
        private static IPackageRepository Repo = PackageRepositoryFactory.Default.CreateRepository(repositoryURL);

        private static string GetPackageVersion(string version = "Latest")
        {
            if (version == "Latest")
            {
                return Repo.FindPackage(PackageID).Version.ToFullString();
            }
            else
            {
                return Repo.FindPackage(PackageID,SemanticVersion.Parse(version)).Version.ToFullString();
            }
        }
         
        public static string DownloadVersion(string version, string basePath)
        {
            //Initialize the package manager
            try
            {
                var packageVersion = GetPackageVersion(version);
                var downloadPath = Path.Combine(basePath, string.Concat("OctopusTools.", packageVersion));

                PackageManager packageManager = new PackageManager(Repo, downloadPath);

                //Download and unzip the package
                packageManager.InstallPackage(PackageID);
                var exeOriginalPath = Path.Combine(downloadPath, string.Concat(PackageID,".",packageVersion), "tools","Octo.exe");
                var exeDestinationPath = Path.Combine(downloadPath, "Octo.exe");

                File.Move(exeOriginalPath, exeDestinationPath);
                Directory.Delete(Path.Combine(downloadPath, string.Concat(PackageID, ".", packageVersion)),true);

                return exeDestinationPath;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}