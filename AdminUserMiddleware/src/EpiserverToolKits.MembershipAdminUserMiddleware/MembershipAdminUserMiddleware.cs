using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Security;

namespace EpiserverToolKits.MembershipAdminUserMiddleware
{
    using AppFunc = Func<IDictionary<string, object>, Task>;

    public class MembershipAdminUserMiddleware
    {
        private readonly AppFunc _next;
        private readonly AdminUser _adminUser;

        private static readonly string[] _roles = { "WebAdmins", "WebEditors", "Administrators" };
        private static bool _userCreated = false;
        private Lazy<bool> _createUser = new Lazy<bool>(() => false);


        public MembershipAdminUserMiddleware(AppFunc next, AdminUser adminUser)
        {
            _next = next;
            _adminUser = adminUser;
            _createUser = new Lazy<bool>(HasUserAlreadyExisted);
        }

        public async Task Invoke(IDictionary<string, object> environment)
        {
            if (!_createUser.Value && !_userCreated)
            {
                Membership.CreateUser(_adminUser.Username, _adminUser.Password, _adminUser.Email);

                foreach (var role in _roles)
                {
                    EnsureRoleExists(role);
                }

                Roles.AddUserToRoles(_adminUser.Username, _roles);

                _userCreated = true;
            }

            await _next.Invoke(environment);
        }

        private bool HasUserAlreadyExisted()
        {
            if (_userCreated)
                return _userCreated;

            var mu = Membership.GetUser(_adminUser.Username);
            return mu != null;
        }

        private void EnsureRoleExists(string roleName)
        {
            if (Roles.RoleExists(roleName)) return;

            Roles.CreateRole(roleName);
        }

    }
}
