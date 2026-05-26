# Configuración OIDC para AsopropesmarApp

El proceso de autenticación OIDC hacia `https://auth.infor-business.com/` se ha implementado. Para que el proyecto funcione correctamente, necesitas registrar esta aplicación como un cliente en el IdentityServer y actualizar las credenciales locales.

## Pasos a Seguir

1. **Registrar un nuevo cliente en `https://auth.infor-business.com/`**:
   - `ClientId`: Genera un identificador único para esta aplicación.
   - `ClientSecret`: Genera una contraseña segura.
   - `AllowedGrantTypes`: Configura como `Code` (Authorization Code).
   - `RequirePkce`: Configúralo en `true`.
   - `RedirectUris`: Agrega `https://{tu-dominio}/signin-oidc` (Por ejemplo, para entorno de desarrollo local: `https://localhost:7123/signin-oidc` o la URL que asigne Kestrel).
   - `PostLogoutRedirectUris`: Agrega `https://{tu-dominio}/Cuenta/Autenticacion/LoggedOut`.
   - `AllowedScopes`: Agrega al menos `openid`, `Administracion.API`, y `SRI.API`.

2. **Actualizar el archivo `appsettings.json` localmente**:
   - Reemplaza `"TU-CLIENT-ID"` con el `ClientId` que creaste.
   - Reemplaza `"TU-CLIENT-SECRET"` con el `ClientSecret` que generaste.

   ```json
   "ClientId": "TU-CLIENT-ID",
   "Secret": "TU-CLIENT-SECRET"
   ```

Una vez que completes estos pasos, el flujo de inicio de sesión, el intercambio de tokens con PKCE, y la finalización de sesión (Logout) interactuarán correctamente con el Identity Provider.
