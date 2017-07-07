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
    public class GetOctopusTenantTests
    {
        private static readonly string CmdletName = "Get-OctopusTenant";
        private static readonly Type CmdletType = typeof(GetOctopusTenant);
        private static readonly string Tenant1 = "TenantTests_Tenant1";
        private static readonly string Tenant2 = "TenantTests_Tenant2";

        [Test]
        public void GetTenantBySingleName()
        {
            var tenantName = Tenant1;

            var parameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "TenantName",
                    SingleValue = tenantName
                }
            };

            Console.WriteLine("Looking for Tenant [{0}]", tenantName);

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<OutputOctopusTenant>();

            Assert.AreEqual(1, results.Count);

            Assert.AreEqual(results[0].Name, tenantName);

            Console.WriteLine("Tenant [{0}] found", tenantName);
        }
        [Test]
        public void GetTenantByMultipleNames()
        {
            var tenantName1 = Tenant1;
            var tenantName2 = Tenant2;
            bool isTenant1 = false;
            bool isTenant2 = false;

            var parameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "TenantName",
                    MultipleValue = new string[]{tenantName1,tenantName2}
                }
            };

            Console.WriteLine("Looking for Tenants [{0}] and [{1}]", tenantName1, tenantName2);

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<OutputOctopusTenant>();

            Assert.AreEqual(2, results.Count);

            foreach (var item in results)
            {
                if (item.Name == tenantName1)
                {
                    isTenant1 = true;
                }
                else if (item.Name == tenantName2)
                {
                    isTenant2 = true;
                }
                else
                {
                    Console.WriteLine("Tenant found with name that was not expected: [{0}]", item.Name);
                    throw new Exception();
                }
            }

            Assert.IsTrue(isTenant1);
            Assert.IsTrue(isTenant2);

            Console.WriteLine("Tenants [{0}] and [{1}] found", tenantName1, tenantName2);
        }
        [Test]
        public void GetTenantByNameUsingWildcard()
        {
            var namePattern = "TenantTests_*";

            var parameters = new List<CmdletParameter> {
                new CmdletParameter(){
                    Name = "Name", SingleValue = namePattern
                }
            };

            Console.WriteLine("Looking for resources with name pattern: {0}", namePattern);

            var pattern = new WildcardPattern(namePattern);

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<OutputOctopusTenant>();

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
        public void DontGetTenantIfNameDoesntMatch()
        {
            var resourceName = "TotallyANameThatYoullNeverPutToAResource";

            var parameters = new List<CmdletParameter> {new CmdletParameter()
            {
                Name = "Name", SingleValue = resourceName
            }};

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<OutputOctopusTenant>();

            Assert.AreEqual(results.Count, 0);
            Console.WriteLine("No resources found with name [{0}]", resourceName);
        }
        [Test]
        public void GetTenantUsingResourceOnlyReturnsRawResource()
        {
            var TenantName = Tenant1;

            var parameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "resourceOnly"
                },
                new CmdletParameter()
                {
                    Name = "TenantName",
                    SingleValue = TenantName
                }
            };

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);

            var results = powershell.Invoke<List<TenantResource>>();

            //If [results] has at least one item, It'll be of the resource type declared on the powershell.Invoke line, meaning the test passed
            Assert.Greater(results[0].Count, 0);
            ;
        }

    }
}
