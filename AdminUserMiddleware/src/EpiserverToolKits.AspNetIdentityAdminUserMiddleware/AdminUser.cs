namespace EpiserverToolKits.AspNetIdentityAdminUserMiddleware
{
    public class AdminUser
    {
        public string Username { get; private set; }
        public string Password { get; private set; }
        public string Email { get; private set; }

        public AdminUser(string username, string password, string email)
        {
            Username = username;
            Password = password;
            Email = email;
        }

    }
}
