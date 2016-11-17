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

        public bool IsResourceHereAlready(string resourceId)
        {
            switch (GetResourceTypebyId(resourceId))
            {
                case "Environments":
                    return Environments.Count != 0 && Environments.Exists(x => x.Id == resourceId);
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
