using Octoposh.Model;
using Octopus.Client.Model;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;

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
    public class GetOctopusUserRole : PSCmdlet
    {
        /// <summary>
        /// <para type="description">User role name</para>
        /// </summary>
        [Alias("Name")]
        [ValidateNotNullOrEmpty()]
        [Parameter(Position = 1, ValueFromPipeline = true)]
        public string[] UserRoleName { get; set; }


        private OctopusConnection _connection;

        protected override void BeginProcessing()
        {
            _connection = new NewOctopusConnection().Invoke<OctopusConnection>().ToList()[0];
        }

        protected override void ProcessRecord()
        {
            var userRoleNameList = UserRoleName?.ToList().ConvertAll(s => s.ToLower());

            var baseResourceList = new List<UserRoleResource>();
            if (userRoleNameList == null)
            {
                baseResourceList = _connection.Repository.UserRoles.FindAll();
            }
            else
            {
                //Multiple values but one of them is wildcarded, which is not an accepted scenario (I.e -MachineName WebServer*, Database1)
                if (userRoleNameList.Any(item => WildcardPattern.ContainsWildcardCharacters(item) && userRoleNameList.Count > 1))
                {
                    throw OctoposhExceptions.ParameterCollectionHasRegularAndWildcardItem("UserRole");
                }
                //Only 1 wildcarded value (ie -MachineName WebServer*)
                else if (userRoleNameList.Any(item => WildcardPattern.ContainsWildcardCharacters(item) && userRoleNameList.Count == 1))
                {
                    var pattern = new WildcardPattern(userRoleNameList.First());
                    baseResourceList = _connection.Repository.UserRoles.FindMany(t => pattern.IsMatch(t.Name.ToLower()));
                }
                //multiple non-wildcared values (i.e. -MachineName WebServer1,Database1)
                else if (!userRoleNameList.Any(WildcardPattern.ContainsWildcardCharacters))
                {
                    baseResourceList = _connection.Repository.UserRoles.FindMany(t => userRoleNameList.Contains(t.Name.ToLower()));
                }
            }

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
