using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using Octoposh.Model;
using Octopus.Client.Model;

namespace Octoposh.Cmdlets
{
    /// <summary>
    /// <para type="synopsis">Gets information about Octopus Environments</para>
    /// </summary>
    /// <summary>
    /// <para type="description">Gets information about Octopus Environments</para>
    /// </summary>
    /// <example>   
    ///   <code>PS C:\> Get-OctopusEnvironment -name Production</code>
    ///   <para>Gets info about the environment "Production"</para>    
    /// </example>
    /// <example>   
    ///   <code>PS C:\> Get-OctopusEnvironment -name "FeatureTest*"</code>
    ///   <para>Gets info about all the environments whose name matches the pattern "FeatureTest*"</para>    
    /// </example>
    /// <para type="link" uri="http://Octoposh.net">WebSite: </para>
    /// <para type="link" uri="https://github.com/Dalmirog/OctoPosh/">Github Project: </para>
    /// <para type="link" uri="https://github.com/Dalmirog/OctoPosh/wiki">Wiki: </para>
    /// <para type="link" uri="https://gitter.im/Dalmirog/OctoPosh#initial">QA and Feature requests: </para>
    [Cmdlet("Get","OctopusEnvironment", DefaultParameterSetName = All)]
    [OutputType(typeof(List<OutputOctopusEnvironment>))]
    [OutputType(typeof(List<EnvironmentResource>))]
    public class GetOctopusEnvironment : PSCmdlet
    {
        private const string ByName = "ByName";
        private const string All = "All";

        /// <summary>
        /// <para type="description">Environment name</para>
        /// </summary>
        [Alias("Name")]
        [ValidateNotNullOrEmpty()]
        [Parameter(Position = 1, ValueFromPipeline = true, ParameterSetName = ByName)]
        public string[] EnvironmentName { get; set; }

        /// <summary>
        /// <para type="description">If set to TRUE the cmdlet will return the basic Octopur resource. If not set or set to FALSE, the cmdlet will return a human friendly Octoposh output object</para>
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
            var baseResourceList = new List<EnvironmentResource>();
            var environmentNameList = EnvironmentName?.ToList().ConvertAll(s => s.ToLower());

            switch (this.ParameterSetName)
            {
                case All:
                    baseResourceList = _connection.Repository.Environments.FindAll();
                    break;

                case ByName:
                    //Multiple values but one of them is wildcarded, which is not an accepted scenario (I.e -MachineName WebServer*, Database1)
                    if (environmentNameList.Any(item => WildcardPattern.ContainsWildcardCharacters(item) && environmentNameList.Count > 1))
                    {
                        throw OctoposhExceptions.ParameterCollectionHasRegularAndWildcardItem("EnvironmentName");
                    }
                    //Only 1 wildcarded value (ie -MachineName WebServer*)
                    else if (environmentNameList.Any(item => WildcardPattern.ContainsWildcardCharacters(item) && environmentNameList.Count == 1))
                    {
                        var pattern = new WildcardPattern(environmentNameList.First().ToLower());
                        baseResourceList = _connection.Repository.Environments.FindMany(x => pattern.IsMatch(x.Name.ToLower()));
                    }
                    //multiple non-wildcared values (i.e. -MachineName WebServer1,Database1)
                    else if (!environmentNameList.Any(WildcardPattern.ContainsWildcardCharacters))
                    {
                        baseResourceList = _connection.Repository.Environments.FindMany(x => environmentNameList.Contains(x.Name.ToLower()));
                    }
                    break;
            }

            if (ResourceOnly)
            {
                if (baseResourceList.Count == 1)
                {
                    WriteObject(baseResourceList.FirstOrDefault(),true);
                }
                else
                {
                    WriteObject(baseResourceList,true);
                }
            }
            else
            {
                var converter = new OutputConverter();
                var outputList = converter.GetOctopusEnvironment(baseResourceList);

                if (outputList.Count == 1)
                {
                    WriteObject(outputList.FirstOrDefault(),true);
                }
                else
                {
                    WriteObject(outputList,true);
                }
            }

        }
    }
}