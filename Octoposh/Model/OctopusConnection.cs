using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Octoposh.Model;
using Octopus.Client;

namespace Octoposh.Model
{
    public class OctopusConnection
    {
        public OctopusRepository Repository;
        public Hashtable Header = new Hashtable();
    }
}
