using System;
using System.Collections.Generic;
using System.Linq;
using Octopus.Client;
using Octopus.Client.Model;
using Serilog;

namespace Octoposh.TestDataGenerator.Fixtures
{
    public static class ConfigurationFixture
    {
        private static readonly string[] UserNamePasswordNames = { "UserTests_User1", "UserTests_User2", "Unmodified_TestUser" };
        private static readonly string[] TeamNames = { "TeamTests_Team1", "TeamTests_Team2", "Unmodified_TestTeam" };
        private static readonly string[] UserRoleNames = { "UserRoleTests_UserRole1", "UserRoleTests_UserRole2", "Unmodified_TestUserRole" };

        private static IOctopusAsyncRepository _repository;

        public static void Run(IOctopusAsyncRepository repository)
        {
            _repository = repository;
            Log.Logger.Information("**Running Configuration Fixture**");

            SetFeatures();
            CreateTeams();
            CreateUserRoles();
            CreateUsers();
        }

        #region Teams/Users/UserRoles

        private static List<UserRoleResource> CreateUserRoles()
        {
            var userRoleList = new List<UserRoleResource>() { };

            foreach (var userRoleName in UserRoleNames)
            {
                var role = _repository.UserRoles.FindByName(userRoleName).Result ??
                    new UserRoleResource();

                role.Name = userRoleName;
                role.Description = GeneralResourceProperty.ResourceDescription;
                role.CanBeDeleted = true;
                role.GrantedPermissions = new List<Permission>() { Permission.AccountCreate };

                try
                {
                    Log.Information($"Creating/Modifying User Role [{role.Name}]");
                    userRoleList.Add(role.Id == null ?
                        _repository.UserRoles.Create(role).Result :
                        _repository.UserRoles.Modify(role).Result
                    );
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }

            return userRoleList;
        }

        private static List<UserResource> CreateUsers()
        {
            var userList = new List<UserResource>();

            foreach (var userNamePasswordName in UserNamePasswordNames)
            {
                var user = _repository.Users.FindMany(u => u.Username == userNamePasswordName).Result.FirstOrDefault() ??
                           new UserResource();

                user.Username = userNamePasswordName;
                user.DisplayName = userNamePasswordName;
                user.EmailAddress = userNamePasswordName + "@SpeedWagonCorp.com";
                user.IsActive = true;
                user.IsService = false;
                user.Password = GeneralResourceProperty.ResourcePassword;

                try
                {
                    Log.Information($"Creating/Modifying User [{user.Username}]");
                    userList.Add(user.Id == null ?
                        _repository.Users.Create(user).Result :
                        _repository.Users.Modify(user).Result
                        );
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }

            return userList;
        }

        private static void CreateTeams()
        {
            var teamList = new List<TeamResource>();

            foreach (var teamName in TeamNames)
            {
                var team = _repository.Teams.FindByName(teamName).Result ?? new TeamResource();

                team.Name = teamName;
                team.UserRoleIds = new ReferenceCollection("userroles-systemadministrator");
                //userRoles.ForEach(x => team.UserRoleIds.Add(x.Id));
                //users.ForEach(x => team.MemberUserIds.Add(x.Id));

                try
                {
                    Log.Information($"Creating/Modifying Team [{team.Name}]");
                    teamList.Add(team.Id == null ?
                        _repository.Teams.Create(team).Result :
                        _repository.Teams.Modify(team).Result
                    );
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }

        #endregion

        #region Instance Features

        private static void SetFeatures()
        {
            Log.Information("Setting Instance features (Multi-Tenancy, Community Action templates, etc");

            var fc = _repository.FeaturesConfiguration.GetFeaturesConfiguration().Result;

            fc.IsMultiTenancyEnabled = true;
            fc.IsCommunityActionTemplatesEnabled = false;

            _repository.FeaturesConfiguration.ModifyFeaturesConfiguration(fc).Wait();
        }
        

        #endregion
    }
}
