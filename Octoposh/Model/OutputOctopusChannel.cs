using System.Collections.Generic;
using Octopus.Client.Model;

namespace Octoposh.Model
{
    public class OutputOctopusChannel
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public string Description { get; set; }
        public string ProjectName { get; set; }
        public string LifecycleName { get; set; }
        public bool IsDefault { get; set; }
        public ReferenceCollection TenantTags { get; set; }
        public List<ChannelVersionRuleResource> Rules { get; set; }
        public ChannelResource Resource { get; set; }
    }
}