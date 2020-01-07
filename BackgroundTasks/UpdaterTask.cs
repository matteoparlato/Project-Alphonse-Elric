using Helpers;
using Project_Alphonse_Elric.Core.Helpers;
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
        /// <param name="taskInstance">The interface to an instance of the background task</param>
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            taskInstance.GetDeferral();

            Update(taskInstance);
        }

        /// <summary>
        /// Method which performs the work of the background task.
        /// </summary>
        /// <param name="taskInstance">The interface to an instance of the background task</param>
        private async static void Update(IBackgroundTaskInstance taskInstance)
        {
            BackgroundTaskDeferral deferral = taskInstance.GetDeferral();

            IReadOnlyList<PasswordCredential> credentialList = new PasswordVault().RetrieveAll();
            foreach (PasswordCredential credential in credentialList.Where(item => item.Resource.Equals("Area personale")))
            {
                credential.RetrievePassword();

                try
                {
                    await Singleton<ClientExtensions>.Instance.Authenticate(credential.UserName, credential.Password);

                    NotificationExtensions.SendTileNotification(Singleton<ClientExtensions>.Instance.AccountDetails);
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
