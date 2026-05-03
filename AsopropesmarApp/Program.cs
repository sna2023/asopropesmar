using AsopropesmarApp.Components;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

builder.Services.AddHttpContextAccessor();
builder.Services.AddRazorPages();

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

    options.SignedOutRedirectUri = "/Cuenta/Autenticacion/LoggedOut";

    options.Events.OnUserInformationReceived = context =>
    {
        var accessToken = context.ProtocolMessage.AccessToken;
        if (!string.IsNullOrEmpty(accessToken))
        {
            foreach (var identity in context.Principal.Identities)
            {
                identity.AddClaim(new Claim("access_token", accessToken));
            }
        }
        return Task.CompletedTask;
    };
});

builder.Services.AddCascadingAuthenticationState();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

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

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapStaticAssets();

app.MapRazorPages();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
