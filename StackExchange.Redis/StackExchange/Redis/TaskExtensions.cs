﻿using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace StackExchange.Redis
{
    internal static class TaskExtensions
    {
        private static readonly Action<Task> observeErrors = ObverveErrors;
        private static void ObverveErrors(this Task task)
        {
            if (task != null) GC.KeepAlive(task.Exception);
        }

        public static Task ObserveErrors(this Task task)
        {
            task?.ContinueWith(observeErrors, TaskContinuationOptions.OnlyOnFaulted);
            return task;
        }

        public static Task<T> ObserveErrors<T>(this Task<T> task)
        {
            task?.ContinueWith(observeErrors, TaskContinuationOptions.OnlyOnFaulted);
            return task;
        }
        public static async Task<bool> TryComplete(this Task task, int timeoutMs)
        {
            var cts = new CancellationTokenSource();

            if (task == await Task.WhenAny(task, Task.Delay(timeoutMs, cts.Token)).ConfigureAwait(false))
            {
                cts.Cancel();
                await task.ConfigureAwait(false);
                return true;
            }
            else
            {
                return false;
            }
        }

        public static ConfiguredTaskAwaitable ForAwait(this Task task) => task.ConfigureAwait(false);
        public static ConfiguredTaskAwaitable<T> ForAwait<T>(this Task<T> task) => task.ConfigureAwait(false);
    }
}
