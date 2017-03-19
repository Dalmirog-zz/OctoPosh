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
    public class GetOctopusLifecycleTests
    {
        private static readonly string CmdletName = "Get-OctopusLifecycle";
        private static readonly Type CmdletType = typeof(GetOctopusLifecycle);
        private static readonly string Lifecycle1 = "LifecycleTests_Lifecycle1";
        private static readonly string Lifecycle2 = "LifecycleTests_Lifecycle2";

        [Test]
        public void GetLifecycleBySingleName()
        {
            var lifecycleName = Lifecycle1;

            var parameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "lifecycleName",
                    SingleValue = lifecycleName
                }
            };

            Console.WriteLine("Looking for Lifecycle [{0}]", lifecycleName);

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<List<OutputOctopusLifecycle>>()[0];

            Assert.AreEqual(1, results.Count);

            Assert.AreEqual(results[0].Name, lifecycleName);

            Console.WriteLine("Lifecycle [{0}] found", lifecycleName);
        }
        [Test]
        public void GetLifecycleByMultipleNames()
        {
            var lifecycleName1 = Lifecycle1;
            var lifecycleName2 = Lifecycle2;
            bool isLifecycle1 = false;
            bool isLifecycle2 = false;

            var parameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "lifecycleName",
                    MultipleValue = new string[]{lifecycleName1,lifecycleName2}
                }
            };

            Console.WriteLine("Looking for Lifecycles [{0}] and [{1}]", lifecycleName1, lifecycleName2);

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<List<OutputOctopusLifecycle>>()[0];

            Assert.AreEqual(2, results.Count);

            foreach (var item in results)
            {
                if (item.Name == lifecycleName1)
                {
                    isLifecycle1 = true;
                }
                else if (item.Name == lifecycleName2)
                {
                    isLifecycle2 = true;
                }
                else
                {
                    Console.WriteLine("Lifecycle found with name that was not expected: [{0}]", item.Name);
                    throw new Exception();
                }
            }

            Assert.IsTrue(isLifecycle1);
            Assert.IsTrue(isLifecycle2);

            Console.WriteLine("Lifecycles [{0}] and [{1}] found", lifecycleName1, lifecycleName2);
        }
        [Test]
        public void GetLifecycleByNameUsingWildcard()
        {
            var namePattern = "LifecycleTests_*";

            var parameters = new List<CmdletParameter> {
                new CmdletParameter(){
                    Name = "Name", SingleValue = namePattern
                }
            };

            Console.WriteLine("Looking for resources with name pattern: {0}", namePattern);

            var pattern = new WildcardPattern(namePattern);

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<List<OutputOctopusLifecycle>>()[0];

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
        public void DontGetLifecycleIfNameDoesntMatch()
        {
            var resourceName = "TotallyANameThatYoullNeverPutToAResource";

            var parameters = new List<CmdletParameter> {new CmdletParameter()
            {
                Name = "Name", SingleValue = resourceName
            }};

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<List<OutputOctopusLifecycle>>()[0];

            Assert.AreEqual(results.Count, 0);
            Console.WriteLine("No resources found with name [{0}]", resourceName);
        }
        [Test]
        public void GetLifecycleUsingResourceOnlyReturnsRawResource()
        {
            var lifecycleName = Lifecycle1;

            var parameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "resourceOnly"
                },
                new CmdletParameter()
                {
                    Name = "lifecycleName",
                    SingleValue = lifecycleName
                }
            };

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);

            var results = powershell.Invoke<List<LifecycleResource>>();

            //If [results] has at least one item, It'll be of the resource type declared on the powershell.Invoke line, meaning the test passed
            Assert.Greater(results[0].Count, 0);
            ;
        }

    }
}
