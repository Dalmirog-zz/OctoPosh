using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Octoposh.Model
{
    internal class VariableSetOwner
    {
        public OwnerType Type { get; set; }
        public string Name { get; set; }
    }

    enum OwnerType
    {
        Project, LibraryVariableSet
    }
}
