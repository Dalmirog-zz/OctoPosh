using System.Collections.Generic;
using Octoposh.TestDataGenerator.Model;
using Octopus.Client.Model;

namespace Octoposh.TestDataGenerator
{
    /// <summary>
    /// Static class that exposes sets/lists of releases that will be used by the DeploymentFixture
    /// </summary>
    public static class Releaseplaybooks
    {
        public static readonly List<Release> DeploymentTestsPlaybook = new List<Release>()
        {
            new Release()
            {
                ReleaseVersion = "1.0.0",
                Deployments = new List<Deployment>()
                {
                    new Deployment(){DeploymentState = TaskState.Success , EnvironmentName = "Dev" }
                }
            },
            new Release()
            {
                ReleaseVersion = "2.0.0",
                Deployments = new List<Deployment>()
                {
                    new Deployment(){DeploymentState = TaskState.Success , EnvironmentName = "Dev" }
                }
            },
            new Release()
            {
                ReleaseVersion = "3.0.0",
                Deployments = new List<Deployment>()
                {
                    new Deployment(){DeploymentState = TaskState.Success , EnvironmentName = "Dev" }
                }
            },
            new Release()
            {
                ReleaseVersion = "4.0.0",
                Deployments = new List<Deployment>()
                {
                    new Deployment(){DeploymentState = TaskState.Success , EnvironmentName = "Dev" },
                    new Deployment(){DeploymentState = TaskState.Success, EnvironmentName = "Stage" },
                    new Deployment(){DeploymentState = TaskState.Success, EnvironmentName = "Prod" },
                }
            }
        };
        public static readonly List<Release> DashboardTestsPlaybook = new List<Release>()
        {
            new Release()
            {
                ReleaseVersion = "1.0.0",
                Deployments = new List<Deployment>()
                {
                    new Deployment(){DeploymentState = TaskState.Success , EnvironmentName = "Dev" },
                    new Deployment(){DeploymentState = TaskState.Success, EnvironmentName = "Stage" },
                    new Deployment(){DeploymentState = TaskState.Success, EnvironmentName = "Prod" },
                }
            },
            new Release()
            {
                ReleaseVersion = "2.0.0",
                Deployments = new List<Deployment>()
                {
                    new Deployment(){DeploymentState = TaskState.Success, EnvironmentName = "Dev" },
                    new Deployment(){DeploymentState = TaskState.Canceled, EnvironmentName = "Stage" }
                }
            },
            new Release()
            {
                ReleaseVersion = "3.0.0",
                Deployments = new List<Deployment>()
                {
                    new Deployment(){DeploymentState = TaskState.Failed, EnvironmentName = "Dev" }
                }
            }

        };
        public static readonly List<Release> DashboardTestsPlaybookShort = new List<Release>()
        {
            new Release()
            {
                ReleaseVersion = "1.0.0",
                Deployments = new List<Deployment>()
                {
                    new Deployment(){DeploymentState = TaskState.Success , EnvironmentName = "Dev" }
                }
            }
        };
        public static readonly List<Release> ReleaseTestsPlaybook = GetListOfEmptyReleases(31);
        public static readonly List<Release> ReleaseTestsPlaybookShort = GetListOfEmptyReleases(3);

        /// <summary>
        /// Returns a list of releases without deployments assigned to them. This is mostly used by the Get-OctopusRelease tests
        /// </summary>
        /// <param name="amountOfReleases"></param>
        /// <returns></returns>
        private static List<Release> GetListOfEmptyReleases(int amountOfReleases)
        {
            var list = new List<Release>();

            for (var i = 1; i < amountOfReleases+1; i++)
            {
                list.Add(new Release()
                {
                    ReleaseVersion = $"{i}.0.0"
                });
            }

            return list;
        }
    }
}
