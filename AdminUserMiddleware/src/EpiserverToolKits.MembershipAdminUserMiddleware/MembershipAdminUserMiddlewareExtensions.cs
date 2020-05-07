using Owin;

namespace EpiserverToolKits.MembershipAdminUserMiddleware
{
    public static class MembershipAdminUserMiddlewareExtensions
    {

        public static void UseMembershipAdminUser(this IAppBuilder appBuilder, AdminUser adminUser)
        {
            appBuilder.Use(typeof(MembershipAdminUserMiddleware), adminUser);
        }
    }
}
