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
    public class GetOctopusUserRoleTests
    {
        private static readonly string CmdletName = "Get-OctopusUserRole";
        private static readonly Type CmdletType = typeof(GetOctopusUserRole);
        private static readonly string UserRole1 = "UserRoleTests_UserRole1";
        private static readonly string UserRole2 = "UserRoleTests_UserRole2";

        [Test]
        public void GetUserRoleBySingleName()
        {
            var UserRoleName = UserRole1;

            var parameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "UserRoleName",
                    SingleValue = UserRoleName
                }
            };

            Console.WriteLine("Looking for UserRole [{0}]", UserRoleName);

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<UserRoleResource>();

            Assert.AreEqual(1, results.Count);

            Assert.AreEqual(results[0].Name, UserRoleName);

            Console.WriteLine("UserRole [{0}] found", UserRoleName);
        }
        [Test]
        public void GetUserRoleByMultipleNames()
        {
            var UserRoleName1 = UserRole1;
            var UserRoleName2 = UserRole2;
            bool isUserRole1 = false;
            bool isUserRole2 = false;

            var parameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "UserRoleName",
                    MultipleValue = new string[]{UserRoleName1,UserRoleName2}
                }
            };

            Console.WriteLine("Looking for UserRoles [{0}] and [{1}]", UserRoleName1, UserRoleName2);

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<UserRoleResource>();

            Assert.AreEqual(2, results.Count);

            foreach (var item in results)
            {
                if (item.Name == UserRoleName1)
                {
                    isUserRole1 = true;
                }
                else if (item.Name == UserRoleName2)
                {
                    isUserRole2 = true;
                }
                else
                {
                    Console.WriteLine("UserRole found with name that was not expected: [{0}]", item.Name);
                    throw new Exception();
                }
            }

            Assert.IsTrue(isUserRole1);
            Console.WriteLine("UserRole [{0}] found", UserRoleName1);
            Assert.IsTrue(isUserRole2);
            Console.WriteLine("UserRole [{0}] found", UserRoleName2);
        }
        [Test]
        public void GetUserRoleByNameUsingWildcard()
        {
            var namePattern = "UserRoleTests_*";

            var parameters = new List<CmdletParameter> {
                new CmdletParameter(){
                    Name = "Name",
                    SingleValue = namePattern
                }
            };

            Console.WriteLine("Looking for resources with name pattern: [{0}]", namePattern);

            var pattern = new WildcardPattern(namePattern);

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<UserRoleResource>();

            Assert.AreEqual(2, results.Count);
            Console.WriteLine("Resources found: [{0}]", results.Count);

            foreach (var item in results)
            {
                Console.WriteLine("Resource name: [{0}]", item.Name);
                Assert.IsTrue(pattern.IsMatch(item.Name));
            }
            Console.WriteLine("All resources found match pattern [{0}]", namePattern);
        }
        [Test]
        public void DontGetUserRoleIfNameDoesntMatch()
        {
            var resourceName = "TotallyANameThatYoullNeverPutToAResource";

            var parameters = new List<CmdletParameter> {new CmdletParameter()
            {
                Name = "Name",
                SingleValue = resourceName
            }};

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<UserRoleResource>();

            Assert.AreEqual(results.Count, 0);
            Console.WriteLine("No resources found with name [{0}]", resourceName);
        }
    }
}
