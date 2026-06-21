using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace AsopropesmarApp.Components.Layout
{
    public partial class MainLayout
    {
        [Inject]
        protected NavigationManager _navigation { get; set; }

        [CascadingParameter]
        private Task<AuthenticationState> AuthStat { get; set; }

        protected async override Task OnInitializedAsync()
        {
            var authState = await AuthStat;
            var user = authState?.User;

            if (user is null || user.Identity is null || !user.Identity.IsAuthenticated)
            {
                var currentUrl = new Uri(_navigation.Uri).AbsolutePath;
                if (!currentUrl.Equals(AsopropesmarApp.Helpers.Route.Cuenta.Autenticacion.LoggedOut, StringComparison.OrdinalIgnoreCase))
                {
                    _navigation.NavigateTo(AsopropesmarApp.Helpers.Route.Cuenta.Autenticacion.LoggedOut, forceLoad: true);
                }
            }
        }
    }
}
