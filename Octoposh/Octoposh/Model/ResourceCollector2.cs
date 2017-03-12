using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Octopus.Client.Model;

namespace Octoposh.Model
{
    internal class ResourceCollector2
    {
        public List<EnvironmentResource> Environments = new List<EnvironmentResource>();
        public List<LifecycleResource> Lifecycles = new List<LifecycleResource>();
        public List<TenantResource> Tenants = new List<TenantResource>();
        public List<ProjectResource> Projects = new List<ProjectResource>();
        private OctopusConnection _connection;


        public ResourceCollector2(OctopusConnection octopusConnection)
        {
            _connection = octopusConnection;
        }

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

        private void AddResourceToCollection(string resourceId)
        {
            switch (GetResourceTypebyId(resourceId))
            {
                case "Environments":
                    Environments.Add(_connection.Repository.Environments.Get(resourceId));
                    break;
                case "Lifecycles":
                    Lifecycles.Add(_connection.Repository.Lifecycles.Get(resourceId));
                    break;
                case "Tenants":
                    Tenants.Add(_connection.Repository.Tenants.Get(resourceId));
                    break;
                case "Projects":
                    Projects.Add(_connection.Repository.Projects.Get(resourceId));
                    break;
                default:
                    Console.WriteLine("whatever");
                    break;
            }
        }

        private Resource GetResourceFromCollection(string resourceId)
        {
            switch (GetResourceTypebyId(resourceId))
            {
                case "Environments":
                    return Environments.First(x => x.Id == resourceId);
                case "Lifecycles":
                    return Lifecycles.First(x => x.Id == resourceId);
                case "Tenants":
                    return Tenants.First(x => x.Id == resourceId);
                case "Projects":
                    return Projects.First(x => x.Id == resourceId);
                default:
                    return null;
            }
        }

        public Resource Fetch(string resourceId)
        {
            if (IsResourceHereAlready(resourceId))
            {
                return GetResourceFromCollection(resourceId);
            }
            else
            {
                try
                {
                    AddResourceToCollection(resourceId);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }

                return GetResourceFromCollection(resourceId);
            }
        }

        private string GetResourceTypebyId(string Id)
        {
            return Id.Split('-')[0];
        }

    }
}
