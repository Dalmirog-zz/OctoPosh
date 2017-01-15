using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Octopus.Client.Model;

namespace Octoposh.Tests
{
    class CmdletParameter
    {
        public string Name { get; set; }
        public string[] MultipleValue { get; set; }
        public string SingleValue { get; set; }
        public Resource Resource { get; set; }
    }
}
