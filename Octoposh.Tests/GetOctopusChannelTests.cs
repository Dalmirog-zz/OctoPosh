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
    public class GetOctopusChannelTests
    {
        private static readonly string CmdletName = "Get-OctopusChannel";
        private static readonly Type CmdletType = typeof(GetOctopusChannel);
        private static readonly string Channel1 = "ChannelTests_Channel1";
        private static readonly string Channel2 = "ChannelTests_Channel2";
        private static readonly string Project1 = "ProjectTests_Project1";
        private static readonly string Project2 = "ProjectTests_Project2";
        private static readonly string DefaultChannel = "Default";
        
        [Test]
        public void GetChannelBySingleNameAndSingleProject()
        {
            var channelName = DefaultChannel;
            var projectName = Project1;

            var parameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "ChannelName",
                    SingleValue = channelName
                },
                new CmdletParameter()
                {
                    Name = "ProjectName",
                    SingleValue = projectName
                }
            };

            Console.WriteLine("Looking for a channel with name [{0}] in project [{1}]", channelName,projectName);

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<OutputOctopusChannel>();

            Console.WriteLine("Found [{0}] channels", results.Count);
            Assert.AreEqual(1,results.Count);
            
            Assert.AreEqual(results[0].Name, channelName);
            Assert.AreEqual(results[0].ProjectName, projectName);

            Console.WriteLine("The [{0}] channel found has the name [{1}] and belongs to the project [{2}]", results.Count, channelName,projectName);
        }

        [Test]
        public void GetChannelBySingleNameAndMultipleProjects()
        {
            var channelName = DefaultChannel;
            var projectNames = new string[] {Project1,Project2};

            var parameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "ChannelName",
                    SingleValue = channelName
                },
                new CmdletParameter()
                {
                    Name = "ProjectName",
                    MultipleValue = projectNames
                }
            };

            Console.WriteLine("Looking for channels with name [{0}] in [{1}] projects", channelName, projectNames.Length);

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<OutputOctopusChannel>();

            Console.WriteLine("Found [{0}] channels", results.Count);

            Assert.AreEqual(projectNames.Length,results.Count);

            foreach (var resource in results)
            {
                Assert.AreEqual(channelName,resource.Name);
                Assert.IsTrue(projectNames.Contains(resource.ProjectName));
            }

            Console.WriteLine("The [{0}] channels found have the name [{1}] and belongs to the [{2}] projects in the list", results.Count, channelName,projectNames.Length);
        }

        [Test]
        public void GetChannelBySingleNameInAllProjects()
        {
            var channelName = DefaultChannel;

            var parameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "ChannelName",
                    SingleValue = channelName
                }
            };

            Console.WriteLine("Looking for channels with name [{0}] ", channelName);

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<OutputOctopusChannel>();

            Console.WriteLine("Found [{0}] channels", results.Count);
            Assert.Greater(results.Count,1);

            foreach (var channel in results)
            {
                Assert.AreEqual(channel.Name, channelName);
            }

            Console.WriteLine("The [{0}] channels found have the name [{1}]", results.Count, channelName);
        }

        [Test]
        public void GetChannelByMultipleNamesAndSingleProject()
        {
            var projectName = Project1;
            var name1 = Channel1;
            var name2 = Channel2;
            var names = new string[] { name1, name2 };
            bool isName1 = false;
            bool isName2 = false;

            var parameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "ChannelName",
                    MultipleValue = names
                },
                new CmdletParameter()
                {
                    Name = "ProjectName",
                    SingleValue = projectName
                }
            };

            Console.WriteLine("Looking for a channels with names [{0}] and [{1}] in project", name1, name2,projectName);

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<OutputOctopusChannel>();

            Console.WriteLine("Found [{0}] channels", results.Count);
            Assert.AreEqual(names.Length,results.Count);

            foreach (var resource in results)
            {
                if (resource.Name == name1)
                {
                    isName1 = true;
                }
                else if (resource.Name == name2)
                {
                    isName2 = true;
                }
                else
                {
                    Console.WriteLine("Team found with name that was not expected: [{0}]", resource.Name);
                    throw new Exception();
                }

                Assert.AreEqual(projectName,resource.ProjectName);
            }

            Assert.IsTrue(isName1);
            Assert.IsTrue(isName2);

            Console.WriteLine("The [{0}] channels found have either the name [{1}] or [{2}] and belong to the project [{3}]", results.Count, name1, name2,projectName);
        }

        [Test]
        public void GetChannelByMultipleNamesInAllProjects()
        {
            var name1 = Channel1;
            var name2 = Channel2;
            var names = new string[] {name1, name2};
            bool isName1 = false;
            bool isName2 = false;

            var parameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "ChannelName",
                    MultipleValue = names
                }
            };

            Console.WriteLine("Looking for a channels with names [{0}] or [{1}]", name1, name2);

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<OutputOctopusChannel>();

            Console.WriteLine("Found [{0}] channels", results.Count);

            foreach (var resource in results)
            {
                if (resource.Name == name1)
                {
                    isName1 = true;
                }
                else if (resource.Name == name2)
                {
                    isName2 = true;
                }
                else
                {
                    Console.WriteLine("Channel found with name that was not expected: [{0}]", resource.Name);
                    throw new Exception();
                }
            }

            Assert.IsTrue(isName1);
            Assert.IsTrue(isName2);

            Console.WriteLine("The [{0}] channels found have either the name [{1}] or [{2}]", results.Count, name1, name2);
        }

        [Test]
        public void GetChannelUsingResourceOnlyReturnsRawResource()
        {
            var name = Channel1;
            var projectName = Project1;

            var parameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "resourceOnly"
                },
                new CmdletParameter()
                {
                    Name = "ChannelName",
                    SingleValue = name
                },
                new CmdletParameter()
                {
                    Name = "ProjectName",
                    SingleValue = projectName
                }
            };

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);

            var results = powershell.Invoke<List<ChannelResource>>();

            //If [results] has at least one item, It'll be of the resource type declared on the powershell.Invoke line, meaning the test passed
            Assert.Greater(results[0].Count, 0);
            ;
        }
    }
}

