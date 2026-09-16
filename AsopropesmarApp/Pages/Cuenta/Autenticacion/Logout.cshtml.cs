using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AsopropesmarApp.Pages.Cuenta.Autenticacion
{
    public class LogoutModel : PageModel
    {
        public IActionResult OnGet()
        {
            return SignOut(
                new AuthenticationProperties { RedirectUri = "/Cuenta/Autenticacion/LoggedOut" },
                CookieAuthenticationDefaults.AuthenticationScheme,
                OpenIdConnectDefaults.AuthenticationScheme);
        }
    }
}
