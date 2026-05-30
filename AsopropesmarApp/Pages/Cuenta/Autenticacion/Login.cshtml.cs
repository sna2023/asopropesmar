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

            if (HttpContext.User.Identity != null && HttpContext.User.Identity.IsAuthenticated)
            {
                if (Url.IsLocalUrl(redirectUri))
                {
                    return Redirect(redirectUri);
                }
                return Redirect(Url.Content($"~{Helpers.Route.Inicio.Bienvenida}"));
            }

            return Challenge(
                new AuthenticationProperties
                {
                    RedirectUri = Url.IsLocalUrl(redirectUri) ? redirectUri : Url.Content($"~{Helpers.Route.Inicio.Bienvenida}")
                },
                OpenIdConnectDefaults.AuthenticationScheme);
        }
    }
}
