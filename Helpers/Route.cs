namespace AsopropesmarApp.Helpers
{
    public static class Route
    {
        public static class Inicio
        {
            public const string Bienvenida = "/";
        }

        public static class Cuenta
        {
            public static class Autenticacion
            {
                public const string LoggedOut = "/Cuenta/Autenticacion/LoggedOut";
            }
        }
    }
}
