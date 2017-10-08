using Octoposh.Model;
using Octopus.Client.Model;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Octoposh.Infrastructure;

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
    /// <para type="link" uri="http://octoposh.readthedocs.io">Wiki: </para>
    /// <para type="link" uri="https://gitter.im/Dalmirog/OctoPosh#initial">QA and Feature requests: </para>
    [Cmdlet("Get", "OctopusTagSet")]
    [OutputType(typeof(List<OutputOctopusTagSet>))]
    [OutputType(typeof(List<TagSetResource>))]
    public class GetOctopusTagSet : GetOctoposhCmdlet
    {
        /// <summary>
        /// <para type="description">TagSet name</para>
        /// </summary>
        [Alias("Name")]
        [ValidateNotNullOrEmpty()]
        [Parameter(Position = 1, ValueFromPipeline = true)]
        public string[] TagSetName { get; set; }

        protected override void ProcessRecord()
        {
            var tagSetNameList = TagSetName?.ToList().ConvertAll(s => s.ToLower());

            var baseResourceList = new List<TagSetResource>();

            if (tagSetNameList == null)
            {
                baseResourceList.AddRange(Connection.Repository.TagSets.FindAll());
            }

            else
            {
                baseResourceList = FilterByName(tagSetNameList, Connection.Repository.TagSets, "TagSetName");
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