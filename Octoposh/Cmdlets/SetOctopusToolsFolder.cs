using Octoposh.Model;
using System.Management.Automation;

namespace Octoposh.Cmdlets
{
    /// <summary>
    /// <para type="synopsis">This cmdlet sets the path of the "Octopus Tools Folder". This folder is where Install-OctopusTool will download Octo.exe, and its also from where Get-OctopusToolVersion will resolve the path of the downloaded Octo.exe versions.</para>
    /// <para type="synopsis">For Get-OctopusToolVersion to be able to find Octo.exe version inside of the "Octopus Tools Folder", the folder structure must be like this:</para>
    /// <para type="synopsis">["Octopus Tools Folder"]\[Child Folder]\Octo.exe</para>
    /// <para type="synopsis">For example, given the following structure:</para>
    /// <para type="synopsis">["Octopus Tools Folder"]\1.0.0\Octo.exe</para>
    /// <para type="synopsis">["Octopus Tools Folder"]\SomeFolderName\Octo.exe</para>
    /// <para type="synopsis">["Octopus Tools Folder"]\SomeFolderName\AnotherFolder\Octo.exe</para>
    /// <para type="synopsis">The first 2 Octo.exe versions will be properly discovered, but the 3rd one wont because its not on the root of a direct child of the "Octopus Tools Folder"</para>
    /// </summary>
    /// <summary>
    /// <para type="description">This cmdlet sets the path of the "Octopus Tools Folder". This folder is where Install-OctopusTool will download Octo.exe, and its also from where Get-OctopusToolVersion will resolve the path of the downloaded Octo.exe versions.</para>
    /// <para type="description">For Get-OctopusToolVersion to be able to find Octo.exe version inside of the "Octopus Tools Folder", the folder structure must be like this:</para>
    /// <para type="description">["Octopus Tools Folder"]\[Child Folder]\Octo.exe</para>
    /// <para type="description">For example, given the following structure:</para>
    /// <para type="description">["Octopus Tools Folder"]\1.0.0\Octo.exe</para>
    /// <para type="description">["Octopus Tools Folder"]\SomeFolderName\Octo.exe</para>
    /// <para type="description">["Octopus Tools Folder"]\SomeFolderName\AnotherFolder\Octo.exe</para>
    /// <para type="description">The first 2 Octo.exe versions will be properly discovered, but the 3rd one wont because its not on the root of a direct child of the "Octopus Tools Folder"</para>
    /// </summary>
    /// <example>   
    ///   <code>PS C:\> Set-OctopusToolsFolder -path C:\tools</code>
    ///   <para>Sets the "Octopus Tools Folder" to "C:\Tools"</para>
    /// </example>
    /// <para type="link" uri="http://Octoposh.net">WebSite: </para>
    /// <para type="link" uri="https://github.com/Dalmirog/OctoPosh/">Github Project: </para>
    /// <para type="link" uri="http://octoposh.readthedocs.io">Wiki: </para>
    /// <para type="link" uri="https://gitter.im/Dalmirog/OctoPosh#initial">QA and Feature requests: </para>
    [Cmdlet("Set", "OctopusToolsFolder")]
    [OutputType(typeof(void))]
    public class SetOctopusToolsFolder : PSCmdlet
    {
        /// <summary>
        /// <para type="description">Sets the path of the "Octopus Tools folder".</para>
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