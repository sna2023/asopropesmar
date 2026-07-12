using AsopropesmarApp.Components;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using IdentityModel;
using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// Add services to the container.
builder.Services.AddHttpContextAccessor();
builder.Services.AddRazorPages();
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddCascadingAuthenticationState();

builder.Services.AddTransient<AsopropesmarApp.Helpers.HttpCustomHandler>();
builder.Services.AddHttpClient("api").AddHttpMessageHandler<AsopropesmarApp.Helpers.HttpCustomHandler>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;       // "Cookies"
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;     // "OpenIdConnect"
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
{
    options.SlidingExpiration = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(15);
})
.AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
{
    // 1. Servidor de identidad (Authority)
    options.Authority = config["Url:Identity.API"];

    // 2. Credenciales del cliente
    options.ClientId = config["ClientId"];
    options.ClientSecret = config["Secret"];

    // 3. Mapeo de claims personalizados desde el IdP
    options.ClaimActions.MapUniqueJsonKey(JwtClaimTypes.Name, JwtClaimTypes.Name);
    options.ClaimActions.MapUniqueJsonKey(JwtClaimTypes.NickName, JwtClaimTypes.NickName);
    options.ClaimActions.MapUniqueJsonKey(JwtClaimTypes.GivenName, JwtClaimTypes.GivenName);
    options.ClaimActions.MapUniqueJsonKey(JwtClaimTypes.FamilyName, JwtClaimTypes.FamilyName);
    options.ClaimActions.MapUniqueJsonKey(JwtClaimTypes.Profile, JwtClaimTypes.Profile);
    options.ClaimActions.MapUniqueJsonKey("setting", "setting");
    options.ClaimActions.MapUniqueJsonKey("profile", "profile");

    // 4. Guardar tokens (access_token, id_token, refresh_token) en la cookie
    options.SaveTokens = true;

    // 5. Flujo Authorization Code
    options.ResponseType = "code";

    // 6. Scopes solicitados
    options.Scope.Add("openid");                // Obligatorio para OIDC
    options.Scope.Add("Administracion.API");    // Acceso a la API de Administración
    options.Scope.Add("SRI.API");               // Acceso a la API del SRI

    // 7. URL de redirección post-logout
    options.SignedOutRedirectUri = "/Cuenta/Autenticacion/LoggedOut";

    // 8. Obtener claims adicionales del endpoint UserInfo
    options.GetClaimsFromUserInfoEndpoint = true;

    // 9. Habilitar PKCE (Proof Key for Code Exchange)
    options.UsePkce = true;

    options.MapInboundClaims = false;

    // 10. Eventos personalizados
    options.Events.OnTicketReceived = context =>
    {
        // Establecer la expiración del ticket de autenticación a 1 hora
        if (context.Properties != null)
        {
            context.Properties.ExpiresUtc = DateTime.UtcNow.AddHours(1);
        }
        return Task.CompletedTask;
    };

    options.Events.OnUserInformationReceived = eventArgs =>
    {
        // Extraer el access_token y agregarlo como claim del usuario
        var accessToken = eventArgs.ProtocolMessage.AccessToken;
        if (eventArgs.Principal?.Identities != null)
        {
            foreach (ClaimsIdentity item in eventArgs.Principal.Identities)
                item.AddClaim(new Claim("access_token", accessToken ?? string.Empty));
        }
        return Task.CompletedTask;
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapRazorPages();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
