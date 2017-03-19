using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Octoposh.Model;
using Octopus.Client.Model;

namespace Octoposh.Cmdlets
{
    /// <summary>
    /// <para type="synopsis">Gets the Octopus Server thumbprint. Admin access in the Octopus instance will be needed for this to work</para>
    /// </summary>
    /// <summary>
    /// <para type="synopsis">Gets the Octopus Server thumbprint. Admin access in the Octopus instance will be needed for this to work</para>
    /// </summary>
    /// <example>
    ///   <code>PS C:\> Get-OctopusServerThumbprint</code>
    ///   <para>Gets the thumbprint of the Octopus Server</para>    
    /// </example>
    /// <para type="link" uri="http://Octoposh.net">WebSite: </para>
    /// <para type="link" uri="https://github.com/Dalmirog/OctoPosh/">Github Project: </para>
    /// <para type="link" uri="https://github.com/Dalmirog/OctoPosh/wiki">Wiki: </para>
    /// <para type="link" uri="https://gitter.im/Dalmirog/OctoPosh#initial">QA and Feature requests: </para>
    [Cmdlet("Get", "OctopusServerThumbprint")]
    [OutputType(typeof(string))]
    public class GetOctopusServerThumbprint: PSCmdlet
    {
        private OctopusConnection _connection;

        protected override void BeginProcessing()
        {
            _connection = new NewOctopusConnection().Invoke<OctopusConnection>().ToList()[0];
        }

        protected override void ProcessRecord()
        {
            string thumbprint;

            try
            {
                thumbprint = _connection.Repository.Certificates.GetOctopusCertificate().Thumbprint;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            WriteObject(thumbprint);
        }
    }
}
