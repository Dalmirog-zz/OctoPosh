using System;
using System.Collections.Generic;
using System.Linq;
using Octopus.Client;
using Octopus.Client.Model;
using Serilog;

namespace Octoposh.TestDataGenerator.Fixtures
{
    public class DeploymentFixture
    {
        private static IOctopusAsyncRepository _repository;

        private static readonly string[] ProjectNames = {"DashboardTests_Project1","DashboardTests_Project2", "DeploymentTests_Project1", "ReleaseTests_Project1" };

        private static List<ProjectResource> _allProjects;
        private static List<ReleaseResource> _allReleases;
        private static List<EnvironmentResource> _allEnvironments;

        public static void Run()
        {
            _repository = OctopusRepository.GetRepository().Result;

            Log.Logger.Information("**Running Deployment Fixture**");

            //Getting all projects from ProjectNames
            _allProjects = _repository.Projects.FindMany(p => ProjectNames.Contains(p.Name)).Result;

            //Getting all releases for projects in _allProjects
            _allReleases = _repository.Releases.FindMany(r => _allProjects.Select(p => p.Id).ToList().Contains(r.ProjectId)).Result;

            //Getting all environments
            _allEnvironments = _repository.Environments.FindAll().Result;

            //Create and deploy releases
            CreateAndDeployReleases();

            Log.Logger.Information("**Finished running Deployment Fixture**");
        }

        private class ReleasePlaybook
        {
            public List<Release> ReleaseList { get; set; }
            public ProjectResource Project { get; set; }
        }

        public class Release
        {
            public string ReleaseVersion { get; set; }

            public List<Deployment> Deployments { get; set; }

            public ReleaseResource ReleaseResource { get; set; }
        }
        
        private static void CreateAndDeployReleases()
        {
            foreach (var project in _allProjects)
            {
                DeleteAllReleasesFromProject(project);

                var playbook = new ReleasePlaybook
                {
                    Project = project,
                    ReleaseList = GetReleasePlaybook(project.Name)
                };

                RunPlaybook(playbook);
            }
        }

        /// <summary>
        /// Returns a playbook from ReleasePlaybooks based on the project name
        /// </summary>
        /// <param name="projectName">Name of the project</param>
        /// <returns></returns>
        private static List<Release> GetReleasePlaybook(string projectName)
        {
            switch (projectName)
            {
                case "DeploymentTests_Project1":
                    return Releaseplaybooks.DeploymentTestsPlaybook;
                case "DashboardTests_Project1":
                    return Releaseplaybooks.DashboardTestsPlaybook;
                case "DashboardTests_Project2":
                    return Releaseplaybooks.DashboardTestsPlaybookShort;
                case "ReleaseTests_Project1":
                    return Releaseplaybooks.ReleaseTestsPlaybook;
                default:
                    throw new Exception($"Project {projectName}");
            }
        }

        private static void DeleteAllReleasesFromProject(ProjectResource project)
        {
            var allProjectReleases = _allReleases.Where(r => r.ProjectId == project.Id).ToList();

            if (allProjectReleases.Count != 0)
            {
                Log.Logger.Information($"Found [{allProjectReleases.Count}] releases for project [{project.Name}]. Deleting all of them");

                foreach (var release in allProjectReleases)
                {
                    _repository.Releases.Delete(release).Wait();
                }
            }
        }

        private static void RunPlaybook(ReleasePlaybook playbook)
        {
            foreach (var release in playbook.ReleaseList){

                var rl = CreateRelease(playbook.Project,release);

                if (release.Deployments != null)
                {
                    release.ReleaseResource = rl;

                    DeployRelease(playbook.Project, release);
                }
                else
                {
                    Log.Logger.Information($"\tNo deployments were declared for this release");
                }
            }
        }

        private static ReleaseResource CreateRelease(ProjectResource project, Release release)
        {
            Log.Logger.Information($"Creating release [{release.ReleaseVersion}] for project [{project.Name}]");
            try
            {
                return _repository.Releases.Create(new ReleaseResource()
                {
                    ProjectId = project.Id,
                    Version = release.ReleaseVersion,
                }).Result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static void DeployRelease(ProjectResource project, Release release)
        {
            foreach (var deployment in release.Deployments)
            {
                var deploymentResource = new DeploymentResource()
                {
                    DeploymentProcessId = project.DeploymentProcessId,
                    EnvironmentId = GetEnvironmentId(deployment.EnvironmentName),
                    ReleaseId = release.ReleaseResource.Id,
                    ChannelId = release.ReleaseResource.ChannelId,
                    ProjectId = release.ReleaseResource.ProjectId,
                };

                Log.Logger.Information($"Deploying [{project.Name}]->[{release.ReleaseResource.Version}]->[{deployment.EnvironmentName}]");

                var dp = _repository.Deployments.Create(deploymentResource).Result;

                TaskResource task;

                do
                {
                    task = _repository.Tasks.Get(dp.TaskId).Result;
                } while (task.IsCompleted != true);
              
                SetDeploymentTaskState(task, deployment.DeploymentState);
                
            }
        }

        private static string GetEnvironmentId(string deploymentEnvironmentName)
        {
            var id = _allEnvironments.FirstOrDefault(e => e.Name == deploymentEnvironmentName)?.Id;

            if (id != null)
            {
                return id;
            }

            throw new Exception($"No environment found with name {deploymentEnvironmentName}");
        }

        private static void SetDeploymentTaskState(TaskResource deploymentTask, TaskState deploymentState)
        {
            Log.Logger.Information($"\tSetting deployment status to [{deploymentState.ToString()}]");
            if(deploymentTask.State != deploymentState) { 
                _repository.Tasks.ModifyState(deploymentTask, deploymentState, GeneralResourceProperty.TaskStateModification).Wait();
            }
        }
    }
}



