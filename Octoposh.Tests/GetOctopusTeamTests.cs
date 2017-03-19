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
    public class GetOctopusTeamTests
    {
        private static readonly string CmdletName = "Get-OctopusTeam";
        private static readonly Type CmdletType = typeof(GetOctopusTeam);
        private static readonly string Team1 = "TeamTests_Team1";
        private static readonly string Team2 = "TeamTests_Team2";

        [Test]
        public void GetTeamBySingleName()
        {
            var teamName = Team1;

            var parameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "TeamName",
                    SingleValue = teamName
                }
            };

            Console.WriteLine("Looking for Team [{0}]",teamName);

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<List<OutputOctopusTeam>>()[0];

            Assert.AreEqual(1 ,results.Count);

            Assert.AreEqual(results[0].Name, teamName);

            Console.WriteLine("Team [{0}] found",teamName);
        }
        [Test]
        public void GetTeamByMultipleNames()
        {
            var teamName1 = Team1;
            var teamName2 = Team2;
            bool isTeam1 = false;
            bool isTeam2 = false;

            var parameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "TeamName",
                    MultipleValue = new string[]{teamName1,teamName2}
                }
            };

            Console.WriteLine("Looking for Teams [{0}] and [{1}]", teamName1, teamName2);

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<List<OutputOctopusTeam>>()[0];

            Assert.AreEqual(2, results.Count);

            foreach (var item in results)
            {
                if (item.Name == teamName1)
                {
                    isTeam1 = true;
                }
                else if (item.Name == teamName2)
                {
                    isTeam2 = true;
                }
                else
                {
                    Console.WriteLine("Team found with name that was not expected: [{0}]", item.Name);
                    throw new Exception();
                }
            }

            Assert.IsTrue(isTeam1);
            Assert.IsTrue(isTeam2);

            Console.WriteLine("Teams [{0}] and [{1}] found", teamName1, teamName2);
        }
        [Test]
        public void GetTeamByNameUsingWildcard()
        {
            var namePattern = "TeamTests_*";

            var parameters = new List<CmdletParameter> {
                new CmdletParameter(){
                    Name = "Name", SingleValue = namePattern
                }
            };

            Console.WriteLine("Looking for resources with name pattern: {0}", namePattern);

            var pattern = new WildcardPattern(namePattern);

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<List<OutputOctopusTeam>>()[0];

            Assert.AreEqual(2, results.Count);
            Console.WriteLine("Resources found: {0}", results.Count);

            foreach (var item in results)
            {
                Console.WriteLine("Resource name: {0}", item.Name);
                Assert.IsTrue(pattern.IsMatch(item.Name));
            }
            Console.WriteLine("Resources found match pattern [{0}]",namePattern);
        }
        [Test]
        public void DontGetTeamIfNameDoesntMatch()
        {
            var resourceName = "TotallyANameThatYoullNeverPutToAResource";

            var parameters = new List<CmdletParameter> {new CmdletParameter()
            {
                Name = "Name", SingleValue = resourceName
            }};

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<List<OutputOctopusTeam>>()[0];

            Assert.AreEqual(results.Count, 0);
            Console.WriteLine("No resources found with name [{0}]",resourceName);
        }
        [Test]
        public void GetTeamUsingResourceOnlyReturnsRawResource()
        {
            var teamName = Team1;

            var parameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "resourceOnly"
                },
                new CmdletParameter()
                {
                    Name = "TeamName",
                    SingleValue = teamName
                }
            };

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);

            var results = powershell.Invoke<List<TeamResource>>();

            //If [results] has at least one item, It'll be of the resource type declared on the powershell.Invoke line, meaning the test passed
            Assert.Greater(results[0].Count, 0);
            ;
        }

    }
}
