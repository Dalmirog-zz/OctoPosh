using System.Collections.Generic;
using Octopus.Client.Model;

namespace Octoposh.Model
{
    public class OutputOctopusUser
    {
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public string EmailAdress { get; set; }
        public bool IsService { get; set; }
        public List<string> Teams { get; set; }
        public UserResource Resource { get; set; }
    }
}