using Octoposh.Model;
using Octopus.Client.Model;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;

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
    /// <para type="link" uri="https://github.com/Dalmirog/OctoPosh/wiki">Wiki: </para>
    /// <para type="link" uri="https://gitter.im/Dalmirog/OctoPosh#initial">QA and Feature requests: </para>
    [Cmdlet("Get", "OctopusTeam")]
    [OutputType(typeof(List<OutputOctopusProject>))]
    [OutputType(typeof(List<ProjectResource>))]
    public class GetOctopusTeam : PSCmdlet
    {
        /// <summary>
        /// <para type="description">Team name</para>
        /// </summary>
        [Alias("Name")]
        [ValidateNotNullOrEmpty()]
        [Parameter(Position = 1, ValueFromPipeline = true)]
        public List<string> TeamName { get; set; }

        /// <summary>
        /// <para type="description">If set to TRUE the cmdlet will return the basic Octopur resource. If not set or set to FALSE, the cmdlet will return a human friendly Octoposh output object</para>
        /// </summary>
        [Parameter]
        public SwitchParameter ResourceOnly { get; set; }

        private OctopusConnection _connection;

        protected override void BeginProcessing()
        {
            _connection = new NewOctopusConnection().Invoke<OctopusConnection>().ToList()[0];
            TeamName = TeamName?.ConvertAll(s => s.ToLower());
        }

        protected override void ProcessRecord()
        {
            var baseResourceList = new List<TeamResource>();
            if (TeamName == null)
            {
                baseResourceList = _connection.Repository.Teams.FindAll();
            }
            else { 
                //Multiple values but one of them is wildcarded, which is not an accepted scenario (I.e -MachineName WebServer*, Database1)
                if (TeamName.Any(item => WildcardPattern.ContainsWildcardCharacters(item) && TeamName.Count > 1))
                {
                    throw OctoposhExceptions.ParameterCollectionHasRegularAndWildcardItem("TeamName");
                }
                //Only 1 wildcarded value (ie -MachineName WebServer*)
                else if (TeamName.Any(item => WildcardPattern.ContainsWildcardCharacters(item) && TeamName.Count == 1))
                {
                    var pattern = new WildcardPattern(TeamName.First());
                    baseResourceList = _connection.Repository.Teams.FindMany(t => pattern.IsMatch(t.Name.ToLower()));
                }
                //multiple non-wildcared values (i.e. -MachineName WebServer1,Database1)
                else if (!TeamName.Any(WildcardPattern.ContainsWildcardCharacters))
                {
                    baseResourceList = _connection.Repository.Teams.FindMany(t => TeamName.Contains(t.Name.ToLower()));
                }
            }

            if (ResourceOnly)
            {
                WriteObject(baseResourceList);
            }

            else
            {
                var converter = new OutputConverter();
                var outputList = converter.GetOctopusTeam(baseResourceList);

                WriteObject(outputList);
            }

        }
    }
}