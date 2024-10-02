using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.Json;
using Microsoft.IdentityModel.Tokens;

namespace WebAPI_ZitaDel;
public class ValidatorError : Exception
{
    public Dictionary<string, string> Error { get; }
    public int StatusCode { get; }

    public ValidatorError(Dictionary<string, string> error, int statusCode)
    {
        Error = error;
        StatusCode = statusCode;
    }
}

public class ZitadelIntrospectTokenValidator
{
    private readonly IConfiguration _configuration;
    private static Dictionary<string, string> _apiPrivateKeyFile = new();

    private readonly string _zitadelDomain;
    private readonly string _zitadelIntrospectionUrl;
    private readonly string _apiPrivateKeyFilePath;

    public ZitadelIntrospectTokenValidator(IConfiguration configuration)
    {
        _configuration = configuration;
        _zitadelDomain = "https://connect-idp-service-bmr36j.us1.zitadel.cloud";
        _zitadelIntrospectionUrl = "https://connect-idp-service-bmr36j.us1.zitadel.cloud/oauth/v2/introspect";
        _apiPrivateKeyFilePath = _configuration["key"];

        LoadApiPrivateKey(_configuration);
    }

    private static void LoadApiPrivateKey(IConfiguration configuration)
    {
        _apiPrivateKeyFile["client_id"] = configuration["clientId"];
        _apiPrivateKeyFile["key_id"] = configuration["keyId"];
        _apiPrivateKeyFile["private_key"] = configuration["key"];
    }

    public async Task<Dictionary<string, object>> IntrospectTokenAsync(string tokenString)
    {
        // Create JWT for client assertion
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Iss, _apiPrivateKeyFile["client_id"]),
            new Claim(JwtRegisteredClaimNames.Sub, _apiPrivateKeyFile["client_id"]),
            new Claim(JwtRegisteredClaimNames.Aud, _zitadelDomain),
            new Claim(JwtRegisteredClaimNames.Exp, new DateTimeOffset(DateTime.UtcNow.AddHours(1)).ToUnixTimeSeconds().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString())
        };

        // Create RSA from private key
        var rsa = RSA.Create();
        rsa.ImportFromPem(_apiPrivateKeyFile["private_key"].ToCharArray());

        // Create signing credentials using RSA key
        var credentials = new SigningCredentials(new RsaSecurityKey(rsa), SecurityAlgorithms.RsaSha256)
        {
            CryptoProviderFactory = new CryptoProviderFactory { CacheSignatureProviders = false }
        };

        // Construct JwtPayload using the claims
        var jwtPayload = new JwtPayload(
            issuer: _apiPrivateKeyFile["client_id"],
            audience: _zitadelDomain,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddHours(1)
        );

        // Create JwtHeader and add the "kid" claim
        var jwtHeader = new JwtHeader(credentials)
        {{ "kid", _apiPrivateKeyFile["key_id"] }};

        // Create JwtSecurityToken using header and payload
        var jwtToken = new JwtSecurityToken(jwtHeader, jwtPayload);

        // Generate the JWT token string
        var jwtHandler = new JwtSecurityTokenHandler();
        string jwtTokenString = jwtHandler.WriteToken(jwtToken);

        // Output JWT token string (for testing)
        Console.WriteLine(jwtTokenString);

        // Send introspection request
        using var client = new HttpClient();
        var content = new FormUrlEncodedContent(new[]
        {
        new KeyValuePair<string, string>("client_assertion_type", "urn:ietf:params:oauth:client-assertion-type:jwt-bearer"),
        new KeyValuePair<string, string>("client_assertion", jwtTokenString),
        new KeyValuePair<string, string>("token", tokenString)
    });

        var response = await client.PostAsync(_zitadelIntrospectionUrl, content);
        response.EnsureSuccessStatusCode();

        var tokenData = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Token data from introspection: {tokenData}");

        return JsonSerializer.Deserialize<Dictionary<string, object>>(tokenData);
    }

    private static bool MatchTokenScopes(Dictionary<string, object> token, string[] requiredScopes)
    {
        if (requiredScopes == null) return true;

        var tokenScopes = token.ContainsKey("scope") ? token["scope"].ToString().Split() : Array.Empty<string>();
        foreach (var andScope in requiredScopes)
        {
            if (Array.Exists(tokenScopes, scope => scope == andScope)) return true;
        }
        return false;
    }

    private static void ValidateToken(Dictionary<string, object> token, string[] requiredScopes)
    {
        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        if (token == null || !token.ContainsKey("active") || !(token["active"] is bool active && active))
        {
            // Handle token inactive or null cases
            throw new ValidatorError(new Dictionary<string, string>
    {
        { "code", "invalid_token_inactive" },
        { "description", "Token is inactive." }
    }, 401);
        }

        if (token.ContainsKey("exp") && (long)token["exp"] < now)
        {
            throw new ValidatorError(new Dictionary<string, string>
        {
            { "code", "invalid_token_expired" },
            { "description", "Token has expired." }
        }, 401);
        }

        if (!MatchTokenScopes(token, requiredScopes))
        {
            throw new ValidatorError(new Dictionary<string, string>
        {
            { "code", "insufficient_scope" },
            { "description", $"Token has insufficient scope. Route requires: {string.Join(", ", requiredScopes)}" }
        }, 401);
        }
    }

    public async Task<Dictionary<string, object>> ValidateTokenAsync(string tokenString, string[] requiredScopes)
    {
        var token = await IntrospectTokenAsync(tokenString);
        ValidateToken(token, requiredScopes);
        return token;
    }
}
