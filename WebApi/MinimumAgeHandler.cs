
using Microsoft.AspNetCore.Authorization;

namespace WebApi
{
    public class MinimumAgeHandler : AuthorizationHandler<MinimumAgeRequirement>
    {
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, MinimumAgeRequirement requirement)
        {
           context.Succeed(requirement);
            return;
        }
    }
}
