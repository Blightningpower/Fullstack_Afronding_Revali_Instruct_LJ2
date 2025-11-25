using System.Security.Claims;

namespace RevaliInstruct.Api.Services
{
    public interface ICurrentUserService
    {
        int? UserId { get; }
        string? Role { get; }
    }

    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int? UserId
        {
            get
            {
                var user = _httpContextAccessor.HttpContext?.User;
                if (user == null || !(user.Identity?.IsAuthenticated ?? false))
                    return null;

                var idClaim =
                    user.FindFirst(ClaimTypes.NameIdentifier) ??
                    user.FindFirst("sub");

                if (idClaim == null)
                    return null;

                return int.TryParse(idClaim.Value, out var id) ? id : (int?)null;
            }
        }

        public string? Role
        {
            get
            {
                var user = _httpContextAccessor.HttpContext?.User;
                return user?.FindFirst(ClaimTypes.Role)?.Value;
            }
        }
    }
}
