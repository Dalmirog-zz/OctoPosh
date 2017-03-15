using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using NUnit.Framework;
using Octoposh.Cmdlets;
using Octoposh.Model;
using Octopus.Client;
using Octopus.Client.Model;

namespace Octoposh.Tests
{
    [TestFixture]
    public class SetOctopusReleaseStatusTests
    {
        //If sure this mess of initialization variables can be much simpler. But its 10pm and it works :). We accept PRs!
        private static readonly string CmdletName = "Set-OctopusReleaseStatus";
        private static readonly Type CmdletType = typeof(SetOctopusReleaseStatus);
        private static readonly string ProjectName = "ReleaseTests_Project1";

        private static readonly string Server = string.Concat("http://localhost:", ConfigurationManager.AppSettings["OctopusBindingPort"]);
        private static readonly string Apikey = ConfigurationManager.AppSettings["OctopusAPIKey"];
        private static readonly OctopusServerEndpoint Endpoint = new OctopusServerEndpoint(Server, Apikey);
        private static readonly OctopusRepository Repository = new OctopusRepository(Endpoint);

        private static readonly ProjectResource Project = Repository.Projects.FindByName(ProjectName);
        private static readonly string ReleaseVersion = "0.0.1";
        private static readonly ReleaseResource Release = Repository.Projects.GetReleaseByVersion(Project, ReleaseVersion);

        private static readonly string Blocked = "Blocked";
        private static readonly string Unblocked = "Unblocked";
        private static readonly string Description = $"Raising defect from unit tests on [{DateTime.Now}]";

        private const string ByReleaseResource = "ByReleaseResource";
        private const string ByProjectAndVersion = "ByProjectAndVersion";

        [Test]
        public void BlockAndUnblockReleaseByProjectNameAndVersion()
        {
            var latestDefect = Repository.Defects.GetDefects(Release).Items.LastOrDefault();

            //If the release doesn't have any defects or the last one has the status "Resolved" (meaning its not blocked) => Block first and unblock later.
            if (latestDefect == null || latestDefect.Status.ToString() == "Resolved")
            {
                Assert.IsTrue(InvokeCmdlet(Release, Blocked, ByProjectAndVersion));
                Assert.IsTrue(InvokeCmdlet(Release, Unblocked, ByProjectAndVersion));
            }
            //else Unblock first, and block later.
            else
            {
                Assert.IsTrue(InvokeCmdlet(Release, Unblocked, ByProjectAndVersion));
                Assert.IsTrue(InvokeCmdlet(Release, Blocked, ByProjectAndVersion));
            }
        }

        [Test]
        public void BlockAndUnblockReleaseByReleaseResource()
        {
            var latestDefect = Repository.Defects.GetDefects(Release).Items.LastOrDefault();

            //If the release doesn't have any defects or the last one has the status "Resolved" (meaning its not blocked) => Block first and unblock later.
            if (latestDefect == null || latestDefect.Status.ToString() == "Resolved")
            {
                Assert.IsTrue(InvokeCmdlet(Release, Blocked, ByReleaseResource));
                Assert.IsTrue(InvokeCmdlet(Release, Unblocked, ByReleaseResource));
            }

            //else Unblock first, and block later.
            else
            {
                Assert.IsTrue(InvokeCmdlet(Release, Unblocked, ByReleaseResource));
                Assert.IsTrue(InvokeCmdlet(Release, Blocked, ByReleaseResource));
            }
        }

        private bool InvokeCmdlet(ReleaseResource release,string status,string parameterSet)
        {
            List<CmdletParameter> parameters;

            if (parameterSet == ByProjectAndVersion)
            {
                parameters = new List<CmdletParameter>
                {
                    new CmdletParameter()
                    {
                        Name = "ProjectName",
                        SingleValue = ProjectName
                    },
                    new CmdletParameter()
                    {
                        Name = "ReleaseVersion",
                        SingleValue = ReleaseVersion
                    },
                    new CmdletParameter()
                    {
                        Name = "Status",
                        SingleValue = status
                    }
                    ,
                    new CmdletParameter()
                    {
                        Name = "Description",
                        SingleValue = Description
                    }
                };
            }
            else
            {
                parameters = new List<CmdletParameter>
                {
                    new CmdletParameter()
                    {
                        Name = "Resource",
                        Resource = Release
                    },
                    new CmdletParameter()
                    {
                        Name = "Status",
                        SingleValue = status
                    }
                    ,
                    new CmdletParameter()
                    {
                        Name = "Description",
                        SingleValue = Description
                    }
                };
            }

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);

            try
            {
                powershell.Invoke<List<OutputOctopusTeam>>();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }

            return true;
        }
    }
}
