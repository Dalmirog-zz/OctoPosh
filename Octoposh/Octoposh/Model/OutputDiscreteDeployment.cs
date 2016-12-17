using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Octoposh.Model
{
    public class OutputDiscreteDeployment{
        public string ProjectName { get; set; }
        public string EnvironmentName { get; set; }
        public string Releaseversion { get; set; }
        public string State { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? Endtime { get; set; }
    }
}
