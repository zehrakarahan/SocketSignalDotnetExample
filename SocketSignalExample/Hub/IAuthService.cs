using SocketSignalExample.Model;

namespace SocketSignalRExample.Hub
{
    public interface IAuthService
    {
        public Task<UserLoginResponse> LoginUserAsync(UserLoginRequest request);

        public string GenerateJSONWebToken(UserModel userInfo);
    }
}
