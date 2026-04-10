using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using IdentityModel;
using AsopropesmarApp.Components;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

builder.Services.AddHttpContextAccessor();
builder.Services.AddRazorPages();
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

    options.SaveTokens = true;
    options.ResponseType = "code";
    options.UsePkce = true;
    options.GetClaimsFromUserInfoEndpoint = true;

    options.Scope.Add("openid");
    options.Scope.Add("Administracion.API");
    options.Scope.Add("SRI.API");

    options.SignedOutRedirectUri = "/Cuenta/Autenticacion/LoggedOut";

    options.Events.OnUserInformationReceived = eventArgs =>
    {
        var accessToken = eventArgs.ProtocolMessage.AccessToken;
        if (accessToken != null && eventArgs.Principal?.Identities != null)
        {
            foreach (var item in eventArgs.Principal.Identities)
            {
                item.AddClaim(new Claim("access_token", accessToken));
            }
        }
        return Task.CompletedTask;
    };
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();    // ANTES de UseAuthorization
app.UseAuthorization();
app.MapControllers();
app.MapRazorPages();
app.UseAntiforgery();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
