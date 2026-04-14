using AsopropesmarApp.Components;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using IdentityModel;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

builder.Services.AddHttpContextAccessor();
builder.Services.AddRazorPages();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Evitar renombrado de claims
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
{
    options.SlidingExpiration = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(15);
})
.AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
{
    options.Authority = config["Url:Identity.API"];     // "https://auth.infor-business.com/"
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

    options.SaveTokens = true;
    options.ResponseType = "code";
    options.UsePkce = true;
    options.GetClaimsFromUserInfoEndpoint = true;

    options.Scope.Add("openid");
    options.Scope.Add("Administracion.API");
    options.Scope.Add("SRI.API");

    options.SignedOutRedirectUri = "/Cuenta/Autenticacion/LoggedOut";

    // Opcional: agregar access_token como claim
    options.Events.OnUserInformationReceived += eventArgs =>
    {
        var accessToken = eventArgs.ProtocolMessage.AccessToken;
        foreach (ClaimsIdentity item in eventArgs.Principal.Identities)
            item.AddClaim(new Claim("access_token", accessToken));
        return Task.CompletedTask;
    };

    options.Events.OnTicketReceived = async (context) =>
    {
        // Establecer la expiración del ticket de autenticación a 1 hora
        context.Properties.ExpiresUtc = DateTime.UtcNow.AddHours(1);
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
