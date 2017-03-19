using Octopus.Client.Model;

namespace Octoposh.Model
{
    public class FriendlyVariable
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public bool IsEditable { get; set; }
        public bool IsSensitive { get; set; }
        public FriendlyScopeCollection Scope { get; set; }
        public VariablePromptOptions Prompt { get; set; }
    }
}