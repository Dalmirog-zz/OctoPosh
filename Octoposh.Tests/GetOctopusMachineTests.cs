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
    public class GetOctopusMachineTests
    {
        private static readonly string CmdletName = "Get-OctopusMachine";
        private static readonly Type CmdletType = typeof(GetOctopusMachine);
        private static readonly string Machine1 = "MachineTests_Machine1";
        private static readonly string Machine2 = "MachineTests_Machine2";

        /// <summary>
        /// This test is the equivalent of running the command:
        /// Get-OctopusMachine -MachineName "MachineTests_Machine1"
        /// </summary>
        [Test]
        public void GetMachineBySingleName()
        {
            var name = Machine1;

            //Here we are creating the cmdlet parameter "-MachineName 'MachineTests_Machine1'" that will be passed to the cmdlet
            var parameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "MachineName",
                    SingleValue = name
                }};

            Console.WriteLine("Looking for resource with name [{0}]", name);

            //Creating the powershell cmdlet.
            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);

            //Running the powershell cmdlet and checking its results.
            var results = powershell.Invoke<OutputOctopusMachine>();

            Assert.AreEqual(1, results.Count);

            Console.WriteLine("Found [{0}]");
            foreach (var item in results)
            {
                Console.WriteLine(item.Name);
            }
        }

        /// <summary>
        /// This test is the equivalent of running the command:
        /// Get-OctopusMachine -ResourceOnly
        /// </summary>
        [Test]
        public void GetMachineUsingResourceOnlyReturnsRawResource()
        {
            //Creating a list of parameters only with the switch "-resourceOnly"
            var parameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "resourceOnly"
                }
            };

            //Creating the Powershell cmdlet
            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);

            //Running the Powershell cmdlet
            var results = powershell.Invoke<List<MachineResource>>();

            //If [results] has at least one item, It'll be of the base resource type meaning the test was successful
            Assert.Greater(results[0].Count, 0);
            ;
        }

        [Test]
        public void GetMachineByMultipleNames()
        {

            var names = new[] { Machine1, Machine2 };

            var parameters = new List<CmdletParameter> {new CmdletParameter()
            {
                Name = "MachineName",
                MultipleValue = names
            }};

            Console.WriteLine("Looking for [{0}] machines with the names [{1}]", names.Length, names);
            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<OutputOctopusMachine>();

            Console.WriteLine("Found [{0}] resources", results.Count);
            Assert.AreEqual(2, results.Count);

            foreach (var item in results)
            {
                Console.WriteLine("Resource name: {0}", item.Name);
                Assert.IsTrue(names.Contains(item.Name));
            }
            Console.WriteLine("The [{0}] resources have the expected names", names.Length);
        }

        [Test]
        public void GetMachinesByNameUsingWildcard()
        {
            var namePattern = "MachineTests_*";

            var parameters = new List<CmdletParameter> {new CmdletParameter()
            {
                Name = "MachineName", SingleValue = namePattern
            }};

            Console.WriteLine("Looking for resources with name pattern: {0}", namePattern);

            var pattern = new WildcardPattern(namePattern);

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<OutputOctopusMachine>();

            Console.WriteLine("Resources found: {0}", results.Count);
            Assert.Greater(results.Count, 0);

            foreach (var item in results)
            {
                Console.WriteLine("Resource name: {0}", item.Name);
                Assert.IsTrue(pattern.IsMatch(item.Name));
            }
            Console.WriteLine("All resources found match the pattern [{0}]", namePattern);
        }

        [Test]
        public void DontGetMachineIfNameDoesntMatch()
        {
            var name = "TotallyANameThatYoullNeverPutToAResource";

            var parameters = new List<CmdletParameter> {new CmdletParameter()
            {
                Name = "MachineName", SingleValue = name
            }};

            Console.WriteLine("Looking for a machine with the name [{0}]", name);
            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<OutputOctopusMachine>();

            Assert.AreEqual(results.Count, 0);
            Console.WriteLine("Machine with name [{0}] not found", name);
        }

        [Test]
        public void GetMachineByCommunicationStyle()
        {
            var communicationStyles = new string[] { "ListeningTentacle", "PollingTentacle", "SSHEndpoint", "CloudRegion", "OfflineDrop" };

            foreach (var style in communicationStyles)
            {
                var parameters = new List<CmdletParameter> {new CmdletParameter()
                {
                    Name = "CommunicationStyle", SingleValue = style
                }};

                var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
                var results = powershell.Invoke<OutputOctopusMachine>();

                if (results != null)
                {
                    foreach (var result in results)
                    {
                        Assert.AreEqual(result.CommunicationStyle, style);
                    }
                }
                else
                {
                    Assert.Inconclusive("No targets of the type [{0}] were found", style);
                }
            }
        }
    }
}
