using Octopus.Client.Model;

namespace Octoposh.Model
{
    public class OutputOctopusFeed
    {
        public string Name { get; set; }
        public string FeedURI { get; set; }
        public string LoginUser { get; set; }
        public string Id { get; set; }
        public FeedType FeedType { get; set; }
        public FeedResource Resource { get; set; }
        
    }
}