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

        public bool IsResourceHereAlready(string resourceId, string type)
        {
            switch (type)
            {
                case "Environment":
                    if (Environments.Count != 0)
                    {
                        return Environments.Exists(x => x.Id == resourceId);
                    }
                    else
                    {
                        return false;
                    }
                    
                default:
                    return false;
            }
        }
    }
}
