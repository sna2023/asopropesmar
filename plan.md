1. **Understand Request**: Setup OpenID Connect authentication to IdentityServer4 at `https://auth.infor-business.com/` with given ClientId and Secret in a Blazor .NET 8 application. Use standard Cookie + OIDC flows. Move current `index.html` UI to Blazor components and ensure unauthenticated access redirects to a login prompt/logged out screen, which uses Razor pages for full browser redirect to OIDC endpoints.

2. **Actions Completed Already**:
    - Created Blazor web app (`dotnet new blazor -interactivity server`).
    - Added packages (`Microsoft.AspNetCore.Authentication.OpenIdConnect`, `IdentityModel`, `IdentityServer4`).
    - Configured `appsettings.json` with identity provider info.
    - Set up constants in `Helpers/Route.cs`.
    - Created `Helpers/HttpCustomHandler.cs`.
    - Configured `Program.cs` for cookie and OIDC authentication.
    - Added `Pages/Cuenta/Autenticacion/Login.cshtml` and `Logout.cshtml` with code behind.
    - Set up `Components/Pages/LoggedOut.razor`.
    - Set up `Components/Routes.razor` with `<CascadingAuthenticationState>` and `AuthorizeRouteView`.
    - Configured `Components/Layout/MainLayout.razor` to check auth state and handle `<AuthorizeView>`.
    - Updated `Components/App.razor` to map to existing index.html structure.
    - Migrated contents of `index.html` and assets into `wwwroot/` and `Components/Pages/Home.razor` appropriately.
    - Fixed compilation issues and tested routing (`test-login.js` and `test-logout.js`).

3. **Remaining Actions**:
    - Call pre commit instructions.
    - Submit changes.
