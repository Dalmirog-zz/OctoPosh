using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Octopus.Client.Model;

namespace Octoposh.Model
{
    public class OutputOctopusLifecycle
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public string Description { get; set; }
        public IList<PhaseResource> Phases { get; set; }
        public RetentionPeriod ReleaseRetentionPolicy { get; set; }
        public RetentionPeriod TentacleRetentionPolicy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public LifecycleResource Resource { get; set; }
        
    }
}
