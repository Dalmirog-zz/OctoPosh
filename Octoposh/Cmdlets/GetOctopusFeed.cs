using Octoposh.Model;
using Octopus.Client.Model;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;

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
    /// <para type="link" uri="https://github.com/Dalmirog/OctoPosh/wiki">Wiki: </para>
    /// <para type="link" uri="https://gitter.im/Dalmirog/OctoPosh#initial">QA and Feature requests: </para>
    [Cmdlet("Get", "OctopusFeed", DefaultParameterSetName = All)]
    [OutputType(typeof(List<OutputOctopusFeed>))]
    [OutputType(typeof(List<FeedResource>))]
    public class GetOctopusFeed : PSCmdlet
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

        /// <summary>
        /// <para type="description">If set to TRUE the cmdlet will return the basic Octopur resource. If not set or set to FALSE, the cmdlet will return a human friendly Octoposh output object</para>
        /// </summary>
        [Parameter]
        public SwitchParameter ResourceOnly { get; set; }

        private OctopusConnection _connection;
        private List<string> _feedNameList;
        private List<string> _urlList;

        protected override void BeginProcessing()
        {
            _connection = new NewOctopusConnection().Invoke<OctopusConnection>().ToList()[0];
            _feedNameList = FeedName?.ToList().ConvertAll(s => s.ToLower());
            _urlList = URL?.ToList().ConvertAll(s => s.ToLower());
        }

        protected override void ProcessRecord()
        {
            var baseResourceList = new List<FeedResource>();

            switch (ParameterSetName)
            {
                case All:
                    baseResourceList = _connection.Repository.Feeds.FindAll();
                    break;
                case ByName:
                    //Multiple values but one of them is wildcarded, which is not an accepted scenario (I.e -MachineName WebServer*, Database1)
                    if (_feedNameList.Any(item => WildcardPattern.ContainsWildcardCharacters(item) && _feedNameList.Count > 1))
                    {
                        throw OctoposhExceptions.ParameterCollectionHasRegularAndWildcardItem("FeedName");
                    }
                    //Only 1 wildcarded value (ie -MachineName WebServer*)
                    else if (_feedNameList.Any(item => WildcardPattern.ContainsWildcardCharacters(item) && _feedNameList.Count == 1))
                    {
                        var pattern = new WildcardPattern(_feedNameList.First());
                        baseResourceList = _connection.Repository.Feeds.FindMany(t => pattern.IsMatch(t.Name.ToLower()));
                    }
                    //multiple non-wildcared values (i.e. -MachineName WebServer1,Database1)
                    else if (!_feedNameList.Any(WildcardPattern.ContainsWildcardCharacters))
                    {
                        baseResourceList = _connection.Repository.Feeds.FindMany(t => _feedNameList.Contains(t.Name.ToLower()));
                    }
                    break;
                case ByUrl:
                    //Multiple values but one of them is wildcarded, which is not an accepted scenario (I.e -MachineName WebServer*, Database1)
                    if (_urlList.Any(item => WildcardPattern.ContainsWildcardCharacters(item) && _urlList.Count > 1))
                    {
                        throw OctoposhExceptions.ParameterCollectionHasRegularAndWildcardItem("URL");
                    }
                    //Only 1 wildcarded value (ie -MachineName WebServer*)
                    else if (_urlList.Any(item => WildcardPattern.ContainsWildcardCharacters(item) && _urlList.Count == 1))
                    {
                        var pattern = new WildcardPattern(_urlList.First());
                        baseResourceList = _connection.Repository.Feeds.FindMany(t => pattern.IsMatch(t.FeedUri.ToLower()));
                    }
                    //multiple non-wildcared values (i.e. -MachineName WebServer1,Database1)
                    else if (!_urlList.Any(WildcardPattern.ContainsWildcardCharacters))
                    {
                        baseResourceList = _connection.Repository.Feeds.FindMany(t => _urlList.Contains(t.Name.ToLower()));
                    }
                    
                    break;
            }

            if (ResourceOnly)
            {
                if (baseResourceList.Count == 1)
                {
                    WriteObject(baseResourceList.FirstOrDefault());
                }
                else
                {
                    WriteObject(baseResourceList);
                }
            }

            else
            {
                var converter = new OutputConverter();
                var outputList = converter.GetOctopusFeed(baseResourceList);

                if (outputList.Count == 1)
                {
                    WriteObject(outputList.FirstOrDefault());
                }
                else
                {
                    WriteObject(outputList);
                }
            }

        }
    }
}