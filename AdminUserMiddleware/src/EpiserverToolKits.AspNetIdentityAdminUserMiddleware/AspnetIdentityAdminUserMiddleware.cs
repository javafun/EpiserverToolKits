using EPiServer.Cms.UI.AspNetIdentity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EpiserverToolKits.AspNetIdentityAdminUserMiddleware
{
    using AppFunc = Func<IDictionary<string, object>, Task>;

    public class AspnetIdentityAdminUserMiddleware
    {
        private readonly AppFunc _next;
        private readonly AdminUser _adminUser;

        private static readonly string[] _roles = { "WebAdmins", "WebEditors", "Administrators" };
        public static bool hasCreated = false;

        private static Lazy<bool> _createUser = new Lazy<bool>(() => false);

        public AspnetIdentityAdminUserMiddleware(AppFunc next, AdminUser adminUser)
        {
            _next = next;
            _adminUser = adminUser;
            _createUser = new Lazy<bool>(HasUserAlreadyExisted);
        }


        public async Task Invoke(IDictionary<string, object> environment)
        {
            if (!_createUser.Value)
            {
                using (UserStore<ApplicationUser> store = new UserStore<ApplicationUser>(new ApplicationDbContext<ApplicationUser>("EPiServerDB")))
                {
                    var createdUser = CreateUser(store, _adminUser.Username, _adminUser.Password, _adminUser.Email);
                    AddUserToRoles(store, createdUser, _roles);
                    await store.UpdateAsync(createdUser);
                }
            }

            await _next.Invoke(environment);
        }


        private bool HasUserAlreadyExisted()
        {
            using (UserStore<ApplicationUser> store = new UserStore<ApplicationUser>(new ApplicationDbContext<ApplicationUser>("EPiServerDB")))
            {
                //If there's already a user, then we don't need a seed
                if (!store.Users.Any(x => x.UserName == _adminUser.Username))
                {
                    return false;
                }
            }

            return true;
        }

        private ApplicationUser CreateUser(UserStore<ApplicationUser> store, string username, string password, string email)
        {
            //We know that this Password hasher is used as it's configured
            IPasswordHasher hasher = new PasswordHasher();
            string passwordHash = hasher.HashPassword(password);

            ApplicationUser applicationUser = new ApplicationUser
            {
                Email = email,
                EmailConfirmed = true,
                LockoutEnabled = true,
                IsApproved = true,
                UserName = username,
                PasswordHash = passwordHash,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            store.CreateAsync(applicationUser).GetAwaiter().GetResult();

            //Get the user associated with our username
            ApplicationUser createdUser = store.FindByNameAsync(username).GetAwaiter().GetResult();
            return createdUser;
        }

        private void AddUserToRoles(UserStore<ApplicationUser> store, ApplicationUser user, string[] roles)
        {
            IUserRoleStore<ApplicationUser, string> userRoleStore = store;
            using (var roleStore = new RoleStore<IdentityRole>(new ApplicationDbContext<ApplicationUser>("EPiServerDB")))
            {
                IList<string> userRoles = userRoleStore.GetRolesAsync(user).GetAwaiter().GetResult();
                foreach (string roleName in roles)
                {
                    if (roleStore.FindByNameAsync(roleName).GetAwaiter().GetResult() == null)
                    {
                        roleStore.CreateAsync(new IdentityRole { Name = roleName }).GetAwaiter().GetResult();
                    }
                    if (!userRoles.Contains(roleName))
                        userRoleStore.AddToRoleAsync(user, roleName).GetAwaiter().GetResult();
                }
            }
        }
    }
}
