using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using NUnit.Framework;
using Octoposh.Cmdlets;
using Octoposh.Model;
using Octopus.Client.Model;

namespace Octoposh.Tests
{
    [TestFixture]
    public class GetOctopusFeedTests
    {
        private static readonly string CmdletName = "Get-OctopusFeed";
        private static readonly Type CmdletType = typeof(GetOctopusFeed);

        [TestCase("FeedTests_Feed1")]
        [TestCase("FeedTests_Feed2")]
        public void GetFeedBySingleName(string feedName)
        {
            var parameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "FeedName",
                    SingleValue = feedName
                }
            };

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<OutputOctopusFeed>();

            Assert.AreEqual(1, results.Count);

            Assert.AreEqual(results[0].Name, feedName);
        }

        [TestCase(new[] { "FeedTests_Feed1", "FeedTests_Feed2" }, null)]
        [TestCase(new[] { "FeedTests_Feed3", "FeedTests_Feed4" }, null)]
        public void GetFeedByMultipleNames(string[] feedNames,string unused)
        {
            var parameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "FeedName",
                    MultipleValue = feedNames
                }
            };

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<OutputOctopusFeed>();

            Assert.AreEqual(2, results.Count);
            var feedNamesInResults = results.Select(f => f.Name).ToList();

            foreach (var feedName in feedNames)
            {
                Assert.Contains(feedName,feedNamesInResults);
            }

        }

        [TestCase("*1")]
        [TestCase("*2")]
        [TestCase("*3")]
        public void GetFeedByNameUsingWildcard(string namePattern)
        {
            var parameters = new List<CmdletParameter> {
                new CmdletParameter(){
                    Name = "Name",
                    SingleValue = namePattern
                }
            };

            var pattern = new WildcardPattern(namePattern);

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<OutputOctopusFeed>();

            Assert.AreEqual(1, results.Count);
            
            Assert.IsTrue(pattern.IsMatch(results[0].Name));
        }

        [TestCase("FeedTests_Feed123")]
        [TestCase("FeedTests_Feed234")]
        [TestCase("FeedTests_Feed345")]
        public void DontGetFeedIfNameDoesntMatch(string feedName)
        {
            var parameters = new List<CmdletParameter> {
                new CmdletParameter()
                {
                    Name = "Name",
                    SingleValue = feedName
                }};

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<OutputOctopusFeed>();

            Assert.AreEqual(results.Count, 0);
        }

        [Test]
        public void GetFeedUsingResourceOnlyReturnsRawResource()
        {
            var feedName = "FeedTests_Feed1";

            var parameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "resourceOnly"
                },
                new CmdletParameter()
                {
                    Name = "FeedName",
                    SingleValue = feedName
                }
            };

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);

            var results = powershell.Invoke<List<FeedResource>>();

            //If [results] has at least one item, It'll be of the resource type declared on the powershell.Invoke line, meaning the test passed
            Assert.Greater(results[0].Count, 0);
            ;
        }

        //todo Add tests for GetByURI

    }
}
