using System.Collections.Generic;
using Octopus.Client.Model;

namespace Octoposh.Model
{
    public class OutputOctopusTagSet
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public string Description { get; set; }
        public int SortOrder { get; set; }
        public IList<TagResource> Tags { get; set; }
        public TagSetResource Resource { get; set; }
    }
}