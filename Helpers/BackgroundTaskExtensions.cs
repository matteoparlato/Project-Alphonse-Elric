using Microsoft.AppCenter.Analytics;
using Microsoft.Toolkit.Uwp.Helpers;
using System;
using System.Collections.Generic;
using Windows.ApplicationModel.Background;

namespace Helpers
{
    /// <summary>
    /// BackgroundTaskExtensions class
    /// </summary>
    public static class BackgroundTaskExtensions
    {
        private const string TASK_NAME = "UpdaterTask";
        private const string ENTRY_POINT = "BackgroundTasks.UpdaterTask";

        /// <summary>
        /// Method which registers a new background task for the app.
        /// Registers a new backgroud task which will be triggered every 15 minutes.
        /// </summary>
        public static void Register()
        {
            Unregister();
            try
            {
               BackgroundTaskHelper.Register(TASK_NAME, ENTRY_POINT, new TimeTrigger(15, false), true, true);
            }
            catch(Exception ex)
            {
                Analytics.TrackEvent(ex.Message, new Dictionary<string, string> { { "exception", ex.ToString() } });
            }
        }

        /// <summary>
        /// Method which unregisters the TASK_NAME task of the app.
        /// </summary>
        public static void Unregister()
        {
            BackgroundTaskHelper.Unregister(TASK_NAME);
        }
    }
}
