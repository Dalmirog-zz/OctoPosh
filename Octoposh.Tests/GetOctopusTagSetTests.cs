using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using NUnit.Framework;
using Octoposh.Cmdlets;
using Octoposh.Model;
using Octopus.Client.Model;

namespace Octoposh.Tests
{
    [TestFixture]
    public class GetOctopusTagSetTests
    {
        private static readonly string CmdletName = "Get-OctopusTagSet";
        private static readonly Type CmdletType = typeof(GetOctopusTagSet);
        private static readonly string TagSet1 = "TagSetTests_TagSet1";
        private static readonly string TagSet2 = "TagSetTests_TagSet2";

        [TestCase("TagSetTests_TagSet1")]
        [TestCase("TagSetTests_TagSet2")]
        public void GetTagSetBySingleName(string tagSetName)
        {
            var parameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "TagSetName",
                    SingleValue = tagSetName
                }
            };
            
            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<OutputOctopusTagSet>();

            Assert.AreEqual(1, results.Count);
            Assert.AreEqual(results[0].Name, tagSetName);
        }

        [TestCase(new[] {"TagSetTests_TagSet1","TagSetTests_TagSet2"}, null)]
        [TestCase(new[] { "TagSetTests_TagSet3","TagSetTests_TagSet4"}, null)]
        public void GetTagSetByMultipleNames(string[] tagsetNames, string unused)
        {
            var parameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "TagSetName",
                    MultipleValue = tagsetNames
                }
            };

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<OutputOctopusTagSet>();

            Assert.AreEqual(2,results.Count);

            var tagSetNamesInResults = results.Select(t => t.Name).ToList();

            foreach (var tagsetName in tagsetNames)
            {
                Assert.Contains(tagsetName,tagSetNamesInResults);
            }
        }

        [Test]
        public void GetTagSetByNameUsingWildcard()
        {
            var namePattern = "TagSetTests_*";

            var parameters = new List<CmdletParameter> {
                new CmdletParameter(){
                    Name = "Name", SingleValue = namePattern
                }
            };

            Console.WriteLine("Looking for resources with name pattern: {0}", namePattern);

            var pattern = new WildcardPattern(namePattern);

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<OutputOctopusTagSet>();

            Assert.AreEqual(4, results.Count);
            Console.WriteLine("Resources found: {0}", results.Count);

            foreach (var item in results)
            {
                Console.WriteLine("Resource name: {0}", item.Name);
                Assert.IsTrue(pattern.IsMatch(item.Name));
            }
            Console.WriteLine("Resources found match pattern [{0}]", namePattern);
        }
        [Test]
        public void DontGetTagSetIfNameDoesntMatch()
        {
            var resourceName = "TotallyANameThatYoullNeverPutToAResource";

            var parameters = new List<CmdletParameter> {new CmdletParameter()
            {
                Name = "Name", SingleValue = resourceName
            }};

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<OutputOctopusTagSet>();

            Assert.AreEqual(results.Count, 0);
            Console.WriteLine("No resources found with name [{0}]", resourceName);
        }
        [Test]
        public void GetTagSetUsingResourceOnlyReturnsRawResource()
        {
            var tagSetName = TagSet1;

            var parameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "resourceOnly"
                },
                new CmdletParameter()
                {
                    Name = "TagSetName",
                    SingleValue = tagSetName
                }
            };

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);

            var results = powershell.Invoke<List<TagSetResource>>();

            //If [results] has at least one item, It'll be of the resource type declared on the powershell.Invoke line, meaning the test passed
            Assert.Greater(results[0].Count, 0);
            ;
        }

    }
}
