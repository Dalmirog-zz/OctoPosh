using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management.Automation;

namespace Octoposh.Model
{
    /// <summary>
    /// This class handles everything re how Octoposh deals with Octo.exe, from discovery to download from Nuget/Chocolatey
    /// </summary>
    public class OctopusToolsHandler
    {
        /// <summary>
        /// Sets the path value of the environment variable that Octoposh will use to find/download the versions of Octo.exe
        /// </summary>
        /// <param name="path">Path where the folders with the copies of Octo.exe are sitting</param>
        public void SetToolsFolder(string path)
        {
            OctoposhEnvVariables.OctopusToolsFolder = path;
        }

        /// <summary>
        /// Gets the current path of the Octopus Tools Folder. 
        /// </summary>
        public string GetToolsFolder()
        {
            if (OctoposhEnvVariables.OctopusToolsFolder == null)
            {
                Console.WriteLine("The Octopus Tools folder has not been set yet. Run [Get-help Set-OctopusToolsFolder] to learn how to do this");
                return null;
            }
            else
            {
                return OctoposhEnvVariables.OctopusToolsFolder;
            }
        }

        /// <summary>
        /// Gets all the versions of Octo.exe available in the Octopus Tools Folder.
        /// </summary>
        public List<OctopusToolVersion> GetAllToolVersions()
        {
            var versions = new List<OctopusToolVersion>();

            var toolsFolder = GetToolsFolder();

            var toolsFolderChilds = Directory.GetDirectories(toolsFolder, "*",SearchOption.TopDirectoryOnly);

            if (toolsFolderChilds.Length == 0)
            {
                throw OctoposhExceptions.NoOctoExeVersionFoundInToolsFolder();
            }

            foreach (var child in toolsFolderChilds)
            {
                var files = Directory.GetFiles(child, "Octo.exe*");

                foreach (var file in files)
                {
                    var info = FileVersionInfo.GetVersionInfo(file);

                    versions.Add(new OctopusToolVersion()
                    {
                        Version = Version.Parse(info.ProductVersion),
                        Path = file.Replace(@"\\",@"\")
                    });
                }
            }

            if (versions.Count == 0)
            {
                throw OctoposhExceptions.NoOctoExeVersionFoundInToolsFolder();
            }
            else
            {
                return new List<OctopusToolVersion>(versions.OrderByDescending(v => v.Version));
            }
        }

        /// <summary>
        /// Gets the highest version Octo.exe available on the Octopus Tools Folder
        /// </summary>
        /// <returns></returns>
        public OctopusToolVersion GetLatestToolVersion()
        {
            return GetAllToolVersions().FirstOrDefault();
        }

        /// <summary>
        /// Gets a specific Octo.exe version from the Octopus Tools Folder
        /// </summary>
        /// <param name="version"></param>
        /// <exception cref="OctoposhExceptions.ToolVersionNotfound"></exception>
        public OctopusToolVersion GetToolByVersion(string version)
        {
            var toolVersion = GetAllToolVersions().FirstOrDefault(v => v.Version == Version.Parse(version));

            if (toolVersion == null)
            {
                throw OctoposhExceptions.ToolVersionNotfound(version);
            }
            else
            {
                return toolVersion;
            }

        }

        /// <summary>
        /// Sets the path of the Environment Variable that references Octo.exe
        /// </summary>
        /// <param name="path">Full path of Octo.exe</param>
        public void SetOctoExePath(string path)
        {
            OctoposhEnvVariables.Octoexe = path;
        }

        /// <summary>
        /// Returns the current value of the environment variable that references Octo.exe
        /// </summary>
        /// <returns></returns>
        public string GetToolPath()
        {
            return OctoposhEnvVariables.Octoexe;
        }

        internal void DownloadOctoExe(string version = "Latest")
        {
            NugetHandler.DownloadVersion(version, GetToolsFolder());
        }
    }
}
