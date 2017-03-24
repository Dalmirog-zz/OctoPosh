using System;
using Octoposh.Model;
using System.Collections.Generic;
using System.Management.Automation;

namespace Octoposh.Cmdlets
{
    /// <summary>
    /// <para type="synopsis">Gets a list of available versions of Octo.exe in the folder set on the "Octopus Tools Folder". If you don't know what this path is run [Get-Help Set-OctopusToolsFolder] to learn more about it</para>
    /// </summary>
    /// <summary>
    /// <para type="description">Gets a list of available versions of Octo.exe in the folder set on the "Octopus Tools Folder". If you don't know what this path is run [Get-Help Set-OctopusToolsFolder] to learn more about it</para>
    /// </summary>
    /// <example>   
    ///   <code>PS C:\> Get-OctopusToolVersion</code>
    ///   <para>Gets a list of available versions of Octo.exe in the folder set on the "Octopus Tools Folder". If you don't know what this path is run [Get-Help Set-OctopusToolsFolder] to learn more about it</para>
    /// </example>
    /// <example>   
    ///   <code>PS C:\> Get-OctopusToolVersion -latest</code>
    ///   <para>Gets the latest version of Octo.exe in the folder set on the "Octopus Tools Folder". If you don't know what this path is run [Get-Help Set-OctopusToolsFolder] to learn more about it</para>
    /// </example>
    /// <para type="link" uri="http://Octoposh.net">WebSite: </para>
    /// <para type="link" uri="https://github.com/Dalmirog/OctoPosh/">Github Project: </para>
    /// <para type="link" uri="https://github.com/Dalmirog/OctoPosh/wiki">Wiki: </para>
    /// <para type="link" uri="https://gitter.im/Dalmirog/OctoPosh#initial">QA and Feature requests: </para>
    [Cmdlet("Get", "OctopusToolVersion",DefaultParameterSetName = All)]
    [OutputType(typeof(List<OctopusToolVersion>))]
    [OutputType(typeof(OctopusToolVersion))]
    public class GetOctopusToolVersion : PSCmdlet
    {
        private const string ByLatest = "Latest";
        private const string ByVersion = "ByVersion";
        private const string All = "All";

        /// <summary>
        /// <para type="description">If set to TRUE the cmdlet will only return the highest version of Octo.exe found on the child folders $env:OctopusToolsFolder. If you don't know what this path is run [Get-Help Set-OctopusToolsFolder] to learn more about it</para>
        /// </summary>
        [Parameter(ParameterSetName = ByLatest)]
        public SwitchParameter Latest { get; set; }

        /// <summary>
        /// <para type="description">Name of the Machine to filter by</para>
        /// </summary>
        [Alias("Name")]
        [ValidateNotNullOrEmpty()]
        [Parameter(Position = 0, ValueFromPipeline = true, ParameterSetName = ByVersion)]
        public string Version { get; set; }

        protected override void ProcessRecord()
        {
            var octopusTools = new OctopusToolsHandler();

            switch (ParameterSetName)
            {
                case ByLatest:
                    WriteObject(octopusTools.GetLatestToolVersion());
                    break;
                case ByVersion:
                    WriteObject(octopusTools.GetToolByVersion(Version));
                    break;
                case All:
                    WriteObject(octopusTools.GetAllToolVersions());
                    break;
            }
        }
    }
}