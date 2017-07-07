using Octoposh.Model;
using Octopus.Client.Model;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;

namespace Octoposh.Cmdlets
{
    /// <summary>
    /// <para type="Description">Gets information about Octopus TagSets</para>
    /// </summary>
    /// <summary>
    /// <para type="synopsis">Gets information about Octopus TagSets</para>
    /// </summary>
    /// <example>   
    ///   <code>PS C:\> Get-OctopusTagSet</code>
    ///   <para>Gets all the TagSets of the instance</para>    
    /// </example>
    /// <example>   
    ///   <code>PS C:\> Get-OctopusTagSet -name "Upgrade Ring"</code>
    ///   <para>Gets the TagSet with the name "Upgrade Ring"</para>    
    /// </example>
    /// <example>   
    ///   <code>PS C:\> Get-OctopusTagSet -name "Upgrade Ring","EAP"</code>
    ///   <para>Gets the TagSets with the names "Upgrade Ring" and "EAP"</para>
    /// </example>
    /// <example>   
    ///   <code>PS C:\> Get-OctopustagSet -name "*_Customers"</code>
    ///   <para>Gets all the TagSets whose name matches the pattern "*_Customers"</para>    
    /// </example>
    /// <para type="link" uri="http://Octoposh.net">WebSite: </para>
    /// <para type="link" uri="https://github.com/Dalmirog/OctoPosh/">Github Project: </para>
    /// <para type="link" uri="https://github.com/Dalmirog/OctoPosh/wiki">Wiki: </para>
    /// <para type="link" uri="https://gitter.im/Dalmirog/OctoPosh#initial">QA and Feature requests: </para>
    [Cmdlet("Get", "OctopusTagSet")]
    [OutputType(typeof(List<OutputOctopusTagSet>))]
    [OutputType(typeof(List<TagSetResource>))]
    public class GetOctopusTagSet : PSCmdlet
    {
        /// <summary>
        /// <para type="description">TagSet name</para>
        /// </summary>
        [Alias("Name")]
        [ValidateNotNullOrEmpty()]
        [Parameter(Position = 1, ValueFromPipeline = true)]
        public string[] TagSetName { get; set; }

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
            var TagSetNameList = TagSetName?.ToList().ConvertAll(s => s.ToLower());

            var baseResourceList = new List<TagSetResource>();

            if (TagSetNameList == null)
            {
                baseResourceList.AddRange(_connection.Repository.TagSets.FindAll());
            }

            else
            {
                //Multiple values but one of them is wildcarded, which is not an accepted scenario (I.e -MachineName WebServer*, Database1)
                if (TagSetNameList.Any(item => WildcardPattern.ContainsWildcardCharacters(item) && TagSetNameList.Count > 1))
                {
                    throw OctoposhExceptions.ParameterCollectionHasRegularAndWildcardItem("TagSetName");
                }
                //Only 1 wildcarded value (ie -MachineName WebServer*)
                else if (TagSetNameList.Any(item => WildcardPattern.ContainsWildcardCharacters(item) && TagSetNameList.Count == 1))
                {
                    var pattern = new WildcardPattern(TagSetNameList.First());
                    baseResourceList.AddRange(_connection.Repository.TagSets.FindMany(t => pattern.IsMatch(t.Name.ToLower())));
                }
                //multiple non-wildcared values (i.e. -MachineName WebServer1,Database1)
                else if (!TagSetNameList.Any(WildcardPattern.ContainsWildcardCharacters))
                {
                    baseResourceList.AddRange(_connection.Repository.TagSets.FindMany(t => TagSetNameList.Contains(t.Name.ToLower())));
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
                    WriteObject(baseResourceList,true);
                }
            }

            else
            {
                var converter = new OutputConverter();
                List<OutputOctopusTagSet> outputList = converter.GetOctopusTagSet(baseResourceList);

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