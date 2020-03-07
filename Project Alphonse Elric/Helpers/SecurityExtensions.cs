using Helpers;
using System.Collections.Generic;
using System.Linq;
using Windows.ApplicationModel.Resources;
using Windows.Security.Credentials;
using Windows.Storage;

namespace Project_Alphonse_Elric.Helpers
{
    /// <summary>
    /// SecurityExtensions class
    /// </summary>
    internal static class SecurityExtensions
    {
        internal static readonly string MESSAGE = string.Format("Per motivi di sicurezza, {0} deve verificare l'identità dell'utente.", ResourceLoader.GetForCurrentView().GetString("AppName"));

        /// <summary>
        /// A method that de-registers the HelloAuthenticationEnabled key to disable Windows Hello.
        /// </summary>
        internal static void DisableWindowsHello()
        {
            if (ApplicationData.Current.LocalSettings.Values.ContainsKey("HelloAuthenticationEnabled"))
            {
                ApplicationData.Current.LocalSettings.Values.Remove("HelloAuthenticationEnabled");
            }
        }

        /// <summary>
        /// Method that registers the HelloAuthenticationEnabled key to enable Windows Hello.
        /// </summary>
        internal static void EnableWindowsHello()
        {
            if (!ApplicationData.Current.LocalSettings.Values.ContainsKey("HelloAuthenticationEnabled"))
            {
                ApplicationData.Current.LocalSettings.Values.Add("HelloAuthenticationEnabled", true);
            }
        }

        /// <summary>
        /// Method which returns the credentials stored in the Windows vault by the application.
        /// </summary>
        /// <returns>User's credentials</returns>
        internal static PasswordCredential RetrieveCredentials()
        {
            IReadOnlyList<PasswordCredential> credentialList = new PasswordVault().RetrieveAll();
            foreach (PasswordCredential credential in credentialList.Where(item => item.Resource.Equals(ResourceLoader.GetForCurrentView().GetString("AppName"))))
            {
                return credential;
            }

            return null;
        }

        /// <summary>
        /// Method which removes the credentials from the Windows vault stored by the application.
        /// </summary>
        internal static void RemoveCredentials()
        {
            BackgroundTaskExtensions.Unregister();

            DisableWindowsHello();

            PasswordCredential credential = RetrieveCredentials();
            if (credential != null)
            {
                credential.RetrievePassword();

                new PasswordVault().Remove(new PasswordCredential(ResourceLoader.GetForCurrentView().GetString("AppName"), credential.UserName, credential.Password));
            }
        }

        /// <summary>
        /// Method which stores the credentials into the Windows vault.
        /// </summary>
        /// <param name="username">The username</param>
        /// <param name="password">The password</param>
        internal static void AddCredentials(string username, string password)
        {
            new PasswordVault().Add(new PasswordCredential(ResourceLoader.GetForCurrentView().GetString("AppName"), username, password));

            BackgroundTaskExtensions.Register();
        }
    }
}
