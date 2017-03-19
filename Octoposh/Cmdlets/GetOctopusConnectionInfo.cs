using System;
using System.Management.Automation;
using Octoposh.Model;

namespace Octoposh.Cmdlets
{
    /// <summary>
    /// <para type="synopsis">This function gets the data of the variables $env:OctopusURI and $env:OctopusAPI that are used by all the cmdlets of the Octoposh module</para>
    /// </summary>
    /// <summary>
    /// <para type="description">This function gets the data of the variables $env:OctopusURI and $env:OctopusAPI that are used by all the cmdlets of the Octoposh module</para>
    /// </summary>
    /// <example>   
    ///   <code>PS C:\> Get-OctopusConnectionInfo</code>
    ///   <para>Get the current connection info. Its the same as getting the values of $env:OctopusURL and $Env:OctopusAPIKey</para>    
    /// </example>
    /// <para type="link" uri="http://Octoposh.net">WebSite: </para>
    /// <para type="link" uri="https://github.com/Dalmirog/OctoPosh/">Github Project: </para>
    /// <para type="link" uri="https://github.com/Dalmirog/OctoPosh/wiki">Wiki: </para>
    /// <para type="link" uri="https://gitter.im/Dalmirog/OctoPosh#initial">QA and Feature requests: </para>
    [Cmdlet ("Get","OctopusConnectionInfo")]
    [OutputType(typeof(OctopusConnectionInfo))]
    public class GetOctopusConnectionInfo : Cmdlet
    {
        protected override void ProcessRecord()
        {
            WriteObject(new OctopusConnectionInfo
            {
                OctopusUrl = Environment.GetEnvironmentVariable("OctopusURL"),
                OctopusApiKey = Environment.GetEnvironmentVariable("OctopusAPIKey")
            });
        }
    }
}