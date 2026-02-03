using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Infrastructure.Auth;

public sealed class OpenIddictSeeder
{
    private readonly IOpenIddictApplicationManager _apps;
    private readonly IOpenIddictScopeManager _scopes;

    public OpenIddictSeeder(IOpenIddictApplicationManager  apps, IOpenIddictScopeManager scopes)
    {
            _apps = apps;
            _scopes = scopes;
    }

    public async Task SeedAsync(CancellationToken ct = default)
    {
        // -----------------------------
        // 1) SCOPES
        // -----------------------------
        await EnsureScopeAsync(
            name: "api",
            displayName: "FrostHaven API",
            resources: new[] { "frosthaven-api" },
            ct);

        // (Opcional) scopes OIDC estándar si planeas usar OpenID Connect con usuario:
        // openid/profile/email normalmente no se crean como "scopes" aquí,
        // se registran como scopes en la config del servidor, pero puedes dejarlos en config.
        // Aquí normalmente solo guardas scopes de recursos (ej: "api").

        // -----------------------------
        // 2) APPLICATIONS (CLIENTS)
        // -----------------------------

        // A) Client para microservicios (Client Credentials)
        await EnsureConfidentialClientAsync(
            clientId: "frosthaven_services",
            clientSecret: "super-secret-dev", // en prod: Secrets Manager / env var
            displayName: "FrostHaven Services",
            scopes: new[] { "api" },
            ct);

        // B) Client para Angular SPA (Authorization Code + PKCE)
        await EnsureSpaPkceClientAsync(
            clientId: "frosthaven_spa",
            displayName: "FrostHaven SPA",
            redirectUris: new[]
            {
                "http://localhost:4200/auth-callback"
            },
            postLogoutRedirectUris: new[]
            {
                "http://localhost:4200/"
            },
            scopes: new[] { "api" },
            ct);
    }
    
    private async Task EnsureScopeAsync(string name, string displayName, string[] resources, CancellationToken ct)
    {
        if (await _scopes.FindByNameAsync(name, ct) is not null)
            return;

        var descriptor = new OpenIddictScopeDescriptor
        {
            Name = name,
            DisplayName = displayName
        };

        foreach (var r in resources)
            descriptor.Resources.Add(r);

        await _scopes.CreateAsync(descriptor, ct);
    }
    
    private async Task EnsureConfidentialClientAsync(string clientId, string clientSecret, string displayName, string[] scopes, CancellationToken ct)
    {
        if (await _apps.FindByClientIdAsync(clientId, ct) is not null)
            return;

        var descriptor = new OpenIddictApplicationDescriptor
        {
            ClientId = clientId,
            ClientSecret = clientSecret,
            DisplayName = displayName,
            Permissions =
            {
                Permissions.Endpoints.Token,
                Permissions.GrantTypes.ClientCredentials,
                Permissions.Prefixes.Scope + "api"
            }
        };

        foreach (var s in scopes)
            descriptor.Permissions.Add(Permissions.Prefixes.Scope + s);

        await _apps.CreateAsync(descriptor, ct);
    }
    
    private async Task EnsureSpaPkceClientAsync(string clientId, string displayName, string[] redirectUris, string[] postLogoutRedirectUris, string[] scopes, CancellationToken ct)
    {
        if (await _apps.FindByClientIdAsync(clientId, ct) is not null)
            return;

        var descriptor = new OpenIddictApplicationDescriptor
        {
            ClientId = clientId,
            DisplayName = displayName,
            ConsentType = ConsentTypes.Implicit,
            Permissions =
            {
                Permissions.Endpoints.Authorization,
                Permissions.Endpoints.Token,
                Permissions.GrantTypes.AuthorizationCode,
                Permissions.ResponseTypes.Code,
                Permissions.Prefixes.Scope + "api"
            },
            Requirements =
            {
                Requirements.Features.ProofKeyForCodeExchange
            }
        };

        foreach (var uri in redirectUris)
            descriptor.RedirectUris.Add(new Uri(uri));

        foreach (var uri in postLogoutRedirectUris)
            descriptor.PostLogoutRedirectUris.Add(new Uri(uri));

        foreach (var s in scopes)
            descriptor.Permissions.Add(Permissions.Prefixes.Scope + s);

        // Si luego usas OIDC “completo” con userinfo/claims:
        // descriptor.Permissions.Add(Permissions.Scopes.Profile);
        // descriptor.Permissions.Add(Permissions.Scopes.Email);
        // descriptor.Permissions.Add(Permissions.Scopes.Roles);

        await _apps.CreateAsync(descriptor, ct);
    }
}