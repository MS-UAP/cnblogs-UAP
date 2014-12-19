using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

namespace CNBlogs.DataHelper.Helper
{
    public class BackgroundTaskHelper
    {
        static string taskName = "BackgroundTask";
        static string taskEntryPoint = "BackgroundTask.CNBlogBackgroundTask";

        // TODO: only register when the task not exist
        public async static Task<bool> Register(Action action = null)
        {
            try
            {
                UnRegister();

                // do the registeration
                // check access permission
                BackgroundAccessStatus status = await BackgroundExecutionManager.RequestAccessAsync();
                switch (status)
                {
                    case BackgroundAccessStatus.Denied: // reach maxmium number, or, disabled by user
                        return false;
                    case BackgroundAccessStatus.Unspecified:
                        return false;
                    case BackgroundAccessStatus.AllowedMayUseActiveRealTimeConnectivity:
                        break;
                }

                // register the task in the next step.
                BackgroundTaskBuilder builder = new BackgroundTaskBuilder();
                builder.Name = taskName;
                builder.TaskEntryPoint = taskEntryPoint;
                builder.SetTrigger(new TimeTrigger(15, false)); // run every 15 minutes

                var registration = builder.Register();

                if (registration != null && action != null)
                {
                    registration.Completed += (s, a) =>
                    {
                        action();
                    };
                }


                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool UnRegister()
        {
            try
            {
                BackgroundTaskRegistration task = null;
                // Check for existing registrations of this background task.
                foreach (var cur in BackgroundTaskRegistration.AllTasks)
                {
                    if (cur.Value.Name == taskName)
                    {
                        // The task is already registered.
                        task = (BackgroundTaskRegistration)(cur.Value);
                        break;
                    }
                }

                if (task != null)
                {
                    task.Unregister(false);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}
