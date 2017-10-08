using Octoposh.Model;
using Octopus.Client.Model;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Runtime.InteropServices;
using Octoposh.Infrastructure;

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
    /// <para type="link" uri="http://octoposh.readthedocs.io">Wiki: </para>
    /// <para type="link" uri="https://gitter.im/Dalmirog/OctoPosh#initial">QA and Feature requests: </para>
    [Cmdlet("Get", "OctopusChannel")]
    [OutputType(typeof(List<OutputOctopusChannel>))]
    [OutputType(typeof(List<ChannelResource>))]
    public class GetOctopusChannel : GetOctoposhCmdlet
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

        protected override void ProcessRecord()
        {
            var baseResourceList = new List<ChannelResource>();

            var channelNameList = ChannelName?.ToList().ConvertAll(s => s.ToLower());
            var projectNameList = ProjectName?.ToList().ConvertAll(s => s.ToLower());

            if (projectNameList == null)
            {
                var allProjects = Connection.Repository.Projects.FindAll();

                if (channelNameList == null)
                {
                    allProjects.ForEach(p => baseResourceList.AddRange(Connection.Repository.Projects.GetChannels(p).Items.ToList()));
                }
                else
                {
                    allProjects.ForEach(p => baseResourceList.AddRange(Connection.Repository.Projects.GetChannels(p).Items.Where(c => channelNameList.Contains(c.Name.ToLower())).ToList()));
                }
            }
            else
            {
                var projects = new List<ProjectResource>();

                foreach (var name in projectNameList)
                {
                    var project = Connection.Repository.Projects.FindByName(name);

                    if (project == null)
                    {
                        throw OctoposhExceptions.ResourceNotFound(name, "Project");
                    }

                    projects.Add(project);

                }

                foreach (var project in projects)
                {
                    if (channelNameList == null)
                    {
                        baseResourceList = Connection.Repository.Projects.GetChannels(project).Items.ToList();
                    }

                    else
                    {
                        //todo Ask Shannon how to work around this
                        //baseResourceList = FilterByName<ChannelResource>(channelNameList, Connection.Repository.Channels, "Channel");

                        //Multiple values but one of them is wildcarded, which is not an accepted scenario (I.e -MachineName WebServer*, Database1)
                        if (channelNameList.Any(item => WildcardPattern.ContainsWildcardCharacters(item) && channelNameList.Count > 1))
                        {
                            throw OctoposhExceptions.ParameterCollectionHasRegularAndWildcardItem("ChannelName");
                        }
                        //Only 1 wildcarded value (ie -MachineName WebServer*)
                        else if (channelNameList.Any(item => WildcardPattern.ContainsWildcardCharacters(item) && channelNameList.Count == 1))
                        {
                            var pattern = new WildcardPattern(channelNameList.First());
                            baseResourceList.AddRange(Connection.Repository.Projects.GetChannels(project).Items.Where(t => pattern.IsMatch(t.Name.ToLower())).ToList());
                        }
                        //multiple non-wildcared values (i.e. -MachineName WebServer1,Database1)
                        else if (!channelNameList.Any(WildcardPattern.ContainsWildcardCharacters))
                        {
                            baseResourceList.AddRange(Connection.Repository.Projects.GetChannels(project).Items.Where(t => channelNameList.Contains(t.Name.ToLower())).ToList());
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
                    WriteObject(baseResourceList,true);
                }
            }

            else
            {
                var converter = new OutputConverter();

                //todo Ask shannon how to make this more generic
                var outputList = converter.GetOctopusChannel(baseResourceList);

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