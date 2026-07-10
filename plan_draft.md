1. **Prepare static assets and structure:**
   - Copy static files (`css/`, `js/`, `.jpg`, `.jpeg`) to `AsopropesmarApp/wwwroot/`.
   - Update `.gitignore` to ignore `bin/` and `obj/`.

2. **Install necessary NuGet packages:**
   - Add `Microsoft.AspNetCore.Authentication.OpenIdConnect` (version 8.0.*).
   - Add `IdentityModel` (version 7.0.0).
   - Add `IdentityServer4` (version 4.1.2) if needed, though might be unnecessary for client, will add as requested by instructions.

3. **Configure AppSettings:**
   - Update `appsettings.json` with the required OIDC configuration (Url:Identity.API, ClientId, Secret placeholders).

4. **Create Helper Classes:**
   - Create `AsopropesmarApp/Helpers/Route.cs` for route constants.
   - Create `AsopropesmarApp/Helpers/HttpCustomHandler.cs` (DelegatingHandler for tokens).

5. **Create Razor Pages for Auth Flows (Login/Logout):**
   - Create `Pages/Cuenta/Autenticacion/Login.cshtml` and `Login.cshtml.cs`.
   - Create `Pages/Cuenta/Autenticacion/Logout.cshtml` and `Logout.cshtml.cs`.
   - Implement `Challenge` and `SignOut` respectively, validating local return URLs and using correct namespaces.

6. **Create Blazor Components for Auth:**
   - Create `Components/Pages/Cuenta/Autenticacion/LoggedOut.razor`.
   - Modify `Components/Routes.razor` and `Components/App.razor` to include `CascadingAuthenticationState` and `AuthorizeRouteView`.

7. **Migrate index.html to Blazor:**
   - Move the structure to `Components/Layout/MainLayout.razor`, integrating `<AuthorizeView>` based on user instructions.
   - Extract page content into `Components/Pages/Home.razor` (mapped to `/inicio/`).

8. **Update Program.cs Pipeline & Services:**
   - Add authentication, OIDC setup (clearing inbound claims via `JsonWebTokenHandler`, MapUniqueJsonKey, OnUserInformationReceived, OnTicketReceived).
   - Register HttpCustomHandler, HttpClient, CascadingAuthenticationState, Razor Pages.
   - Map Razor Pages (`app.MapRazorPages()`).

9. **Complete pre commit steps**
   - Ensure proper testing, verification, review, and reflection are done by running `pre_commit_instructions` and addressing anything required.

10. **Submit changes**
