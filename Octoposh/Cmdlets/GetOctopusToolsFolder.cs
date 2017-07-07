using System;
using Octoposh.Model;
using System.Collections.Generic;
using System.Management.Automation;

namespace Octoposh.Cmdlets
{
    /// <summary>
    /// <para type="synopsis">This cmdlet gets the path of the "Octopus Tools Folder" that will be used by other Octoposh cmdlets. To learn how to set this path and what its the expected file structure on it, run Get-Help Set-OctopusToolsFolder</para>
    /// <para type="synopsis">In the background [Get-OctopusToolsFolder] and [Set-OctopusToolsFolder] simply Get/Set the value of the environment variable $env:OctopusToolsFolder</para>
    /// </summary>
    /// <summary>
    /// <para type="description">This cmdlet gets the path of the "Octopus Tools Folder" that will be used by other Octoposh cmdlets. To learn how to set this path and what its the expected file structure on it, run Get-Help Set-OctopusToolsFolder</para>
    /// <para type="description">In the background [Get-OctopusToolsFolder] and [Set-OctopusToolsFolder] simply Get/Set the value of the environment variable $env:OctopusToolsFolder</para>
    /// </summary>
    /// <example>   
    ///   <code>PS C:\> Get-OctopusToolsFolder</code>
    ///   <para>Gets the path of the "Octopus Tools Folder"</para>
    /// </example>
    /// <para type="link" uri="http://Octoposh.net">WebSite: </para>
    /// <para type="link" uri="https://github.com/Dalmirog/OctoPosh/">Github Project: </para>
    /// <para type="link" uri="http://octoposh.readthedocs.io">Wiki: </para>
    /// <para type="link" uri="https://gitter.im/Dalmirog/OctoPosh#initial">QA and Feature requests: </para>
    [Cmdlet("Get", "OctopusToolsFolder")]
    [OutputType(typeof(string))]
    public class GetOctopusToolsFolder : PSCmdlet
    {
        protected override void ProcessRecord()
        {
            var octoTools = new OctopusToolsHandler();
            WriteObject(octoTools.GetToolsFolder(),true);
            
        }
    }
}