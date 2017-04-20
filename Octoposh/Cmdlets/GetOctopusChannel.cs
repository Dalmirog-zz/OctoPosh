using Octoposh.Model;
using Octopus.Client.Model;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Runtime.InteropServices;

namespace Octoposh.Cmdlets
{
    /// <summary>
    /// <para type="Description">Gets information about Octopus Channels</para>
    /// </summary>
    /// <summary>
    /// <para type="synopsis">Gets information about Octopus Channels</para>
    /// </summary>
    /// <example>   
    ///   <code>PS C:\> Get-OctopusChannel</code>
    ///   <para>Gets all the channels in all the projects of the instance</para>    
    /// </example>
    /// <example>   
    ///   <code>PS C:\> Get-OctopusChannel -Project "MyFinantialApp"</code>
    ///   <para>Gets all the channels of the project "MyFinantialApp"</para>    
    /// </example>
    /// <example>   
    ///   <code>PS C:\> Get-OctopusChannel -name "Hotfix_Website" -Project "MyFinantialApp"</code>
    ///   <para>Gets the Channel with the name "Hotfix_Website" of the project "MyFinantialApp"</para>
    /// </example>
    /// <example>   
    ///   <code>PS C:\> Get-OctopusChannel -name "Hotfix_Website","Hotfix_WebService" -Project "MyFinantialApp"</code>
    ///   <para>Gets the Channels with the names "Hotfix_Website" and "Hotfix_WebService" of the project "MyFinantialApp"</para>
    /// </example>
    /// <example>   
    ///   <code>PS C:\> Get-OctopusChannel -name "Hotfix_*" -Project "MyFinantialApp"</code>
    ///   <para>Gets all the Channels whose name starts with "Hotfix_" of the project "MyFinantialApp"</para>    
    /// </example>
    /// <para type="link" uri="http://Octoposh.net">WebSite: </para>
    /// <para type="link" uri="https://github.com/Dalmirog/OctoPosh/">Github Project: </para>
    /// <para type="link" uri="https://github.com/Dalmirog/OctoPosh/wiki">Wiki: </para>
    /// <para type="link" uri="https://gitter.im/Dalmirog/OctoPosh#initial">QA and Feature requests: </para>
    [Cmdlet("Get", "OctopusChannel")]
    [OutputType(typeof(List<OutputOctopusChannel>))]
    [OutputType(typeof(List<ChannelResource>))]
    public class GetOctopusChannel : PSCmdlet
    {
        /// <summary>
        /// <para type="description">Channel name</para>
        /// </summary>
        [Alias("Name")]
        [ValidateNotNullOrEmpty()]
        [Parameter(Position = 1, ValueFromPipeline = true)]
        public string[] ChannelName { get; set; }

        /// <summary>
        /// <para type="description">Project name</para>
        /// </summary>
        [Alias("Project")]
        [ValidateNotNullOrEmpty()]
        [Parameter(Position = 1, ValueFromPipeline = true)]
        public string[] ProjectName { get; set; }

        /// <summary>
        /// <para type="description">If set to TRUE the cmdlet will return the basic Octopur resource. If not set or set to FALSE, the cmdlet will return a human friendly Octoposh output object</para>
        /// </summary>
        [Parameter]
        public SwitchParameter ResourceOnly { get; set; }

        private OctopusConnection _connection;
        private List<string> _channelNameList;
        private List<string> _projectNameList;


        protected override void BeginProcessing()
        {
            _connection = new NewOctopusConnection().Invoke<OctopusConnection>().ToList()[0];
            _channelNameList = ChannelName?.ToList().ConvertAll(s => s.ToLower());
            _projectNameList = ProjectName?.ToList().ConvertAll(s => s.ToLower());
        }

        protected override void ProcessRecord()
        {
            var baseResourceList = new List<ChannelResource>();
            if (_projectNameList == null)
            {
                var allProjects = _connection.Repository.Projects.FindAll();

                if (_channelNameList == null)
                {
                    allProjects.ForEach(p => baseResourceList.AddRange(_connection.Repository.Projects.GetChannels(p).Items.ToList()));
                }
                else
                {
                    allProjects.ForEach(p => baseResourceList.AddRange(_connection.Repository.Projects.GetChannels(p).Items.Where(c => _channelNameList.Contains(c.Name.ToLower())).ToList()));
                }
            }
            else
            {
                var projects = new List<ProjectResource>();

                foreach (var name in _projectNameList)
                {
                    var project = _connection.Repository.Projects.FindByName(name);

                    if (project == null)
                    {
                        throw OctoposhExceptions.ResourceNotFound(name, "Project");
                    }

                    projects.Add(project);

                }

                foreach (var project in projects)
                {
                    if (_channelNameList == null)
                    {
                        baseResourceList.AddRange(_connection.Repository.Projects.GetChannels(project).Items.ToList());
                    }

                    else
                    {
                        //Multiple values but one of them is wildcarded, which is not an accepted scenario (I.e -MachineName WebServer*, Database1)
                        if (_channelNameList.Any(item => WildcardPattern.ContainsWildcardCharacters(item) && _channelNameList.Count > 1))
                        {
                            throw OctoposhExceptions.ParameterCollectionHasRegularAndWildcardItem("ChannelName");
                        }
                        //Only 1 wildcarded value (ie -MachineName WebServer*)
                        else if (_channelNameList.Any(item => WildcardPattern.ContainsWildcardCharacters(item) && _channelNameList.Count == 1))
                        {
                            var pattern = new WildcardPattern(_channelNameList.First());
                            baseResourceList.AddRange(_connection.Repository.Projects.GetChannels(project).Items.Where(t => pattern.IsMatch(t.Name.ToLower())).ToList());
                        }
                        //multiple non-wildcared values (i.e. -MachineName WebServer1,Database1)
                        else if (!_channelNameList.Any(WildcardPattern.ContainsWildcardCharacters))
                        {
                            baseResourceList.AddRange(_connection.Repository.Projects.GetChannels(project).Items.Where(t => _channelNameList.Contains(t.Name.ToLower())).ToList());
                        }
                    }
                }
            }

            if (ResourceOnly)
            {
                if (baseResourceList.Count == 1)
                {
                    WriteObject(baseResourceList.FirstOrDefault());
                }
                else
                {
                    WriteObject(baseResourceList);
                }
            }

            else
            {
                var converter = new OutputConverter();
                var outputList = converter.GetOctopusChannel(baseResourceList);

                if (outputList.Count == 1)
                {
                    WriteObject(outputList.FirstOrDefault());
                }
                else
                {
                    WriteObject(outputList);
                }
            }

        }
    }
}