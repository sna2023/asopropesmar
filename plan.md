1. **Add NuGet Dependencies**: Install `Microsoft.AspNetCore.Authentication.OpenIdConnect` (v10.0.x) and `IdentityModel` (v7.0.0) in the `AsopropesmarApp` project.
2. **Setup AppSettings**: Add OIDC configuration properties (Url, ClientId, Secret) with placeholders in `appsettings.json`.
3. **Create Helpers**:
   - `Helpers/Route.cs` with route constants (`Login`, `Logout`, `LoggedOut`, `Bienvenida`).
   - `Helpers/HttpCustomHandler.cs` (DelegatingHandler to append `access_token` to HTTP requests, checking for nulls).
4. **Setup Razor Pages for Authentication**:
   - Create `Pages/Cuenta/Autenticacion/Login.cshtml` and `Login.cshtml.cs` using `Challenge()`.
   - Create `Pages/Cuenta/Autenticacion/Logout.cshtml` and `Logout.cshtml.cs` using `SignOut()`.
5. **Setup Blazor Components**:
   - Create `Components/Pages/Cuenta/Autenticacion/LoggedOut.razor`.
   - Update `Components/Routes.razor` to use `<CascadingAuthenticationState>` and `<AuthorizeRouteView>`.
   - Update `_Imports.razor` to include `Microsoft.AspNetCore.Components.Authorization`.
   - Modify `Components/Layout/MainLayout.razor` and `NavMenu.razor` to use `<AuthorizeView>`, securing the UI and providing Login/Logout links (with `data-enhance-nav="false"`).
6. **Configure `Program.cs`**:
   - Register `HttpContextAccessor`, `RazorPages`, `HttpClient` (with `HttpCustomHandler`), and `CascadingAuthenticationState`.
   - Configure Authentication with Cookies and OIDC.
   - Use `MapInboundClaims = false`, configure requested scopes, map claims, and handle `OnUserInformationReceived` (with null checks) & `OnTicketReceived`.
   - Setup correct middleware order: Routing -> AuthN -> AuthZ -> Antiforgery -> MapRazorPages -> MapRazorComponents.
7. **Migrate Frontend Assets**: Move CSS/JS/Images to `wwwroot` and adapt `index.html` content into `Components/Pages/Home.razor` and `Components/Layout/MainLayout.razor` for Blazor routing, linking the custom Login button to the real OIDC flow.
8. **Git Ignore**: Add `.gitignore` to exclude `bin/`, `obj/`, and `wwwroot/lib/`.
9. **Pre-commit Verification**: Run tests, check for compilation errors, and complete pre-commit steps to ensure proper testing, verification, review, and reflection are done.
10. **Submit**: Provide instructions for the user on how to configure the OIDC secrets manually, and submit the changes.
