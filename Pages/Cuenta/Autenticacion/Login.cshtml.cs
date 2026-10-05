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
                redirectUri = Url.Content($"~{AsopropesmarApp.Helpers.Route.Inicio.Bienvenida}");
            }

            if (Url.IsLocalUrl(redirectUri) == false && redirectUri.StartsWith("/") == false)
            {
                redirectUri = Url.Content($"~{AsopropesmarApp.Helpers.Route.Inicio.Bienvenida}");
            }

            if (HttpContext.User.Identity?.IsAuthenticated == true)
            {
                return Redirect(redirectUri);
            }

            return Challenge(
                new AuthenticationProperties { RedirectUri = redirectUri },
                OpenIdConnectDefaults.AuthenticationScheme);
        }
    }
}
