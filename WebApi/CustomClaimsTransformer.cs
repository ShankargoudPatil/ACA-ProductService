using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.Text.Json;

namespace WebApi;

public class CustomClaimsTransformer : IClaimsTransformation
{
    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        var identity = (ClaimsIdentity)principal.Identity;

        // Locate the Zitadel role claim
        var zitadelRoleClaim = identity.FindFirst("urn:zitadel:iam:org:project:roles");

        if (zitadelRoleClaim != null)
        {
            try
            {
                var roleData = JsonDocument.Parse(zitadelRoleClaim.Value);
                // Loop through the properties (roles) dynamically
                foreach (var role in roleData.RootElement.EnumerateObject())
                {
                    // For each role found, add it as a claim in ASP.NET Core
                    if (role.Value.ValueKind == JsonValueKind.Object)
                    {
                      Console.WriteLine($"User Role is ==>: {role.Name}");
                        identity.AddClaim(new Claim(ClaimTypes.Role, role.Name));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing role claims: {ex.Message}");
            }
        }

        return Task.FromResult(principal);
    }
}
