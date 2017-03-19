using System.Collections.Generic;
using System.Security.AccessControl;

namespace Octoposh.Model
{
    public class FriendlyScopeCollection
    {
        public List<string> Environments { get; set; }
        public List<string> Machines { get; set; }
        public List<string> Channels { get; set; }
        public List<string> Actions { get; set; }
        public List<string> Roles { get; set; }
    }
}