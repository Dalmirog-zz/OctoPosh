using System;
using System.Collections.Generic;
using Octopus.Client.Model;

namespace Octoposh.Model
{
    public class OutputOctopusVariableSet
    {
        public string ProjectName { get; set; }
        public string LibraryVariableSetName { get; set; }
        public string ID { get; set; }
        public List<FriendlyVariable> Variables { get; set; }
        public List<string> Usage { get; set; }
        public DateTime LastModifiedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public VariableSetResource Resource { get; set; }

    }
}