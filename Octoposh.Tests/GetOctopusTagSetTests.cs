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
    public class GetOctopusTagSetTests
    {
        private static readonly string CmdletName = "Get-OctopusTagSet";
        private static readonly Type CmdletType = typeof(GetOctopusTagSet);
        private static readonly string TagSet1 = "TagSetTests_TagSet1";
        private static readonly string TagSet2 = "TagSetTests_TagSet2";

        [Test]
        public void GetTagSetBySingleName()
        {
            var tagSetName = TagSet1;

            var parameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "TagSetName",
                    SingleValue = tagSetName
                }
            };

            Console.WriteLine("Looking for TagSet [{0}]", tagSetName);

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<OutputOctopusTagSet>();

            Assert.AreEqual(1, results.Count);

            Assert.AreEqual(results[0].Name, tagSetName);

            Console.WriteLine("TagSet [{0}] found", tagSetName);
        }
        [Test]
        public void GetTagSetByMultipleNames()
        {
            var tagSetName1 = TagSet1;
            var tagSetName2 = TagSet2;
            bool isTagSet1 = false;
            bool isTagSet2 = false;

            var parameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "TagSetName",
                    MultipleValue = new string[]{tagSetName1,tagSetName2}
                }
            };

            Console.WriteLine("Looking for TagSets [{0}] and [{1}]", tagSetName1, tagSetName2);

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<OutputOctopusTagSet>();

            Assert.AreEqual(2, results.Count);

            foreach (var item in results)
            {
                if (item.Name == tagSetName1)
                {
                    isTagSet1 = true;
                }
                else if (item.Name == tagSetName2)
                {
                    isTagSet2 = true;
                }
                else
                {
                    Console.WriteLine("TagSet found with name that was not expected: [{0}]", item.Name);
                    throw new Exception();
                }
            }

            Assert.IsTrue(isTagSet1);
            Assert.IsTrue(isTagSet2);

            Console.WriteLine("TagSets [{0}] and [{1}] found", tagSetName1, tagSetName2);
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
