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
        private static readonly string Feed1 = "FeedTests_Feed1";
        private static readonly string Feed2 = "FeedTests_Feed2";

        [Test]
        public void GetFeedBySingleName()
        {
            var feedName = Feed1;

            var parameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "FeedName",
                    SingleValue = feedName
                }
            };

            Console.WriteLine("Looking for Feed [{0}]", feedName);

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<OutputOctopusFeed>();

            Assert.AreEqual(1, results.Count);

            Assert.AreEqual(results[0].Name, feedName);

            Console.WriteLine("Feed [{0}] found", feedName);
        }
        [Test]
        public void GetFeedByMultipleNames()
        {
            var feedName1 = Feed1;
            var feedName2 = Feed2;
            bool isFeed1 = false;
            bool isFeed2 = false;

            var parameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "FeedName",
                    MultipleValue = new string[]{feedName1,feedName2}
                }
            };

            Console.WriteLine("Looking for Feeds [{0}] and [{1}]", feedName1, feedName2);

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<OutputOctopusFeed>();

            Assert.AreEqual(2, results.Count);

            foreach (var item in results)
            {
                if (item.Name == feedName1)
                {
                    isFeed1 = true;
                }
                else if (item.Name == feedName2)
                {
                    isFeed2 = true;
                }
                else
                {
                    Console.WriteLine("Feed found with name that was not expected: [{0}]", item.Name);
                    throw new Exception();
                }
            }

            Assert.IsTrue(isFeed1);
            Assert.IsTrue(isFeed2);

            Console.WriteLine("Feeds [{0}] and [{1}] found", feedName1, feedName2);
        }
        [Test]
        public void GetFeedByNameUsingWildcard()
        {
            var namePattern = "FeedTests_Feed*";

            var parameters = new List<CmdletParameter> {
                new CmdletParameter(){
                    Name = "Name", SingleValue = namePattern
                }
            };

            Console.WriteLine("Looking for resources with name pattern: {0}", namePattern);

            var pattern = new WildcardPattern(namePattern);

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<OutputOctopusFeed>();

            Assert.AreEqual(2, results.Count);
            Console.WriteLine("Resources found: {0}", results.Count);

            foreach (var item in results)
            {
                Console.WriteLine("Resource name: {0}", item.Name);
                Assert.IsTrue(pattern.IsMatch(item.Name));
            }
            Console.WriteLine("Resources found match pattern [{0}]", namePattern);
        }
        [Test]
        public void DontGetFeedIfNameDoesntMatch()
        {
            var resourceName = "TotallyANameThatYoullNeverPutToAResource";

            var parameters = new List<CmdletParameter> {new CmdletParameter()
            {
                Name = "Name", SingleValue = resourceName
            }};

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<OutputOctopusFeed>();

            Assert.AreEqual(results.Count, 0);
            Console.WriteLine("No resources found with name [{0}]", resourceName);
        }
        [Test]
        public void GetFeedUsingResourceOnlyReturnsRawResource()
        {
            var feedName = Feed1;

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

    }
}
