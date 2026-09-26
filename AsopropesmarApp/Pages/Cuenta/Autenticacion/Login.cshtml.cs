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
                redirectUri = Url.Content("~/inicio/");
            }

            if (HttpContext.User.Identity?.IsAuthenticated == true)
            {
                if (Url.IsLocalUrl(redirectUri))
                {
                    return Redirect(redirectUri);
                }
                return Redirect("~/inicio/");
            }

            return Challenge(
                new AuthenticationProperties { RedirectUri = redirectUri },
                OpenIdConnectDefaults.AuthenticationScheme);
        }
    }
}
