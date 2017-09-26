using Octopus.Client.Model;

namespace Octoposh.TestDataGenerator.Model
{
    public class Deployment
    {
        /// <summary>
        /// Environment that the deployment will be run on
        /// </summary>
        public string EnvironmentName { get; set; }

        /// <summary>
        /// Status of the deployment after it runs. By default the deployment will always succeed, and then its status will be overriden by this value
        /// </summary>
        public TaskState DeploymentState { get; set; }
    }
}
