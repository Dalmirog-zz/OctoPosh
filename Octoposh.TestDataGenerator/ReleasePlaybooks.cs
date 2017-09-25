using System;
using System.Collections.Generic;
using System.Text;
using Octoposh.TestDataGenerator.Fixtures;
using Octopus.Client.Model;

namespace Octoposh.TestDataGenerator
{
    public static class Releaseplaybooks
    {
        public static readonly List<DeploymentFixture.Release> DeploymentTestsPlaybook = new List<DeploymentFixture.Release>()
        {
            new DeploymentFixture.Release()
            {
                ReleaseVersion = "1.0.0",
                Deployments = new List<Deployment>()
                {
                    new Deployment(){DeploymentState = TaskState.Success , EnvironmentName = "Dev" },
                    new Deployment(){DeploymentState = TaskState.Success, EnvironmentName = "Stage" },
                    new Deployment(){DeploymentState = TaskState.Success, EnvironmentName = "Prod" },
                }
            },
            new DeploymentFixture.Release()
            {
                ReleaseVersion = "2.0.0",
                Deployments = new List<Deployment>()
                {
                    new Deployment(){DeploymentState = TaskState.Success , EnvironmentName = "Dev" },
                    new Deployment(){DeploymentState = TaskState.Success, EnvironmentName = "Stage" },
                    new Deployment(){DeploymentState = TaskState.Success, EnvironmentName = "Prod" },
                }
            },
            new DeploymentFixture.Release()
            {
                ReleaseVersion = "3.0.0",
                Deployments = new List<Deployment>()
                {
                    new Deployment(){DeploymentState = TaskState.Success , EnvironmentName = "Dev" },
                    new Deployment(){DeploymentState = TaskState.Success, EnvironmentName = "Stage" },
                    new Deployment(){DeploymentState = TaskState.Success, EnvironmentName = "Prod" },
                }
            },
            new DeploymentFixture.Release()
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

        public static readonly List<DeploymentFixture.Release> DashboardTestsPlaybook = new List<DeploymentFixture.Release>()
        {
            new DeploymentFixture.Release()
            {
                ReleaseVersion = "1.0.0",
                Deployments = new List<Deployment>()
                {
                    new Deployment(){DeploymentState = TaskState.Success , EnvironmentName = "Dev" },
                    new Deployment(){DeploymentState = TaskState.Success, EnvironmentName = "Stage" },
                    new Deployment(){DeploymentState = TaskState.Success, EnvironmentName = "Prod" },
                }
            },
            new DeploymentFixture.Release()
            {
                ReleaseVersion = "2.0.0",
                Deployments = new List<Deployment>()
                {
                    new Deployment(){DeploymentState = TaskState.Success, EnvironmentName = "Dev" },
                    new Deployment(){DeploymentState = TaskState.Canceled, EnvironmentName = "Stage" }
                }
            },
            new DeploymentFixture.Release()
            {
                ReleaseVersion = "3.0.0",
                Deployments = new List<Deployment>()
                {
                    new Deployment(){DeploymentState = TaskState.Failed, EnvironmentName = "Dev" }
                }
            }

        };
        public static readonly List<DeploymentFixture.Release> DashboardTestsPlaybookShort = new List<DeploymentFixture.Release>()
        {
            new DeploymentFixture.Release()
            {
                ReleaseVersion = "1.0.0",
                Deployments = new List<Deployment>()
                {
                    new Deployment(){DeploymentState = TaskState.Success , EnvironmentName = "Dev" }
                }
            }
        };

        public static readonly List<DeploymentFixture.Release> ReleaseTestsPlaybook = GetListOfEmptyReleases(31);

        private static List<DeploymentFixture.Release> GetListOfEmptyReleases(int amountOfReleases)
        {
            var list = new List<DeploymentFixture.Release>();

            for (var i = 1; i < amountOfReleases+1; i++)
            {
                list.Add(new DeploymentFixture.Release()
                {
                    ReleaseVersion = $"{i}.0.0"
                });
            }

            return list;
        }

    }

    public class Deployment
    {
        public string EnvironmentName { get; set; }
        public TaskState DeploymentState { get; set; }
    }
}
