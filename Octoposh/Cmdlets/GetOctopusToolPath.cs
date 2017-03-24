using System;
using Octoposh.Model;
using System.Collections.Generic;
using System.Management.Automation;

namespace Octoposh.Cmdlets
{
    /// <summary>
    /// <para type="synopsis">This cmdlet gets the path of the "Octopus Tools Folder" that will be used by other Octoposh cmdlets. To learn how to set this path, run Get-Help Set-OctopusToolsFolder</para>
    /// <para type="synopsis">The "Octopus Tools Folder" structure has to have inside of it 1 folder, each with 1 different version of Octo.exe. For example if you set the folder to be C:\Tools, the folder structure inside of it should be:</para>
    /// <para type="synopsis">C:\tools\1.0.0\Octo.exe</para>
    /// <para type="synopsis">C:\tools\2.0.0\Octo.exe</para>
    /// <para type="synopsis">C:\tools\Whatever\Octo.exe</para>
    /// </summary>
    /// <summary>
    /// <para type="description">This cmdlet gets the path of the "Octopus Tools Folder" that will be used by other Octoposh cmdlets. To learn how to set this path, run Get-Help Set-OctopusToolsFolder</para>
    /// <para type="description">The "Octopus Tools Folder" structure has to have inside of it 1 folder, each with 1 different version of Octo.exe. For example if you set the folder to be C:\Tools, the folder structure inside of it should be:</para>
    /// <para type="description">C:\tools\1.0.0\Octo.exe</para>
    /// <para type="description">C:\tools\2.0.0\Octo.exe</para>
    /// <para type="description">C:\tools\Whatever\Octo.exe</para>
    /// </summary>
    /// <example>   
    ///   <code>PS C:\> Get-OctopusToolsFolder</code>
    ///   <para>Gets the path of the "Octopus Tools Folder"</para>
    /// </example>
    /// <para type="link" uri="http://Octoposh.net">WebSite: </para>
    /// <para type="link" uri="https://github.com/Dalmirog/OctoPosh/">Github Project: </para>
    /// <para type="link" uri="https://github.com/Dalmirog/OctoPosh/wiki">Wiki: </para>
    /// <para type="link" uri="https://gitter.im/Dalmirog/OctoPosh#initial">QA and Feature requests: </para>
    [Cmdlet("Get", "OctopusToolPath")]
    [OutputType(typeof(string))]
    public class GetOctopusToolPath : PSCmdlet
    {
        protected override void ProcessRecord()
        {
            var octoTools = new OctopusToolsHandler();
            WriteObject(octoTools.GetToolPath());
        }
    }
}