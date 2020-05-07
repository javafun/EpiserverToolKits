using Owin;

namespace EpiserverToolKits.AspNetIdentityAdminUserMiddleware
{
    public static class AspnetIdentityAdminUserMiddlewareExtensions
    {
        public static void UseAspnetIdentityAdminUser(this IAppBuilder appBuilder, AdminUser adminUser)
        {
            appBuilder.Use(typeof(AspnetIdentityAdminUserMiddleware), adminUser);
        }
    }
}
