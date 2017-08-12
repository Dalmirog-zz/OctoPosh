using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Octoposh.Model;
using Octopus.Client.Model;

namespace Octoposh.Cmdlets
{
    /// <summary>
    /// <para type="synopsis">Gets information about Octopus deployments</para>
    /// </summary>
    /// <summary>
    /// <para type="description">Gets information about Octopus deployments</para>
    /// </summary>
    /// <example>   
    ///   <code>PS C:\> Get-OctopusDeployment</code>
    ///   <para> Get all the deployments that were done on the Octopus Instance. You might wanna go grab a coffee after hitting [enter] on this one, its gonna take a while.</para>    
    /// </example>
    /// <example>   
    ///   <code>PS C:\> Get-OctopusDeployment -Project "MyProject" -ReleaseVersion 1.0.0</code>
    ///   <para>Get all the deployments that were done for the release 1.0.0 of the project "MyProject"</para>    
    /// </example>
    /// <example>   
    ///   <code>PS C:\> Get-OctopusDeployment -EnvironmentName "Staging","UAT" -ProjectName "MyService"</code>
    ///   <para>Get all the deployents that were done to the environments Staging and UAT on the project "MyService"</para>    
    /// </example>
    /// <example>   
    ///   <code>PS C:\> Get-OctopusDeployment -project "MyProduct.Webapp","MyProduct.service" -Environment "Production"</code>
    ///   <para>Get all the deployments that were done to the environment "Production"  on the projects "MyProduct.webapp" and "MyProduct.service"</para>    
    /// </example>
    /// <example>   
    ///   <code>PS Get-OctopusDeployment -project "MyProduct.Webapp" -Environment "Production" -After 2/20/2017 -Before 2/21/2017</code>
    ///   <para>Gets all the deployments for the project "MyProduct.WebApp" for the environment "Production" between 2/20/2017 and 2/21/2017</para>    
    /// </example>
    /// <para type="link" uri="http://Octoposh.net">WebSite: </para>
    /// <para type="link" uri="https://github.com/Dalmirog/OctoPosh/">Github Project: </para>
    /// <para type="link" uri="http://octoposh.readthedocs.io">Wiki: </para>
    /// <para type="link" uri="https://gitter.im/Dalmirog/OctoPosh#initial">QA and Feature requests: </para>
    [Cmdlet("Get", "OctopusDeployment", DefaultParameterSetName = ByLatest)]
    [OutputType(typeof(List<OutputOctopusDeployment>))]
    [OutputType(typeof(List<DeploymentResource>))]
    public class GetOctopusDeployment : PSCmdlet
    {
        private const string ByVersion = "ByVersion";
        private const string ByLatest = "ByLatest";

        /// <summary>
        /// <para type="description">Name of the Environment to filter by</para>
        /// </summary>
        [Alias("Environment")]
        [ValidateNotNullOrEmpty()]
        [Parameter(ValueFromPipeline = true)]
        public string[] EnvironmentName;

        /// <summary>
        /// <para type="description">Name of the Project to filter by</para>
        /// </summary>
        [Alias("Project")]
        [ValidateNotNullOrEmpty()]
        [Parameter(ValueFromPipeline = true)]
        public string[] ProjectName;

        /// <summary>
        /// <para type="description">Release version to filter by. The cmdlet will only return deployments that belong to these releases</para>
        /// </summary>
        [Alias("Version", "Release")]
        [ValidateNotNullOrEmpty()]
        [Parameter(ParameterSetName = ByVersion)]
        public string[] ReleaseVersion { get; set; }

        /// <summary>
        /// <para type="description">Gets deployments by latest X releases</para>
        /// </summary>
        [Alias("Latest")]
        [ValidateNotNullOrEmpty()]
        [ValidateRange(1, int.MaxValue)]
        [Parameter(ParameterSetName = ByLatest)]
        public int LatestReleases { get; set; } = 30;

        /// <summary>
        /// <para type="description">Target communication style to filter by</para>
        /// </summary>
        [Parameter]
        public DateTimeOffset Before = DateTimeOffset.MaxValue;

        /// <summary>
        /// <para type="description">Target communication style to filter by</para>
        /// </summary>
        [Parameter]
        public DateTimeOffset After = DateTimeOffset.MinValue;

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
            var baseResourceList = new List<DeploymentResource>();
            var projects = new List<DashboardProjectResource>();
            var environments = new List<DashboardEnvironmentResource>();
            var releases = new List<ReleaseResource>();

            //Using the dashboard for this because it involves only 1 API call VS calling Projects/Environmnets.FindByName() plenty of times.
            var rawDashboard = _connection.Repository.Dashboards.GetDashboard();

            if (ProjectName == null)
            {
                projects.AddRange(rawDashboard.Projects);
            }
            else
            {

                var projectNameList = ProjectName?.ToList().ConvertAll(s => s.ToLower());

                foreach (var name in projectNameList)
                {
                    var project = rawDashboard.Projects.FirstOrDefault(p => p.Name.ToLower() == name);
                    if (project != null)
                    {
                        projects.Add(project);
                    }
                    else
                    {
                        //todo Handle this better
                        var message = string.Format("Project not found: {0}", name);
                        WriteObject(message);
                    };
                }
            }
            
            foreach(var dashboardProject in projects){

                var project = _connection.Repository.Projects.Get(dashboardProject.Id);

                switch (ParameterSetName)
                {
                    case ByVersion:
                        foreach (var version in ReleaseVersion)
                        {
                            try
                            {
                                releases.Add(_connection.Repository.Projects.GetReleaseByVersion(project, version));
                            }
                            catch (Exception e)
                            {
                                WriteError(new ErrorRecord(e, "ResourceNotFound", ErrorCategory.ObjectNotFound, e.Message));
                            }
                        }
                        break;

                    case ByLatest:

                        var projectReleases = new List<ReleaseResource>();

                        if (LatestReleases > 30)
                        {
                            projectReleases = _connection.Repository.Projects.GetAllReleases(project).ToList();
                        }
                        else
                        {
                            projectReleases = _connection.Repository.Projects.GetReleases(project).Items.ToList();
                        }

                        if (projectReleases.Count > LatestReleases)
                        {
                            releases.AddRange(projectReleases.GetRange(0, LatestReleases));
                        }
                        else
                        {
                            releases.AddRange(projectReleases);
                        }

                        break;

                    default:
                        releases.AddRange(_connection.Repository.Projects.GetAllReleases(project).ToList());
                        break;
                }
            }

            if (EnvironmentName == null)
            {
                environments = rawDashboard.Environments;
            }
            else
            {
                var environmentNameList = EnvironmentName?.ToList().ConvertAll(s => s.ToLower());

                foreach (var name in environmentNameList)
                {
                    var environment = rawDashboard.Environments.FirstOrDefault(e => e.Name.ToLower() == name);
                    if (environment != null)
                    {
                        environments.Add(environment);
                    }
                    else
                    {
                        //todo Handle this better
                        var message = $"Environment not found: {name}";
                        WriteObject(message);
                    }
                }
            }

            var envIds = new HashSet<string>(environments.Select(e => e.Id));

            foreach (var release in releases)
            {
                baseResourceList.AddRange(_connection.Repository.Releases.GetDeployments(release).Items.Where(d => (d.Created > After) && (d.Created < Before) && (envIds.Contains(d.EnvironmentId))).ToList());
            }

            ResourceOnly = false;

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
                var outputList = converter.GetOctopusDeployment(baseResourceList,environments,projects,releases);

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