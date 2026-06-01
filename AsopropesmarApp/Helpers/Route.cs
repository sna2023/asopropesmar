namespace AsopropesmarApp.Helpers
{
    public static class Route
    {
        public static class Cuenta
        {
            public static class Autenticacion
            {
                public const string LoggedOut = "/Cuenta/Autenticacion/LoggedOut";
                public const string Login = "/Cuenta/Autenticacion/Login";
                public const string Logout = "/Cuenta/Autenticacion/Logout";
            }
        }
        public static class Inicio
        {
            public const string Bienvenida = "/";
        }
    }
}
