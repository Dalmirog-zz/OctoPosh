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
    public class GetOctopusUserTests
    {
        private static readonly string CmdletName = "Get-OctopusUser";
        private static readonly Type CmdletType = typeof(GetOctopusUser);
        private static readonly string User1 = "UserTests_User1";
        private static readonly string User2 = "UserTests_User2";

        [Test]
        public void GetUserBySingleName()
        {
            var userName = User1;

            var parameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "UserName",
                    SingleValue = userName
                }
            };

            Console.WriteLine("Looking for User [{0}]", userName);

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<List<OutputOctopusUser>>()[0];

            Assert.AreEqual(1, results.Count);

            Assert.AreEqual(results[0].UserName, userName);

            Console.WriteLine("User [{0}] found", userName);
        }
        [Test]
        public void GetUserByMultipleNames()
        {
            var userName1 = User1;
            var userName2 = User2;
            bool isUser1 = false;
            bool isUser2 = false;

            var parameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "UserName",
                    MultipleValue = new string[]{userName1,userName2}
                }
            };

            Console.WriteLine("Looking for Users [{0}] and [{1}]", userName1, userName2);

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<List<OutputOctopusUser>>()[0];

            Assert.AreEqual(2, results.Count);

            foreach (var item in results)
            {
                if (item.UserName == userName1)
                {
                    isUser1 = true;
                }
                else if (item.UserName == userName2)
                {
                    isUser2 = true;
                }
                else
                {
                    Console.WriteLine("User found with name that was not expected: [{0}]", item.UserName);
                    throw new Exception();
                }
            }

            Assert.IsTrue(isUser1);
            Console.WriteLine("User [{0}] found", userName1);
            Assert.IsTrue(isUser2);
            Console.WriteLine("User [{0}] found", userName2);
        }
        [Test]
        public void GetUserByNameUsingWildcard()
        {
            var namePattern = "UserTests_*";

            var parameters = new List<CmdletParameter> {
                new CmdletParameter(){
                    Name = "Name",
                    SingleValue = namePattern
                }
            };

            Console.WriteLine("Looking for resources with name pattern: [{0}]", namePattern);

            var pattern = new WildcardPattern(namePattern);

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<List<OutputOctopusUser>>()[0];

            Assert.AreEqual(2, results.Count);
            Console.WriteLine("Resources found: [{0}]", results.Count);

            foreach (var item in results)
            {
                Console.WriteLine("Resource name: [{0}]", item.UserName);
                Assert.IsTrue(pattern.IsMatch(item.UserName));
            }
            Console.WriteLine("All resources found match pattern [{0}]", namePattern);
        }
        [Test]
        public void DontGetUserIfNameDoesntMatch()
        {
            var resourceName = "TotallyANameThatYoullNeverPutToAResource";

            var parameters = new List<CmdletParameter> {new CmdletParameter()
            {
                Name = "Name",
                SingleValue = resourceName
            }};

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<List<OutputOctopusUser>>()[0];

            Assert.AreEqual(results.Count, 0);
            Console.WriteLine("No resources found with name [{0}]", resourceName);
        }
        
        [Test]
        public void GetUserUsingResourceOnlyReturnsRawResource()
        {
            var userName = User1;

            var parameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "resourceOnly"
                },
                new CmdletParameter()
                {
                    Name = "UserName",
                    SingleValue = userName
                }
            };

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            
            var results = powershell.Invoke<List<UserResource>>()[0];

            //If [results] has at least one item, It'll be of the resource type declared on the powershell.Invoke line, meaning the test passed
            Assert.AreEqual(1, results.Count);
            ;
        }

    }
}
