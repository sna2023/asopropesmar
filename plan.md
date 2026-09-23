1. Initialize the project as a Blazor Web App targeting .NET 10 with Server Interactivity.
2. Add necessary NuGet packages (`Microsoft.AspNetCore.Authentication.OpenIdConnect`, `IdentityModel`, `IdentityServer4`).
3. Set up the routes helper (`Helpers/Route.cs`) and HTTP handler (`Helpers/HttpCustomHandler.cs`).
4. Configure OIDC authentication in `Program.cs` according to the provided instructions.
5. Setup user configuration in `appsettings.json` with the provided OIDC credentials.
6. Restructure layout and app components (`App.razor`, `Routes.razor`, `MainLayout.razor`, `NavMenu.razor`).
7. Create authentication endpoints (`Login.cshtml`, `Logout.cshtml`, `LoggedOut.razor`).
8. Migrate static assets (images, css, js, index.html contents) to Blazor components (specifically `Home.razor` and `Bienvenida.razor`).
9. Build and verify the application.
