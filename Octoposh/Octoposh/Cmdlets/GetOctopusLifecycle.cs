using Octoposh.Model;
using Octopus.Client.Model;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;

namespace Octoposh.Cmdlets
{
    /// <summary>
    /// <para type="synopsis">This cmdlet returns Octopus Lifecycles</para>
    /// </summary>
    /// <summary>
    /// <para type="description">This cmdlet returns Octopus Lifecycles</para>
    /// </summary>
    /// <example>   
    ///   <code>PS C:\> Get-OctopusLifecycle</code>
    ///   <para>Get all the Lifecycles of the current Instance</para>    
    /// </example>
    /// <example>   
    ///   <code>PS C:\> Get-OctopusLifecycle -name MyLifecycle</code>
    ///   <para>Get the Lifecycle named "MyLifecycle"</para>    
    /// </example>
    /// <example>   
    ///   <code>PS C:\> Get-OctopusLifecycle -name "ProjectA_Lifecycle","ProjectB_Lifecycle"</code>
    ///   <para>Gets the teams with the names "ProjectA_Lifecycle" and "ProjectB_Lifecycle"</para>    
    /// </example>
    /// <example>   
    ///   <code>PS C:\> Get-OctopusLifecycle -name "ProjectA*"</code>
    ///   <para> Gets all the lifecycles whose name starts with "ProjectA"</para>    
    /// </example>
    /// <para type="link" uri="http://Octoposh.net">WebSite: </para>
    /// <para type="link" uri="https://github.com/Dalmirog/OctoPosh/">Github Project: </para>
    /// <para type="link" uri="https://github.com/Dalmirog/OctoPosh/wiki">Wiki: </para>
    /// <para type="link" uri="https://gitter.im/Dalmirog/OctoPosh#initial">QA and Feature requests: </para>
    [Cmdlet("Get", "OctopusLifecycle")]
    [OutputType(typeof(List<OutputOctopusLifecycle>))]
    [OutputType(typeof(List<LifecycleResource>))]
    public class GetOctopusLifecycle : PSCmdlet
    {
        /// <summary>
        /// <para type="description">Lifecycle name</para>
        /// </summary>
        [Alias("Name")]
        [ValidateNotNullOrEmpty()]
        [Parameter(Position = 1, ValueFromPipeline = true)]
        public List<string> LifecycleName { get; set; }

        /// <summary>
        /// <para type="description">If set to TRUE the cmdlet will return the basic Octopur resource. If not set or set to FALSE, the cmdlet will return a human friendly Octoposh output object</para>
        /// </summary>
        [Parameter]
        public SwitchParameter ResourceOnly { get; set; }

        private OctopusConnection _connection;

        protected override void BeginProcessing()
        {
            _connection = new NewOctopusConnection().Invoke<OctopusConnection>().ToList()[0];
            LifecycleName = LifecycleName?.ConvertAll(s => s.ToLower());
        }

        protected override void ProcessRecord()
        {
            var baseResourceList = new List<LifecycleResource>();
            if (LifecycleName == null)
            {
                baseResourceList = _connection.Repository.Lifecycles.FindAll();
            }
            else
            {
                //Multiple values but one of them is wildcarded, which is not an accepted scenario (I.e -MachineName WebServer*, Database1)
                if (LifecycleName.Any(item => WildcardPattern.ContainsWildcardCharacters(item) && LifecycleName.Count > 1))
                {
                    throw OctoposhExceptions.ParameterCollectionHasRegularAndWildcardItem("Lifecycle");
                }
                //Only 1 wildcarded value (ie -MachineName WebServer*)
                else if (LifecycleName.Any(item => WildcardPattern.ContainsWildcardCharacters(item) && LifecycleName.Count == 1))
                {
                    var pattern = new WildcardPattern(LifecycleName.First());
                    baseResourceList = _connection.Repository.Lifecycles.FindMany(t => pattern.IsMatch(t.Name.ToLower()));
                }
                //multiple non-wildcared values (i.e. -MachineName WebServer1,Database1)
                else if (!LifecycleName.Any(WildcardPattern.ContainsWildcardCharacters))
                {
                    baseResourceList = _connection.Repository.Lifecycles.FindMany(t => LifecycleName.Contains(t.Name.ToLower()));
                }
            }

            if (ResourceOnly)
            {
                WriteObject(baseResourceList);
            }

            else
            {
                var converter = new OutputConverter();
                var outputList = converter.GetOctopusLifecycle(baseResourceList);

                WriteObject(outputList);
            }

        }
    }
}