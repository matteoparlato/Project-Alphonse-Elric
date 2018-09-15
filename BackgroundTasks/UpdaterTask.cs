using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.ApplicationModel.Background;
using Windows.Security.Credentials;

namespace BackgroundTasks
{
    /// <summary>
    /// UpdaterTask class
    /// </summary>
    public sealed class UpdaterTask : IBackgroundTask
    {
        /// <summary>
        /// Performs the work of a background task.
        /// The system calls this method when the associated background task has been triggered.
        /// </summary>
        /// <param name="taskInstance">The interface to an instance of the background task.</param>
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            taskInstance.GetDeferral();

            Update(taskInstance);
        }

        /// <summary>
        /// Method which performs an update of the live tile with the latest information.
        /// </summary>
        /// <param name="taskInstance">The interface to an instance of the background task.</param>
        private async static void Update(IBackgroundTaskInstance taskInstance)
        {
            BackgroundTaskDeferral deferral = taskInstance.GetDeferral();

            IReadOnlyList<PasswordCredential> credentialList = new PasswordVault().RetrieveAll();
            foreach (PasswordCredential credential in credentialList.Where(item => item.Resource.Equals("Iliad X")))
            {
                credential.RetrievePassword();

                try
                {
                    await ClientExtensions.Authenticate(credential.UserName, credential.Password);

                    NotificationExtensions.SendTileNotification(ClientExtensions.AccountDetails);
                }
                catch (Exception)
                {
                    //
                }
            }

            deferral.Complete();
        }
    }
}
