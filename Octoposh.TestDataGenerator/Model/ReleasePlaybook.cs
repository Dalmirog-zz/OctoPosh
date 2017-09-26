using System.Collections.Generic;
using Octopus.Client.Model;

namespace Octoposh.TestDataGenerator.Model
{
    public class ReleasePlaybook
    {
        public List<Release> ReleaseList { get; set; }
        public ProjectResource Project { get; set; }
    }
}