1. Initialize the new project 'AsopropesmarApp' if it doesn't exist, as a Blazor Web App targeting .NET 8.0, and configure it for Server interactivity.
2. Add necessary NuGet packages for OIDC authentication: `Microsoft.AspNetCore.Authentication.OpenIdConnect` (v8.0.0) and `IdentityModel` (v7.0.0).
3. Setup `appsettings.json` with the identity URL, ClientId, and Secret, providing placeholders for the client to complete.
4. Implement the authentication flow components:
   - Create `Login.cshtml` and `Login.cshtml.cs` under `Pages/Cuenta/Autenticacion` to handle OIDC Challenge redirect.
   - Create `Logout.cshtml` and `Logout.cshtml.cs` under `Pages/Cuenta/Autenticacion` to handle local and IdP SignOut.
   - Create `LoggedOut.razor` under `Pages/Cuenta/Autenticacion` to display a post-logout message.
5. Create a `Route.cs` helper class in `Helpers` to store constant route paths for consistency.
6. Create an `HttpCustomHandler.cs` under `Helpers` to attach the retrieved access token automatically to HTTP requests.
7. Configure `Program.cs` to add authentication services with `Cookie` and `OpenIdConnect` default schemes. Setup options for `Authority`, scopes, mapping custom claims with `JwtClaimTypes`, enabling PKCE, and handling events `OnTicketReceived` and `OnUserInformationReceived`. Ensure proper pipeline order `UseRouting()`, `UseAuthentication()`, `UseAuthorization()`.
8. Update `App.razor`, `Routes.razor`, and `MainLayout.razor` inside `Components` to integrate `CascadingAuthenticationState` and `AuthorizeRouteView`, verifying the user's authentication state and handling navigation to the unauthenticated view (`LoggedOut.razor`).
9. Complete pre commit steps to verify the app builds and there are no compilation errors.
