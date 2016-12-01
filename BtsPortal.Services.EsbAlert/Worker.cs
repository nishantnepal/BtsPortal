using BtsPortal.Cache;

namespace BtsPortal.Services.EsbAlert
{
    internal abstract class Worker
    {
        internal abstract void DoWork(ICacheProvider cacheProvider);


        /// <summary>
        /// Request process thread to stop. The current process will finish before the thread is
        /// released for termination.
        /// </summary>
        internal void Stop()
        {
            MarkAsStop = true;
        }

        /// <summary>
        /// Indicates whether a stop can be forcibly applied by clicking stop in the services console.
        /// </summary>
        internal bool CanForceStop { get; set; }

        /// <summary>
        /// Gets or sets the thread to stop value.
        /// </summary>
        internal bool MarkAsStop { get; private set; }
    }
}
