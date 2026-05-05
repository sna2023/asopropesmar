using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AsopropesmarApp.Pages.Cuenta.Autenticacion
{
    public class LoginModel : PageModel
    {
        public IActionResult OnGet(string? redirectUri)
        {
            if (string.IsNullOrWhiteSpace(redirectUri))
            {
                redirectUri = Url.Content($"~{Helpers.Route.Inicio.Bienvenida}");
            }

            // Validar para prevenir Open Redirect
            if (!Url.IsLocalUrl(redirectUri))
            {
                redirectUri = Url.Content($"~{Helpers.Route.Inicio.Bienvenida}");
            }

            if (HttpContext.User.Identity?.IsAuthenticated == true)
            {
                return LocalRedirect(redirectUri);
            }

            return Challenge(
                new AuthenticationProperties
                {
                    RedirectUri = redirectUri
                },
                OpenIdConnectDefaults.AuthenticationScheme);
        }
    }
}