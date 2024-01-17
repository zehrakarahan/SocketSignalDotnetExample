namespace SocketSignalExample.Model
{
    public class UserLoginResponse
    {
        public bool AuthenticateResult { get; set; }
        public string AuthToken { get; set; }
        public DateTime AccessTokenExpireDate { get; set; }
    }

    public class UserModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
