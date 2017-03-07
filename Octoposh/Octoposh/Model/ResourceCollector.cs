using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Octopus.Client.Model;

namespace Octoposh.Model
{
    internal class ResourceCollector
    {
        public List<EnvironmentResource> Environments = new List<EnvironmentResource>();
        public List<LifecycleResource> Lifecycles = new List<LifecycleResource>();
        public List<TenantResource> Tenants = new List<TenantResource>();
        public List<ProjectResource> Projects = new List<ProjectResource>();

        public bool IsResourceHereAlready(string resourceId)
        {
            switch (GetResourceTypebyId(resourceId))
            {
                case "Environments":
                    return Environments.Count != 0 && Environments.Exists(x => x.Id == resourceId);
                case "Lifecycles":
                    return Lifecycles.Count != 0 && Lifecycles.Exists(x => x.Id == resourceId);
                case "Tenants":
                    return Tenants.Count != 0 && Tenants.Exists(x => x.Id == resourceId);
                case "Projects":
                    return Projects.Count != 0 && Projects.Exists(x => x.Id == resourceId);
                default:
                    return false;
            }
        }

        private string GetResourceTypebyId(string Id)
        {
            return Id.Split('-')[0];
        }

    }
}
