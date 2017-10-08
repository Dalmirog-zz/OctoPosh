using Octoposh.Model;
using Octopus.Client.Model;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Octoposh.Infrastructure;

namespace Octoposh.Cmdlets
{
    /// <summary>
    /// <para type="synopsis">This cmdlet returns Octopus Teams</para>
    /// </summary>
    /// <summary>
    /// <para type="description">This cmdlet returns Octopus Teams</para>
    /// </summary>
    /// <example>   
    ///   <code>PS C:\> Get-OctopusTeam</code>
    ///   <para>Gets all the teams on the Octopus instance</para>    
    /// </example>
    /// <example>   
    ///   <code>PS C:\> Get-OctopusTeam -name "ProjectA_Managers"</code>
    ///   <para>Gets the team with the name "ProjectA_Managers"</para>    
    /// </example>
    /// <example>   
    ///   <code>PS C:\> Get-OctopusTeam -name "ProjectA_Managers","ProjectA_Developers"</code>
    ///   <para>Gets the teams with the names "ProjectA_Managers" and "ProjectA_Developers"</para>    
    /// </example>
    /// <example>   
    ///   <code>PS C:\> Get-OctopusTeam -name "ProjectA*"</code>
    ///   <para> Gets all the teams whose name starts with "ProjectA"</para>    
    /// </example>
    /// <para type="link" uri="http://Octoposh.net">WebSite: </para>
    /// <para type="link" uri="https://github.com/Dalmirog/OctoPosh/">Github Project: </para>
    /// <para type="link" uri="http://octoposh.readthedocs.io">Wiki: </para>
    /// <para type="link" uri="https://gitter.im/Dalmirog/OctoPosh#initial">QA and Feature requests: </para>
    [Cmdlet("Get", "OctopusTeam")]
    [OutputType(typeof(List<OutputOctopusTeam>))]
    [OutputType(typeof(List<TeamResource>))]
    public class GetOctopusTeam : GetOctoposhCmdlet
    {
        /// <summary>
        /// <para type="description">Team name</para>
        /// </summary>
        [Alias("Name")]
        [ValidateNotNullOrEmpty()]
        [Parameter(Position = 1, ValueFromPipeline = true)]
        public string[] TeamName { get; set; }

        protected override void ProcessRecord()
        {
            var teamNameList = TeamName?.ToList().ConvertAll(s => s.ToLower());

            var baseResourceList = teamNameList == null ? Connection.Repository.Teams.FindAll() : FilterByName(teamNameList, Connection.Repository.Teams, "TeamName");

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
                var outputList = converter.GetOctopusTeam(baseResourceList);

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