using AsopropesmarApp.Components;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using IdentityModel;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Security.Claims;
using AsopropesmarApp.Helpers;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// Disable inbound claim mapping
JsonWebTokenHandler.DefaultInboundClaimTypeMap.Clear();

builder.Services.AddHttpContextAccessor();

// Add HttpCustomHandler to services
builder.Services.AddTransient<HttpCustomHandler>();

// Add Razor pages for Login and Logout
builder.Services.AddRazorPages();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add CascadingAuthenticationState for AuthorizeView in components
builder.Services.AddCascadingAuthenticationState();

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
    options.Authority = config["Url:Identity.API"];
    options.ClientId = config["ClientId"];
    options.ClientSecret = config["Secret"];

    options.ClaimActions.MapUniqueJsonKey(JwtClaimTypes.Name, JwtClaimTypes.Name);
    options.ClaimActions.MapUniqueJsonKey(JwtClaimTypes.NickName, JwtClaimTypes.NickName);
    options.ClaimActions.MapUniqueJsonKey(JwtClaimTypes.GivenName, JwtClaimTypes.GivenName);
    options.ClaimActions.MapUniqueJsonKey(JwtClaimTypes.FamilyName, JwtClaimTypes.FamilyName);
    options.ClaimActions.MapUniqueJsonKey(JwtClaimTypes.Profile, JwtClaimTypes.Profile);
    options.ClaimActions.MapUniqueJsonKey("setting", "setting");
    options.ClaimActions.MapUniqueJsonKey("profile", "profile");

    options.SaveTokens = true;
    options.ResponseType = "code";

    options.Scope.Add("openid");
    options.Scope.Add("Administracion.API");
    options.Scope.Add("SRI.API");

    options.SignedOutRedirectUri = "/Cuenta/Autenticacion/LoggedOut";
    options.GetClaimsFromUserInfoEndpoint = true;
    options.UsePkce = true;

    options.Events.OnTicketReceived = async (context) =>
    {
        if (context.Properties != null)
        {
            context.Properties.ExpiresUtc = DateTime.UtcNow.AddHours(1);
        }
    };

    options.Events.OnUserInformationReceived += eventArgs =>
    {
        var accessToken = eventArgs.ProtocolMessage.AccessToken;
        if (eventArgs.Principal?.Identities != null)
        {
            foreach (ClaimsIdentity item in eventArgs.Principal.Identities)
            {
                item.AddClaim(new Claim("access_token", accessToken ?? string.Empty));
            }
        }
        return Task.CompletedTask;
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

// Authentication and Authorization
app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapRazorPages(); // Map razor pages for login/logout

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
