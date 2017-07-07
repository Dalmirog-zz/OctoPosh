using System;
using Octoposh.Model;
using System.Collections.Generic;
using System.Management.Automation;

namespace Octoposh.Cmdlets
{
    /// <summary>
    /// <para type="synopsis">This cmdlet returns the full path of the current Octo.exe version set as default. To learn how to set this path run Get-Help Set-OctopusToolPath</para>
    /// <para type="synopsis">In the background [Get-OctopusToolPath] and [Set-OctopusToolPath] simply Get/Set the value of the environment variable $env:OctoExe</para>
    /// </summary>
    /// <summary>
    /// <para type="description">This cmdlet returns the full path of the current Octo.exe version set as default. To learn how to set this path run Get-Help Set-OctopusToolPath</para>
    /// <para type="description">In the background [Get-OctopusToolPath] and [Set-OctopusToolPath] simply Get/Set the value of the environment variable $env:OctoExe</para>
    /// </summary>
    /// <example>   
    ///   <code>PS C:\> Get-OctopusToolPath</code>
    ///   <para>returns the full path of the current Octo.exe version set as default.</para>
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
            WriteObject(octoTools.GetToolPath(),true);
        }
    }
}