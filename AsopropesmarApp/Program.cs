using AsopropesmarApp.Components;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Security.Claims;
using IdentityModel;
using AsopropesmarApp.Helpers;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// Add services to the container.
builder.Services.AddHttpContextAccessor();
builder.Services.AddRazorPages();
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddCascadingAuthenticationState();

// Register the custom HTTP handler
builder.Services.AddTransient<HttpCustomHandler>();

// Configure an HttpClient to use the custom handler (optional, for making outgoing API calls)
builder.Services.AddHttpClient("api").AddHttpMessageHandler<HttpCustomHandler>();

// Clear default inbound claim mapping to avoid renaming claims (e.g., sub -> nameidentifier)
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
    options.Authority = config["Url:Identity.API"]; // "https://auth.infor-business.com/"

    options.ClientId = config["ClientId"];
    options.ClientSecret = config["Secret"];

    // Mapping custom claims
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

    options.Events.OnTicketReceived = context =>
    {
        if (context.Properties != null)
        {
            context.Properties.ExpiresUtc = DateTime.UtcNow.AddHours(1);
        }
        return Task.CompletedTask;
    };

    options.Events.OnUserInformationReceived = eventArgs =>
    {
        var accessToken = eventArgs.ProtocolMessage.AccessToken;
        if (eventArgs.Principal?.Identities != null)
        {
            foreach (ClaimsIdentity item in eventArgs.Principal.Identities)
            {
                if (!string.IsNullOrEmpty(accessToken))
                {
                    item.AddClaim(new Claim("access_token", accessToken));
                }
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
