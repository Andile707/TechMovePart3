using glms_frontend_web.Models;

namespace glms_frontend_web.Services
{
    public interface IAuthApiService
    {
        Task<string?> LoginAsync(LoginViewModel model);
    }
}