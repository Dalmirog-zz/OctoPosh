using Octoposh.Model;
using System.Management.Automation;

namespace Octoposh.Cmdlets
{
    /// <summary>
    /// <para type="synopsis">This cmdlet sets the value of the variable $env:OctoExe</para>
    /// </summary>
    /// <summary>
    /// <para type="description">This cmdlet sets the value of the variable $env:OctoExe</para>
    /// </summary>
    /// <example>   
    ///   <code>PS C:\> Set-OctopusToolPath -version 1.0.0 </code>
    ///   <para>Sets the value of $env:OctoExe to the path of the Octo.exe version 1.0.0 returned by the cmdlet Get-OctopusToolsVersion</para>
    /// </example>
    /// <example>   
    ///   <code>PS C:\> Set-OctopusToolPath -path C:\tools\1.0.0\Octo.exe </code>
    ///   <para>Sets the value of $env:OctoExe to the path C:\Tools\1.0.0\Octo.exe</para>
    /// </example>
    /// <example>   
    ///   <code>PS C:\>Get-OctopusToolsVersion -latest | Set-OctopusToolPath </code>
    ///   <para>Gets the latest version of Octo.exe in $env:OctopusToolsFolder and sets $env:OctoExe with its path</para>
    /// </example>
    /// <para type="link" uri="http://Octoposh.net">WebSite: </para>
    /// <para type="link" uri="https://github.com/Dalmirog/OctoPosh/">Github Project: </para>
    /// <para type="link" uri="https://github.com/Dalmirog/OctoPosh/wiki">Wiki: </para>
    /// <para type="link" uri="https://gitter.im/Dalmirog/OctoPosh#initial">QA and Feature requests: </para>
    [Cmdlet("Set", "OctopusToolPath",DefaultParameterSetName = ByPath)]
    [OutputType(typeof(void))]
    public class SetOctopusToolPath : PSCmdlet
    {

        private const string ByPath = "ByPath";
        private const string ByVersion = "ByVersion";
        /// <summary>
        /// <para type="description">Sets the value of $env:Octoexe based on the version of Octo.exe found on the $env:OctopusToolsFolder folder. This cmdlet uses Get-OctopustoolsVersion in the background to validate the path of the executable</para>
        /// </summary>
        [Parameter(Mandatory = true, ParameterSetName = ByVersion)]
        public string Version { get; set; }

        /// <summary>
        /// <para type="description">Sets the value of $env:Octoexe using a literal path. The cmdlet won't validate if the path leads to an existing octo.exe file</para>
        /// </summary>
        [Parameter(Mandatory = true, ParameterSetName = ByPath ,ValueFromPipelineByPropertyName = true, Position = 0)]
        public string Path { get; set; }

        protected override void ProcessRecord()
        {
            var OctopusTools = new OctopusToolsHandler();

            if (ParameterSetName == ByPath)
            {
                OctopusTools.SetOctoExePath(Path);
            }
            else
            {
                var tool = OctopusTools.GetToolByVersion(Version);
                OctopusTools.SetOctoExePath(tool.Path);
            }
        }
    }
}