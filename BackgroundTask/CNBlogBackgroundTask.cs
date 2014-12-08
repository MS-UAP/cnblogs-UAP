
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Windows.ApplicationModel.Background;
using Windows.UI.Notifications;
using NotificationsExtensions.TileContent;
using CNBlogs.DataHelper;
using CNBlogs.DataHelper.Helper;

namespace BackgroundTask
{
    public sealed class CNBlogBackgroundTask : IBackgroundTask
    {
        // Summary:
        //     Performs the work of a background task. The system calls this method when
        //     the associated background task has been triggered.
        //
        // Parameters:
        //   taskInstance:
        //     An interface to an instance of the background task. The system creates this
        //     instance when the task has been triggered to run.
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            BackgroundTaskDeferral _deferral = taskInstance.GetDeferral();

            try
            {
                // set first post of first page, this is the latest one
                var result = await APIWrapper.Instance.GetHomePostsAsync(1, 1);

                if (result.IsSuccess)
                {
                    var feed = result.Result;

                    if (feed != null && feed.Entries != null && feed.Entries.Count > 0)
                    {
                        Functions.CreateTile(feed.Entries.First());
                    }
                }

            }
            catch
            {
            }
            finally
            {
                _deferral.Complete();
            }
        }

    }
}
