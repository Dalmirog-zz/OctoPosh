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

        /// <summary>
        /// This test is the equivalent of running the command:
        /// Get-OctopusMachine -MachineName "MachineTests_Machine1"
        /// </summary>
        [TestCase("MachineTests_Machine1")]
        [TestCase("MachineTests_Machine2")]
        public void GetMachineBySingleName(string name)
        {
            //var name = Machine1;

            //Here we are creating the cmdlet parameter "-MachineName 'MachineTests_Machine1'" that will be passed to the cmdlet
            var parameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "MachineName",
                    SingleValue = name
                }};
            
            //Creating the powershell cmdlet.
            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);

            //Running the powershell cmdlet and checking its results.
            var results = powershell.Invoke<OutputOctopusMachine>();

            Assert.AreEqual(1, results.Count);
            Assert.AreEqual(name,results[0].Name);
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

        [TestCase(new[] { "MachineTests_Machine1", "MachineTests_Machine2" }, null)]
        [TestCase(new[] { "MachineTests_Environment_CloudRegion", "MachineTests_Environment_SSH" }, null)]
        public void GetMachineByMultipleNames(string[] machineNames, string unused)
        {
            var parameters = new List<CmdletParameter> {new CmdletParameter()
            {
                Name = "MachineName",
                MultipleValue = machineNames
            }};

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<OutputOctopusMachine>();

            Assert.AreEqual(2, results.Count);
            var machineNamesInResults = results.Select(m => m.Name).ToList();

            foreach (var machineName in machineNames)
            {
                Assert.Contains(machineName,machineNamesInResults);
            }
        }

        [TestCase("*SSH")]
        [TestCase("*CloudRegion")]
        [TestCase("MachineTests_*_Polling")]
        public void GetMachinesByNameUsingWildcard(string namePattern)
        {
            var parameters = new List<CmdletParameter> {
                new CmdletParameter()
                {
                    Name = "MachineName",
                    SingleValue = namePattern
                }
            };

            var pattern = new WildcardPattern(namePattern);

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<OutputOctopusMachine>();

            Assert.Greater(results.Count, 0);

            foreach (var item in results)
            {
                Assert.IsTrue(pattern.IsMatch(item.Name));
            }
        }

        [TestCase("BoneMachine")]
        public void DontGetMachineIfNameDoesntMatch(string name)
        {
            var parameters = new List<CmdletParameter> {new CmdletParameter()
            {
                Name = "MachineName", SingleValue = name
            }};

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<OutputOctopusMachine>();

            Assert.AreEqual(results.Count, 0);
        }

        [TestCase("CloudRegion")]
        [TestCase("ListeningTentacle")]
        [TestCase("PollingTentacle")]
        [TestCase("OfflineDrop")]
        [TestCase("SSHEndpoint")]
        public void GetMachineByCommunicationStyle(string communicationStyle)
        {
            var parameters = new List<CmdletParameter> {new CmdletParameter()
            {
                Name = "CommunicationStyle",
                SingleValue = communicationStyle
            }};

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<OutputOctopusMachine>();

            Assert.Greater(results.Count,0);

            foreach (var machine in results)
            {
                Assert.AreEqual(communicationStyle,machine.CommunicationStyle);
            }
        }

        [TestCase("Dev")]
        [TestCase("Stage")]
        [TestCase("Prod")]
        public void GetmachineBySingleEnvironment(string environmentName)
        {
            var parameters = new List<CmdletParameter> {new CmdletParameter()
            {
                Name = "EnvironmentName", SingleValue = environmentName
            }};

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<OutputOctopusMachine>();

            Assert.Greater(results.Count, 0);

            foreach (var machine in results)
            {
                Assert.Contains(environmentName,machine.EnvironmentName);
            }
        }

        [TestCase(new[] { "Dev", "Stage" }, null)]
        [TestCase(new[] { "Stage", "Prod" }, null)]
        public void GetmachineByMultipleEnvironments(string[] environmentNames,string unused)
        {
            var parameters = new List<CmdletParameter> {new CmdletParameter()
            {
                Name = "EnvironmentName",
                MultipleValue = environmentNames
            }};

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<OutputOctopusMachine>();

            Assert.Greater(results.Count, 0);

            var environmentsFromResults = results.SelectMany(machine => machine.EnvironmentName.ToList()).ToList();

            foreach (var environmentName in environmentNames)
            {
                Assert.Contains(environmentName,environmentsFromResults);
            }
        }
    }
}