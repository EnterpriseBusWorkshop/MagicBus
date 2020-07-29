using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace MagicBus.AdminPortal
{
    public class AuthenticationMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task Invoke(HttpContext httpContext)
        {
            var authResult = await httpContext.AuthenticateAsync();
            if (!authResult.Succeeded)
            {
                await httpContext.ChallengeAsync();
                return;
            }
            await _next(httpContext);
        }
    }
}