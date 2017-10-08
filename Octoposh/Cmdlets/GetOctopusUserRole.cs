using Octoposh.Model;
using Octopus.Client.Model;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Octoposh.Infrastructure;

namespace Octoposh.Cmdlets
{
    /// <summary>
    /// <para type="synopsis">This cmdlet returns information about Octopus UserRoles</para>
    /// </summary>
    /// <summary>
    /// <para type="description">This cmdlet returns information about Octopus UserRoles</para>
    /// </summary>
    /// <example>   
    ///   <code>PS C:\> Get-OctopusUserRole</code>
    ///   <para>Get all the UserRoles of the current Instance</para>    
    /// </example>
    /// <example>   
    ///   <code>PS C:\> Get-OctopusUserRole -name "App1_ReleaseCreators"</code>
    ///   <para>Get the User Role named "App1_ReleaseCreators"</para>    
    /// </example>
    /// <example>   
    ///   <code>PS C:\> Get-OctopusUserRole -name "App1_Deployers","App2_Deployers"</code>
    ///   <para>Gets the User Roles with the names "App1_Deployers" and "App2_Deployers"</para>    
    /// </example>
    /// <example>   
    ///   <code>PS C:\> Get-OctopusUserRole -name "*_Administrators"</code>
    ///   <para> Gets all the User Roles whose name ends with "_Administrators*"</para>    
    /// </example>
    /// <para type="link" uri="http://Octoposh.net">WebSite: </para>
    /// <para type="link" uri="https://github.com/Dalmirog/OctoPosh/">Github Project: </para>
    /// <para type="link" uri="http://octoposh.readthedocs.io">Wiki: </para>
    /// <para type="link" uri="https://gitter.im/Dalmirog/OctoPosh#initial">QA and Feature requests: </para>
    [Cmdlet("Get", "OctopusUserRole")]
    [OutputType(typeof(List<UserRoleResource>))]
    public class GetOctopusUserRole : GetOctoposhCmdlet
    {
        /// <summary>
        /// <para type="description">User role name</para>
        /// </summary>
        [Alias("Name")]
        [ValidateNotNullOrEmpty()]
        [Parameter(Position = 1, ValueFromPipeline = true)]
        public string[] UserRoleName { get; set; }

        protected override void ProcessRecord()
        {
            var userRoleNameList = UserRoleName?.ToList().ConvertAll(s => s.ToLower());

            var baseResourceList = userRoleNameList == null ? Connection.Repository.UserRoles.FindAll() : FilterByName(userRoleNameList, Connection.Repository.UserRoles, "UserRoleName");

            if (baseResourceList.Count == 1)
            {
                WriteObject(baseResourceList.FirstOrDefault(), true);
            }
            else
            {
                WriteObject(baseResourceList, true);
            }
        }
    }
}
