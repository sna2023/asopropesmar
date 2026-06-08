using AsopropesmarApp.Components;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Security.Claims;
using IdentityModel;

var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration;

// Add services to the container.
builder.Services.AddHttpContextAccessor();
builder.Services.AddRazorPages();
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddCascadingAuthenticationState();

JsonWebTokenHandler.DefaultInboundClaimTypeMap.Clear();

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

    options.SaveTokens = true;
    options.ResponseType = "code";
    options.UsePkce = true;
    options.GetClaimsFromUserInfoEndpoint = true;

    options.Scope.Add("openid");
    options.Scope.Add("Administracion.API");
    options.Scope.Add("SRI.API");

    options.ClaimActions.MapUniqueJsonKey(JwtClaimTypes.Name, JwtClaimTypes.Name);
    options.ClaimActions.MapUniqueJsonKey(JwtClaimTypes.NickName, JwtClaimTypes.NickName);
    options.ClaimActions.MapUniqueJsonKey(JwtClaimTypes.GivenName, JwtClaimTypes.GivenName);
    options.ClaimActions.MapUniqueJsonKey(JwtClaimTypes.FamilyName, JwtClaimTypes.FamilyName);
    options.ClaimActions.MapUniqueJsonKey(JwtClaimTypes.Profile, JwtClaimTypes.Profile);
    options.ClaimActions.MapUniqueJsonKey("setting", "setting");
    options.ClaimActions.MapUniqueJsonKey("profile", "profile");

    options.SignedOutRedirectUri = "/Cuenta/Autenticacion/LoggedOut";

    options.Events.OnTicketReceived = async (context) =>
    {
        if (context.Properties != null)
        {
            context.Properties.ExpiresUtc = DateTime.UtcNow.AddHours(1);
        }
        await Task.CompletedTask;
    };

    options.Events.OnUserInformationReceived = async (eventArgs) =>
    {
        var accessToken = eventArgs.ProtocolMessage.AccessToken;
        if (eventArgs.Principal?.Identities != null && !string.IsNullOrEmpty(accessToken))
        {
            foreach (var identity in eventArgs.Principal.Identities)
            {
                identity.AddClaim(new Claim("access_token", accessToken));
            }
        }
        await Task.CompletedTask;
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
app.UseStatusCodePagesWithReExecute("/not-found");
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
