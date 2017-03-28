using System;
using Octoposh.Model;
using System.Management.Automation;

namespace Octoposh.Cmdlets
{
    /// <summary>
    /// <para type="synopsis">This cmdlet downloads Octo.exe from Nuget</para>
    /// </summary>
    /// <summary>
    /// <para type="description">This cmdlet downloads Octo.exe from Nuget</para>
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
    /// <para type="link" uri="https://github.com/Dalmirog/OctoPosh/wiki">Wiki: </para>
    /// <para type="link" uri="https://gitter.im/Dalmirog/OctoPosh#initial">QA and Feature requests: </para>
    [Cmdlet("Install", "OctopusTool", DefaultParameterSetName = ByLatest)]
    [OutputType(typeof(void))]
    public class InstallOctopusPath : PSCmdlet
    {

        private const string ByLatest = "ByLatest";
        private const string ByVersion = "ByVersion";
        /// <summary>
        /// <para type="description">Sets the version of Octo.exe that will be downloaded from Nuget</para>
        /// </summary>
        [ValidateNotNullOrEmpty()]
        [Parameter(Mandatory = true, ParameterSetName = ByVersion)]
        public string Version { get; set; }

        /// <summary>
        /// <para type="description">Tells the cmdlet to download the latest version available in Nuget</para>
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
            var OctoExePath = "";

            if (ParameterSetName == ByVersion)
            {
                OctoExePath = OctopusTools.DownloadOctoExe(Version);
            }
            else
            {
                OctoExePath = OctopusTools.DownloadOctoExe();
            }

            if (SetAsDefault)
            {
                OctopusTools.SetOctoExePath(OctoExePath);
            }
        }
    }
}