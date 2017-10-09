using System;
using System.Collections.Generic;
using System.Linq;
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

        private static List<ProjectResource> _allProjects;

        public GetOctopusChannelTests()
        {
            _allProjects = TestUtilities.Repository.Projects.FindAll();
        }

        [TestCase("ChannelTests_Channel1", "ProjectTests_Project1")]
        [TestCase("ChannelTests_Channel2", "ProjectTests_Project1")]
        [TestCase("ChannelTests_Channel1", "ProjectTests_Project2")]
        [TestCase("ChannelTests_Channel2", "ProjectTests_Project2")]
        public void GetChannelBySingleNameAndSingleProject(string channelName, string projectName)
        {
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
            

            //todo ask Shannon - How to make this more generic.
            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<OutputOctopusChannel>();

            Assert.AreEqual(1, results.Count);

            Assert.AreEqual(results[0].Name, channelName);
            Assert.AreEqual(results[0].ProjectName, projectName);
        }

        [TestCase("Default", new[] { "ProjectTests_Project1", "ProjectTests_Project2" })]
        [TestCase("ChannelTests_Channel1", new[] { "ProjectTests_Project1", "ProjectTests_Project2" })]
        [TestCase("ChannelTests_Channel2", new[] { "ProjectTests_Project1", "ProjectTests_Project2" })]
        public void GetChannelBySingleNameAndMultipleProjects(string channelName, string[] projectNames)
        {
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

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<OutputOctopusChannel>();

            Assert.AreEqual(projectNames.Length, results.Count);

            foreach (var resource in results)
            {
                Assert.AreEqual(channelName, resource.Name);
                Assert.IsTrue(projectNames.Contains(resource.ProjectName));
            }

        }

        [TestCase("Default")]
        [TestCase("ChannelTests_Globalchannel")]
        public void GetChannelBySingleNameInAllProjects(string channelName)
        {
            var parameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "ChannelName",
                    SingleValue = channelName
                }
            };

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<OutputOctopusChannel>();

            Assert.AreEqual(results.Count, _allProjects.Count);

            var allProjectNamesInResults = results.Select(c => c.ProjectName).ToList();

            foreach (var project in _allProjects)
            {
                Assert.Contains(project.Name,allProjectNamesInResults);

                foreach (var channel in results)
                {
                    Assert.AreEqual(channel.Name, channelName);
                }
            }
        }

        //todo Add test for getting channels using wildcards
        //todo Add test for getting no channels if name doesn't exist

        [TestCase(new[] { "ChannelTests_Globalchannel", "Default" }, "ProjectTests_Project1")]
        [TestCase(new[] { "ChannelTests_Channel1", "ChannelTests_Channel2" }, "ProjectTests_Project2")]
        public void GetChannelByMultipleNamesAndSingleProject(string[] channelNames, string projectName)
        {
            var parameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "ChannelName",
                    MultipleValue = channelNames
                },
                new CmdletParameter()
                {
                    Name = "ProjectName",
                    SingleValue = projectName
                }
            };

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<OutputOctopusChannel>();

            Assert.AreEqual(channelNames.Length, results.Count);

            var channelNamesInResult = results.Select(c => c.Name).ToList();

            foreach (var channelName in channelNames)
            {
                Assert.Contains(channelName,channelNamesInResult);

                foreach (var channel in results)
                {
                    Assert.AreEqual(channel.ProjectName,projectName);
                }
            }
            
        }

        [TestCase(new[] { "ChannelTests_Globalchannel", "Default" }, null)]
        public void GetChannelByMultipleNamesInAllProjects(string[] channelNames, string unused)
        {
            var parameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "ChannelName",
                    MultipleValue = channelNames
                }
            };

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<OutputOctopusChannel>();

            foreach (var project in _allProjects)
            {
                foreach (var channelName in channelNames)
                {
                    Assert.AreEqual(1, results.Count(c => c.Name == channelName && c.ProjectName == project.Name));
                }
            }
        }

        [Test]
        public void GetChannelUsingResourceOnlyReturnsRawResource()
        {
            var parameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "resourceOnly"
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

