using System.Security.Claims;
using AsopropesmarApp.Components;
using AsopropesmarApp.Helpers;
using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddHttpContextAccessor();
builder.Services.AddRazorPages();

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
    options.Authority = builder.Configuration["Url:Identity.API"];
    options.ClientId = builder.Configuration["ClientId"];
    options.ClientSecret = builder.Configuration["Secret"];

    options.MapInboundClaims = false;
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

    options.SignedOutRedirectUri = AsopropesmarApp.Helpers.Route.Cuenta.Autenticacion.LoggedOut;
    options.GetClaimsFromUserInfoEndpoint = true;
    options.UsePkce = true;

    options.Events.OnTicketReceived = async (context) =>
    {
        if (context.Properties != null)
        {
            context.Properties.ExpiresUtc = DateTime.UtcNow.AddHours(1);
        }
        await Task.CompletedTask;
    };

    options.Events.OnUserInformationReceived = async eventArgs =>
    {
        var accessToken = eventArgs.ProtocolMessage.AccessToken;
        if (eventArgs.Principal?.Identities != null && !string.IsNullOrEmpty(accessToken))
        {
            foreach (ClaimsIdentity item in eventArgs.Principal.Identities)
            {
                item.AddClaim(new Claim("access_token", accessToken));
            }
        }
        await Task.CompletedTask;
    };
});

builder.Services.AddHttpClient("api").AddHttpMessageHandler<HttpCustomHandler>();
builder.Services.AddTransient<HttpCustomHandler>();


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