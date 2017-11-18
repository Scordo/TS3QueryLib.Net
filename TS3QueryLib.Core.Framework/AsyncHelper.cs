using System;
using System.Threading;
using System.Threading.Tasks;

namespace TS3QueryLib.Core
{
    public static class AsyncHelper
    {
        private static TaskFactory TaskFactory { get; } = new TaskFactory(CancellationToken.None, TaskCreationOptions.None, TaskContinuationOptions.None, TaskScheduler.Default);

        public static TResult RunSync<TResult>(Func<Task<TResult>> func)
        {
            return TaskFactory.StartNew(func).Unwrap().GetAwaiter().GetResult();
        }

        public static void RunSync(Func<Task> func)
        {
            TaskFactory.StartNew(func).Unwrap().GetAwaiter().GetResult();
        }
    }
}
