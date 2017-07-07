using System;
using Octoposh.Model;
using System.Management.Automation;

namespace Octoposh.Cmdlets
{
    /// <summary>
    /// <para type="synopsis">This cmdlet downloads Octo.exe from Nuget to the "Octopus Tools Folder".To learn more about this path run Get-Help Set-OctopusToolsFolder</para>
    /// <para type="synopsis">By default this cmdlet will download "Octo.exe" from "https://packages.nuget.org/api/v2". If you want to override this behavior, simply assign a different feed path/url to $env:NugetRepositoryURL</para>
    /// </summary>
    /// <summary>
    /// <para type="description">This cmdlet downloads Octo.exe from Nuget to the "Octopus Tools Folder".To learn more about this path run Get-Help Set-OctopusToolsFolder</para>
    /// <para type="description">By default this cmdlet will download "Octo.exe" from "https://packages.nuget.org/api/v2". If you want to override this behavior, simply assign a different feed path/url to $env:NugetRepositoryURL</para>
    /// </summary>
    /// <example>   
    ///   <code>PS C:\> Install-OctopusTool -Latest </code>
    ///   <para>Downloads the latest version of Octo.exe to the Octopus Tools Folder.</para>
    /// </example>
    /// <example>   
    ///   <code>PS C:\> Install-OctopusTool -version 1.0.0 </code>
    ///   <para>Downloads version 1.0.0 of Octo.exe to the Octopus Tools Folder.</para>
    /// </example>
    /// <example>   
    ///   <code>PS C:\> Install-OctopusTool -version 1.0.0 -SetAsDefault </code>
    ///   <para>Downloads version 1.0.0 of Octo.exe to the Octopus Tools Folder and also sets it as the current defaul version</para>
    /// </example>
    /// <para type="link" uri="http://Octoposh.net">WebSite: </para>
    /// <para type="link" uri="https://github.com/Dalmirog/OctoPosh/">Github Project: </para>
    /// <para type="link" uri="http://octoposh.readthedocs.io">Wiki: </para>
    /// <para type="link" uri="https://gitter.im/Dalmirog/OctoPosh#initial">QA and Feature requests: </para>
    [Cmdlet("Install", "OctopusTool", DefaultParameterSetName = ByLatest)]
    [OutputType(typeof(void))]
    public class InstallOctopusTool : PSCmdlet
    {

        private const string ByLatest = "ByLatest";
        private const string ByVersion = "ByVersion";
        /// <summary>
        /// <para type="description">Downloads a specific version oc Octo.Exe from nuget</para>
        /// </summary>
        [ValidateNotNullOrEmpty()]
        [Parameter(Mandatory = true, ParameterSetName = ByVersion)]
        public string Version { get; set; }

        /// <summary>
        /// <para type="description">Tells the cmdlet to download the latest version of Octo.exe available in Nuget</para>
        /// </summary>
        [Parameter(ParameterSetName = ByLatest)]
        public SwitchParameter Latest { get; set; } = true;

        /// <summary>
        /// <para type="description">If set to true, the cmdlet will set the just downloaded version of Octo.exe as the default one, making it instantly available using $env:Octoexe</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public SwitchParameter SetAsDefault { get; set; }

        protected override void ProcessRecord()
        {
            var OctopusTools = new OctopusToolsHandler();
            string OctoExePath;
            
            try
            {
                if (ParameterSetName == ByVersion)
                {
                    OctopusTools.DownloadOctoExe(Version);
                    OctoExePath = OctopusTools.GetToolByVersion(Version).Path;
                }
                else
                {
                    OctopusTools.DownloadOctoExe();
                    OctoExePath = OctopusTools.GetLatestToolVersion().Path;
                }

                if (SetAsDefault)
                {
                    OctopusTools.SetOctoExePath(OctoExePath);
                }

                Console.WriteLine($"Successfuly downloaded to [{OctoExePath}]");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
        }
    }
}