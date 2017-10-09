using Octoposh.Model;
using Octopus.Client.Model;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Octoposh.Infrastructure;

namespace Octoposh.Cmdlets
{
    /// <summary>
    /// <para type="synopsis">Gets information about the external feeds registered in Octopus</para>
    /// </summary>
    /// <summary>
    /// <para type="description">Gets information about the external feeds registered in Octopus</para>
    /// </summary>
    /// <example>   
    ///   <code>PS C:\> Get-OctopusFeed</code>
    ///   <para>Get all the external feeds registered in the current Instance</para>    
    /// </example>
    /// <example>   
    ///   <code>PS C:\> Get-OctopusFeed -FeedName "MyGet"</code>
    ///   <para>Get the External Feed named "MyGet"</para>    
    /// </example>
    /// <example>   
    ///   <code>PS C:\> Get-OctopusFeed -URL "*Mycompany*"</code>
    ///   <para>Get a feed with a the string "MyCompany" inside its URL</para>    
    /// </example>
    /// <para type="link" uri="http://Octoposh.net">WebSite: </para>
    /// <para type="link" uri="https://github.com/Dalmirog/OctoPosh/">Github Project: </para>
    /// <para type="link" uri="http://octoposh.readthedocs.io">Wiki: </para>
    /// <para type="link" uri="https://gitter.im/Dalmirog/OctoPosh#initial">QA and Feature requests: </para>
    [Cmdlet("Get", "OctopusFeed", DefaultParameterSetName = All)]
    [OutputType(typeof(List<OutputOctopusFeed>))]
    [OutputType(typeof(List<FeedResource>))]
    public class GetOctopusFeed : GetOctoposhCmdlet
    {

        private const string ByName = "ByName";
        private const string ByUrl = "ByURL";
        private const string All = "All";

        /// <summary>
        /// <para type="description">Feed name</para>
        /// </summary>
        [Alias("Name")]
        [ValidateNotNullOrEmpty()]
        [Parameter(Position = 1, ValueFromPipeline = true, ParameterSetName = ByName)]
        public string[] FeedName { get; set; }

        /// <summary>
        /// <para type="description">Feed URL/Path </para>
        /// </summary>
        [Alias("URI")]
        [ValidateNotNullOrEmpty()]
        [Parameter(Position = 1, ValueFromPipeline = true, ParameterSetName = ByUrl)]
        public string[] URL { get; set; }

        protected override void ProcessRecord()
        {
            var baseResourceList = new List<FeedResource>();

            var feedNameList = FeedName?.ToList().ConvertAll(s => s.ToLower());
            var urlList = URL?.ToList().ConvertAll(s => s.ToLower());

            switch (ParameterSetName)
            {
                case All:
                    baseResourceList = Connection.Repository.Feeds.FindAll();
                    break;
                case ByName:
                    baseResourceList = FilterByName(feedNameList,Connection.Repository.Feeds,"Feeds");
                    break;
                case ByUrl:
                    //todo Ask Shannon  - if its possible to make a Filter by X property or if I should have another function for FilterByFeedURI. Also this is the only part of the project where I'm filtering by this property
                    //Multiple values but one of them is wildcarded, which is not an accepted scenario (I.e -MachineName WebServer*, Database1)
                    if (urlList.Any(item => WildcardPattern.ContainsWildcardCharacters(item) && urlList.Count > 1))
                    {
                        throw OctoposhExceptions.ParameterCollectionHasRegularAndWildcardItem("URL");
                    }
                    //Only 1 wildcarded value (ie -MachineName WebServer*)
                    else if (urlList.Any(item => WildcardPattern.ContainsWildcardCharacters(item) && urlList.Count == 1))
                    {
                        var pattern = new WildcardPattern(urlList.First());
                        baseResourceList = Connection.Repository.Feeds.FindMany(t => pattern.IsMatch(t.FeedUri.ToLower()));
                    }
                    //multiple non-wildcared values (i.e. -MachineName WebServer1,Database1)
                    else if (!urlList.Any(WildcardPattern.ContainsWildcardCharacters))
                    {
                        baseResourceList = Connection.Repository.Feeds.FindMany(t => urlList.Contains(t.FeedUri.ToLower()));
                    }
                    break;
            }

            if (ResourceOnly)
            {
                if (baseResourceList.Count == 1)
                {
                    WriteObject(baseResourceList.FirstOrDefault(),true);
                }
                else
                {
                    WriteObject(baseResourceList,true);
                }
            }

            else
            {
                var converter = new OutputConverter();
                var outputList = converter.GetOctopusFeed(baseResourceList);

                if (outputList.Count == 1)
                {
                    WriteObject(outputList.FirstOrDefault(),true);
                }
                else
                {
                    WriteObject(outputList,true);
                }
            }

        }
    }
}