using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using Octoposh.Infrastructure;
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
    /// <para type="link" uri="http://octoposh.readthedocs.io">Wiki: </para>
    /// <para type="link" uri="https://gitter.im/Dalmirog/OctoPosh#initial">QA and Feature requests: </para>
    [Cmdlet("Get","OctopusEnvironment", DefaultParameterSetName = All)]
    [OutputType(typeof(List<OutputOctopusEnvironment>))]
    [OutputType(typeof(List<EnvironmentResource>))]
    public class GetOctopusEnvironment : GetOctoposhCmdlet
    {
        //todo Ask Shannon  - if these could also be put into GetOctoposhCmdlet
        private const string ByName = "ByName";
        private const string All = "All";

        /// <summary>
        /// <para type="description">Environment name</para>
        /// </summary>
        [Alias("Name")]
        [ValidateNotNullOrEmpty()]
        [Parameter(Position = 1, ValueFromPipeline = true, ParameterSetName = ByName)]
        public string[] EnvironmentName { get; set; }

        protected override void ProcessRecord()
        {
            var baseResourceList = new List<EnvironmentResource>();
            var environmentNameList = EnvironmentName?.ToList().ConvertAll(s => s.ToLower());

            switch (ParameterSetName)
            {
                case All:
                    baseResourceList = Connection.Repository.Environments.FindAll();
                    break;

                case ByName:
                    baseResourceList= FilterByName(environmentNameList,Connection.Repository.Environments,"Environment");
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