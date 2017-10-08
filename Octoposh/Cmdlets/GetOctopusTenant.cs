using Octoposh.Model;
using Octopus.Client.Model;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Octoposh.Infrastructure;

namespace Octoposh.Cmdlets
{
    /// <summary>
    /// <para type="Description">Gets information about Octopus Tenants</para>
    /// </summary>
    /// <summary>
    /// <para type="synopsis">Gets information about Octopus Tenants</para>
    /// </summary>
    /// <example>   
    ///   <code>PS C:\> Get-OctopusTenant</code>
    ///   <para>Gets all the tenants of the instance</para>    
    /// </example>
    /// <example>   
    ///   <code>PS C:\> Get-OctopusTenant -name "MyAwesomeTenant"</code>
    ///   <para>Gets the tenant with the name "MyAwesometenant"</para>    
    /// </example>
    /// <example>   
    ///   <code>PS C:\> Get-OctopusTenant -name "MyAwesomeTenant","MyOtherAwesomeTenant"</code>
    ///   <para>Gets the tenants with the names "MyAwesomeTenant" and "MyOtherAwesomeTenant"</para>
    /// </example>
    /// <example>   
    ///   <code>PS C:\> Get-Channel -name "*AwesomeTenant"</code>
    ///   <para>Gets all the tenants whose name matches the pattern "*AwesomeTenant"</para>    
    /// </example>
    /// <para type="link" uri="http://Octoposh.net">WebSite: </para>
    /// <para type="link" uri="https://github.com/Dalmirog/OctoPosh/">Github Project: </para>
    /// <para type="link" uri="http://octoposh.readthedocs.io">Wiki: </para>
    /// <para type="link" uri="https://gitter.im/Dalmirog/OctoPosh#initial">QA and Feature requests: </para>
    [Cmdlet("Get", "OctopusTenant")]
    [OutputType(typeof(List<OutputOctopusTenant>))]
    [OutputType(typeof(List<TenantResource>))]
    public class GetOctopusTenant : GetOctoposhCmdlet
    {
        /// <summary>
        /// <para type="description">Tenant name</para>
        /// </summary>
        [Alias("Name")]
        [ValidateNotNullOrEmpty()]
        [Parameter(Position = 1, ValueFromPipeline = true)]
        public string[] TenantName { get; set; }

        protected override void ProcessRecord()
        {
            var tenantNameList = TenantName?.ToList().ConvertAll(s => s.ToLower());

            var baseResourceList = tenantNameList == null ? (Connection.Repository.Tenants.FindAll()) : FilterByName(tenantNameList, Connection.Repository.Tenants, "TenantName");

            if (ResourceOnly)
            {
                if (baseResourceList.Count == 1)
                {
                    WriteObject(baseResourceList.FirstOrDefault(),true);
                }
                else
                {
                    WriteObject(baseResourceList, true);
                }
            }

            else
            {
                var converter = new OutputConverter();
                var outputList = converter.GetOctopusTenant(baseResourceList);

                if (outputList.Count == 1)
                {
                    WriteObject(outputList.FirstOrDefault(), true);
                }
                else
                {
                    WriteObject(outputList, true);
                }
            }

        }
    }
}