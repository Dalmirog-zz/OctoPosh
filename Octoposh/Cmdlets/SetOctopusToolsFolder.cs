using Octoposh.Model;
using System.Management.Automation;

namespace Octoposh.Cmdlets
{
    /// <summary>
    /// <para type="synopsis">This cmdlet sets the path of the "Octopus Tools Folder" that will be used by other Octoposh cmdlets. To learn more about this folder run Get-Help Get-OctopusToolsFolder</para>
    /// </summary>
    /// <summary>
    /// <para type="Description">This cmdlet sets the path of the "Octopus Tools Folder" that will be used by other Octoposh cmdlets. To learn more about this folder run Get-Help Get-OctopusToolsFolder</para>
    /// </summary>
    /// <example>   
    ///   <code>PS C:\> Set-OctopusToolsFolder -path C:\tools</code>
    ///   <para>Sets the "Octopus Tools Folder" to "C:\Tools"</para>
    /// </example>
    /// <para type="link" uri="http://Octoposh.net">WebSite: </para>
    /// <para type="link" uri="https://github.com/Dalmirog/OctoPosh/">Github Project: </para>
    /// <para type="link" uri="https://github.com/Dalmirog/OctoPosh/wiki">Wiki: </para>
    /// <para type="link" uri="https://gitter.im/Dalmirog/OctoPosh#initial">QA and Feature requests: </para>
    [Cmdlet("Set", "OctopusToolsFolder")]
    [OutputType(typeof(void))]
    public class SetOctopusToolsFolder : PSCmdlet
    {
        /// <summary>
        /// <para type="description">Sets the path of the Octopus Tool folder that will be used by the other Octoposh cmdlets.</para>
        /// </summary>
        [Parameter(Mandatory = true,Position = 0)]
        public string Path { get; set; }

        protected override void ProcessRecord()
        {
            var octoTools = new OctopusToolsHandler();
            octoTools.SetToolsFolder(Path);
        }
    }
}