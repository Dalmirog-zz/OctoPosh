using Octoposh.Model;
using Octopus.Client.Model;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;

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
    public class GetOctopusTenant : PSCmdlet
    {
        /// <summary>
        /// <para type="description">Tenant name</para>
        /// </summary>
        [Alias("Name")]
        [ValidateNotNullOrEmpty()]
        [Parameter(Position = 1, ValueFromPipeline = true)]
        public string[] TenantName { get; set; }

        /// <summary>
        /// <para type="description">If set to TRUE the cmdlet will return the basic Octopus resource. If not set or set to FALSE, the cmdlet will return a human friendly Octoposh output object</para>
        /// </summary>
        [Parameter]
        public SwitchParameter ResourceOnly { get; set; }

        private OctopusConnection _connection;

        protected override void BeginProcessing()
        {
            _connection = new NewOctopusConnection().Invoke<OctopusConnection>().ToList()[0];
        }

        protected override void ProcessRecord()
        {
            var baseResourceList = new List<TenantResource>();

            var tenantNameList = TenantName?.ToList().ConvertAll(s => s.ToLower());

            if (tenantNameList == null)
            {
                baseResourceList.AddRange(_connection.Repository.Tenants.FindAll());
            }

            else
            {
                //Multiple values but one of them is wildcarded, which is not an accepted scenario (I.e -MachineName WebServer*, Database1)
                if (tenantNameList.Any(item => WildcardPattern.ContainsWildcardCharacters(item) && tenantNameList.Count > 1))
                {
                    throw OctoposhExceptions.ParameterCollectionHasRegularAndWildcardItem("TenantName");
                }
                //Only 1 wildcarded value (ie -MachineName WebServer*)
                else if (tenantNameList.Any(item => WildcardPattern.ContainsWildcardCharacters(item) && tenantNameList.Count == 1))
                {
                    var pattern = new WildcardPattern(tenantNameList.First());
                    baseResourceList.AddRange(_connection.Repository.Tenants.FindMany(t => pattern.IsMatch(t.Name.ToLower())));
                }
                //multiple non-wildcared values (i.e. -MachineName WebServer1,Database1)
                else if (!tenantNameList.Any(WildcardPattern.ContainsWildcardCharacters))
                {
                    baseResourceList.AddRange(_connection.Repository.Tenants.FindMany(t => tenantNameList.Contains(t.Name.ToLower())));
                }
            }

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