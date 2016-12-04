﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using NUnit.Framework;
using Octoposh.Cmdlets;
using Octoposh.Model;

namespace Octoposh.Tests
{
    [TestFixture]
    public class GetOctopusMachineTests
    {
        [Test]
        public void GetMachineBySingleName()
        {
            var cmdletname = "Get-OctopusMachine";
            var parameters = new List<CmdletParameter> {new CmdletParameter()
            {
                Name = "MachineName", Value = new[] {"North"}
            }};


            var powershell = new CmdletRunspace().CreatePowershellcmdlet(cmdletname, typeof(GetOctopusMachine), parameters);
            var results = powershell.Invoke<List<OutputOctopusMachine>>();

            Assert.AreEqual(results[0].Count, 1);
            Console.WriteLine("Items Found:");
            foreach (var item in results[0])
            {
                Console.WriteLine(item.Name);
            }
        }

        [Test]
        public void GetMachineByMultipleNames()
        {
            var cmdletname = "Get-OctopusMachine";
            var parameters = new List<CmdletParameter> {new CmdletParameter()
            {
                Name = "MachineName", Value = new[] {"North","South"}
            }};

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(cmdletname, typeof(GetOctopusMachine),parameters);
            var results = powershell.Invoke<List<OutputOctopusMachine>>();
            
            Assert.AreEqual(results[0].Count,2);

            Console.WriteLine("Items Found:");
            foreach (var item in results[0])
            {
                Console.WriteLine(item.Name);
            }
        }
            
    }
}
