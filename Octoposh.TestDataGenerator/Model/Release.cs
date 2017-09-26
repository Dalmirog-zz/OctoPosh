using System;
using System.Collections.Generic;
using System.Text;
using Octopus.Client.Model;

namespace Octoposh.TestDataGenerator.Model
{
    public class Release
    {
        public string ReleaseVersion { get; set; }

        public List<Deployment> Deployments { get; set; }

        public ReleaseResource ReleaseResource { get; set; }
    }
}
