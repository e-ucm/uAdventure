using Polly;
using Polly.Retry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Xasu.Util
{
    /// <summary>
    ///     Fluent API for defining a <see cref="AsyncRetryPolicy" />.
    /// </summary>
    /// 
    public static class UnityPolly
    {

        /// <summary>
        ///     Builds an <see cref="AsyncRetryPolicy"/> that will wait and retry <paramref name="retryCount"/> times.
        ///     On each retry, the duration to wait is calculated by calling <paramref name="sleepDurationProvider"/> with
        ///     the current retry number (1 for first retry, 2 for second etc).
        /// </summary>
        /// <param name="policyBuilder">The policy builder.</param>
        /// <param name="retryCount">The retry count.</param>
        /// <param name="sleepDurationProvider">The function that provides the duration to wait for for a particular retry attempt.</param>
        /// <returns>The policy instance.</returns>
        public static AsyncRetryPolicy UnityWaitAndRetryAsync(this PolicyBuilder policyBuilder, int retryCount, Func<int, TimeSpan> sleepDurationProvider)
        {
            Action<Exception, TimeSpan> doNothing = (_, __) => { };

            return policyBuilder.UnityWaitAndRetryAsync(retryCount, sleepDurationProvider, doNothing);
        }

        /// <summary>
        ///     Builds an <see cref="AsyncRetryPolicy" /> that will wait and retry <paramref name="retryCount" /> times
        ///     calling <paramref name="onRetry" /> on each retry with the raised exception and the current sleep duration.
        ///     On each retry, the duration to wait is calculated by calling <paramref name="sleepDurationProvider" /> with
        ///     the current retry number (1 for first retry, 2 for second etc).
        /// </summary>
        /// <param name="policyBuilder">The policy builder.</param>
        /// <param name="retryCount">The retry count.</param>
        /// <param name="sleepDurationProvider">The function that provides the duration to wait for for a particular retry attempt.</param>
        /// <param name="onRetry">The action to call on each retry.</param>
        /// <returns>The policy instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException">retryCount;Value must be greater than or equal to zero.</exception>
        /// <exception cref="ArgumentNullException">
        ///     sleepDurationProvider
        ///     or
        ///     onRetry
        /// </exception>
        public static AsyncRetryPolicy UnityWaitAndRetryAsync(this PolicyBuilder policyBuilder, int retryCount, Func<int, TimeSpan> sleepDurationProvider, Action<Exception, TimeSpan> onRetry)
        {
            if (onRetry == null) throw new ArgumentNullException(nameof(onRetry));

            return policyBuilder.UnityWaitAndRetryAsync(
                retryCount,
                sleepDurationProvider,
#pragma warning disable 1998 // async method has no awaits, will run synchronously
            onRetryAsync: async (outcome, span, _, __) => onRetry(outcome, span)
#pragma warning restore 1998
        );
        }

        /// <summary>
        ///     Builds an <see cref="AsyncRetryPolicy" /> that will wait and retry <paramref name="retryCount" /> times
        ///     calling <paramref name="onRetryAsync" /> on each retry with the raised exception and the current sleep duration.
        ///     On each retry, the duration to wait is calculated by calling <paramref name="sleepDurationProvider" /> with
        ///     the current retry number (1 for first retry, 2 for second etc).
        /// </summary>
        /// <param name="policyBuilder">The policy builder.</param>
        /// <param name="retryCount">The retry count.</param>
        /// <param name="sleepDurationProvider">The function that provides the duration to wait for for a particular retry attempt.</param>
        /// <param name="onRetryAsync">The action to call asynchronously on each retry.</param>
        /// <returns>The policy instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException">retryCount;Value must be greater than or equal to zero.</exception>
        /// <exception cref="ArgumentNullException">
        ///     sleepDurationProvider
        ///     or
        ///     onRetryAsync
        /// </exception>
        public static AsyncRetryPolicy UnityWaitAndRetryAsync(this PolicyBuilder policyBuilder, int retryCount, Func<int, TimeSpan> sleepDurationProvider, Func<Exception, TimeSpan, Task> onRetryAsync)
        {
            if (onRetryAsync == null) throw new ArgumentNullException(nameof(onRetryAsync));

            return policyBuilder.UnityWaitAndRetryAsync(
                retryCount,
                sleepDurationProvider,
#pragma warning disable 1998 // async method has no awaits, will run synchronously
            onRetryAsync: (outcome, span, _, __) => onRetryAsync(outcome, span)
#pragma warning restore 1998
        );
        }

        /// <summary>
        ///     Builds an <see cref="AsyncRetryPolicy" /> that will wait and retry <paramref name="retryCount" /> times
        ///     calling <paramref name="onRetry" /> on each retry with the raised exception, the current sleep duration and context data.
        ///     On each retry, the duration to wait is calculated by calling <paramref name="sleepDurationProvider" /> with
        ///     the current retry number (1 for first retry, 2 for second etc).
        /// </summary>
        /// <param name="policyBuilder">The policy builder.</param>
        /// <param name="retryCount">The retry count.</param>
        /// <param name="sleepDurationProvider">The function that provides the duration to wait for for a particular retry attempt.</param>
        /// <param name="onRetry">The action to call on each retry.</param>
        /// <returns>The policy instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException">retryCount;Value must be greater than or equal to zero.</exception>
        /// <exception cref="ArgumentNullException">
        ///     sleepDurationProvider
        ///     or
        ///     onRetry
        /// </exception>
        public static AsyncRetryPolicy UnityWaitAndRetryAsync(this PolicyBuilder policyBuilder, int retryCount,
            Func<int, TimeSpan> sleepDurationProvider, Action<Exception, TimeSpan, Context> onRetry)
        {
            if (onRetry == null) throw new ArgumentNullException(nameof(onRetry));

            return policyBuilder.UnityWaitAndRetryAsync(
                retryCount,
                sleepDurationProvider,
#pragma warning disable 1998 // async method has no awaits, will run synchronously
            onRetryAsync: async (outcome, span, _, ctx) => onRetry(outcome, span, ctx)
#pragma warning restore 1998
            );
        }

        /// <summary>
        ///     Builds an <see cref="AsyncRetryPolicy" /> that will wait and retry <paramref name="retryCount" /> times
        ///     calling <paramref name="onRetryAsync" /> on each retry with the raised exception, the current sleep duration and context data.
        ///     On each retry, the duration to wait is calculated by calling <paramref name="sleepDurationProvider" /> with
        ///     the current retry number (1 for first retry, 2 for second etc).
        /// </summary>
        /// <param name="policyBuilder">The policy builder.</param>
        /// <param name="retryCount">The retry count.</param>
        /// <param name="sleepDurationProvider">The function that provides the duration to wait for for a particular retry attempt.</param>
        /// <param name="onRetryAsync">The action to call asynchronously on each retry.</param>
        /// <returns>The policy instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException">retryCount;Value must be greater than or equal to zero.</exception>
        /// <exception cref="ArgumentNullException">
        ///     sleepDurationProvider
        ///     or
        ///     onRetryAsync
        /// </exception>
        public static AsyncRetryPolicy UnityWaitAndRetryAsync(this PolicyBuilder policyBuilder, int retryCount,
            Func<int, TimeSpan> sleepDurationProvider, Func<Exception, TimeSpan, Context, Task> onRetryAsync)
        {
            if (onRetryAsync == null) throw new ArgumentNullException(nameof(onRetryAsync));

            return policyBuilder.UnityWaitAndRetryAsync(
                retryCount,
                sleepDurationProvider,
                onRetryAsync: (outcome, timespan, _, ctx) => onRetryAsync(outcome, timespan, ctx)
            );
        }

        /// <summary>
        ///     Builds an <see cref="AsyncRetryPolicy" /> that will wait and retry <paramref name="retryCount" /> times
        ///     calling <paramref name="onRetry" /> on each retry with the raised exception, the current sleep duration, retry count, and context data.
        ///     On each retry, the duration to wait is calculated by calling <paramref name="sleepDurationProvider" /> with
        ///     the current retry number (1 for first retry, 2 for second etc).
        /// </summary>
        /// <param name="policyBuilder">The policy builder.</param>
        /// <param name="retryCount">The retry count.</param>
        /// <param name="sleepDurationProvider">The function that provides the duration to wait for for a particular retry attempt.</param>
        /// <param name="onRetry">The action to call on each retry.</param>
        /// <returns>The policy instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException">retryCount;Value must be greater than or equal to zero.</exception>
        /// <exception cref="ArgumentNullException">
        ///     sleepDurationProvider
        ///     or
        ///     onRetry
        /// </exception>
        public static AsyncRetryPolicy UnityWaitAndRetryAsync(this PolicyBuilder policyBuilder, int retryCount, Func<int, TimeSpan> sleepDurationProvider, Action<Exception, TimeSpan, int, Context> onRetry)
        {
            if (onRetry == null) throw new ArgumentNullException(nameof(onRetry));

            return policyBuilder.UnityWaitAndRetryAsync(
                retryCount,
                sleepDurationProvider,
#pragma warning disable 1998 // async method has no awaits, will run synchronously
            onRetryAsync: async (outcome, timespan, i, ctx) => onRetry(outcome, timespan, i, ctx)
#pragma warning restore 1998
        );
        }

        /// <summary>
        ///     Builds an <see cref="AsyncRetryPolicy" /> that will wait and retry <paramref name="retryCount" /> times
        ///     calling <paramref name="onRetryAsync" /> on each retry with the raised exception, the current sleep duration, retry count, and context data.
        ///     On each retry, the duration to wait is calculated by calling <paramref name="sleepDurationProvider" /> with
        ///     the current retry number (1 for first retry, 2 for second etc).
        /// </summary>
        /// <param name="policyBuilder">The policy builder.</param>
        /// <param name="retryCount">The retry count.</param>
        /// <param name="sleepDurationProvider">The function that provides the duration to wait for for a particular retry attempt.</param>
        /// <param name="onRetryAsync">The action to call asynchronously on each retry.</param>
        /// <returns>The policy instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException">retryCount;Value must be greater than or equal to zero.</exception>
        /// <exception cref="ArgumentNullException">
        ///     sleepDurationProvider
        ///     or
        ///     onRetryAsync
        /// </exception>
        public static AsyncRetryPolicy UnityWaitAndRetryAsync(this PolicyBuilder policyBuilder, int retryCount,
            Func<int, TimeSpan> sleepDurationProvider, Func<Exception, TimeSpan, int, Context, Task> onRetryAsync)
        {
            if (retryCount < 0) throw new ArgumentOutOfRangeException(nameof(retryCount), "Value must be greater than or equal to zero.");
            if (sleepDurationProvider == null) throw new ArgumentNullException(nameof(sleepDurationProvider));
            if (onRetryAsync == null) throw new ArgumentNullException(nameof(onRetryAsync));

            IEnumerable<TimeSpan> sleepDurations = Enumerable.Range(1, retryCount)
                .Select(sleepDurationProvider);

            return CreateUnityWaitAndRetryAsync(
                policyBuilder,
                onRetryAsync,
                retryCount,
                sleepDurations
            );
        }

        /// <summary>
        ///     Builds an <see cref="AsyncRetryPolicy" /> that will wait and retry <paramref name="retryCount" /> times
        ///     calling <paramref name="onRetry" /> on each retry with the raised exception, the current sleep duration and context data.
        ///     On each retry, the duration to wait is calculated by calling <paramref name="sleepDurationProvider" /> with
        ///     the current retry number (1 for first retry, 2 for second etc) and execution context.
        /// </summary>
        /// <param name="policyBuilder">The policy builder.</param>
        /// <param name="retryCount">The retry count.</param>
        /// <param name="sleepDurationProvider">The function that provides the duration to wait for for a particular retry attempt.</param>
        /// <param name="onRetry">The action to call on each retry.</param>
        /// <returns>The policy instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException">retryCount;Value must be greater than or equal to zero.</exception>
        /// <exception cref="ArgumentNullException">
        ///     sleepDurationProvider
        ///     or
        ///     onRetry
        /// </exception>
        public static AsyncRetryPolicy UnityWaitAndRetryAsync(this PolicyBuilder policyBuilder, int retryCount,
            Func<int, Context, TimeSpan> sleepDurationProvider, Action<Exception, TimeSpan, Context> onRetry)
        {
            if (onRetry == null) throw new ArgumentNullException(nameof(onRetry));

            return policyBuilder.UnityWaitAndRetryAsync(
                retryCount,
                sleepDurationProvider,
#pragma warning disable 1998 // async method has no awaits, will run synchronously
            onRetryAsync: async (outcome, span, _, ctx) => onRetry(outcome, span, ctx)
#pragma warning restore 1998
            );
        }

        /// <summary>
        ///     Builds an <see cref="AsyncRetryPolicy" /> that will wait and retry <paramref name="retryCount" /> times
        ///     calling <paramref name="onRetryAsync" /> on each retry with the raised exception, the current sleep duration and context data.
        ///     On each retry, the duration to wait is calculated by calling <paramref name="sleepDurationProvider" /> with
        ///     the current retry number (1 for first retry, 2 for second etc) and execution context.
        /// </summary>
        /// <param name="policyBuilder">The policy builder.</param>
        /// <param name="retryCount">The retry count.</param>
        /// <param name="sleepDurationProvider">The function that provides the duration to wait for for a particular retry attempt.</param>
        /// <param name="onRetryAsync">The action to call asynchronously on each retry.</param>
        /// <returns>The policy instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException">retryCount;Value must be greater than or equal to zero.</exception>
        /// <exception cref="ArgumentNullException">
        ///     sleepDurationProvider
        ///     or
        ///     onRetryAsync
        /// </exception>
        public static AsyncRetryPolicy UnityWaitAndRetryAsync(this PolicyBuilder policyBuilder, int retryCount,
            Func<int, Context, TimeSpan> sleepDurationProvider, Func<Exception, TimeSpan, Context, Task> onRetryAsync)
        {
            if (onRetryAsync == null) throw new ArgumentNullException(nameof(onRetryAsync));

            return policyBuilder.UnityWaitAndRetryAsync(
                retryCount,
                sleepDurationProvider,
                onRetryAsync: (outcome, timespan, _, ctx) => onRetryAsync(outcome, timespan, ctx)
            );
        }

        /// <summary>
        ///     Builds an <see cref="AsyncRetryPolicy" /> that will wait and retry <paramref name="retryCount" /> times
        ///     calling <paramref name="onRetry" /> on each retry with the raised exception, the current sleep duration, retry count, and context data.
        ///     On each retry, the duration to wait is calculated by calling <paramref name="sleepDurationProvider" /> with
        ///     the current retry number (1 for first retry, 2 for second etc) and execution context.
        /// </summary>
        /// <param name="policyBuilder">The policy builder.</param>
        /// <param name="retryCount">The retry count.</param>
        /// <param name="sleepDurationProvider">The function that provides the duration to wait for for a particular retry attempt.</param>
        /// <param name="onRetry">The action to call on each retry.</param>
        /// <returns>The policy instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException">retryCount;Value must be greater than or equal to zero.</exception>
        /// <exception cref="ArgumentNullException">
        ///     sleepDurationProvider
        ///     or
        ///     onRetry
        /// </exception>
        public static AsyncRetryPolicy UnityWaitAndRetryAsync(this PolicyBuilder policyBuilder, int retryCount, Func<int, Context, TimeSpan> sleepDurationProvider, Action<Exception, TimeSpan, int, Context> onRetry)
        {
            if (onRetry == null) throw new ArgumentNullException(nameof(onRetry));

            return policyBuilder.UnityWaitAndRetryAsync(
                retryCount,
                sleepDurationProvider,
#pragma warning disable 1998 // async method has no awaits, will run synchronously
            onRetryAsync: async (outcome, timespan, i, ctx) => onRetry(outcome, timespan, i, ctx)
#pragma warning restore 1998
        );
        }

        /// <summary>
        ///     Builds an <see cref="AsyncRetryPolicy" /> that will wait and retry <paramref name="retryCount" /> times
        ///     calling <paramref name="onRetryAsync" /> on each retry with the raised exception, the current sleep duration, retry count, and context data.
        ///     On each retry, the duration to wait is calculated by calling <paramref name="sleepDurationProvider" /> with
        ///     the current retry number (1 for first retry, 2 for second etc) and execution context.
        /// </summary>
        /// <param name="policyBuilder">The policy builder.</param>
        /// <param name="retryCount">The retry count.</param>
        /// <param name="sleepDurationProvider">The function that provides the duration to wait for for a particular retry attempt.</param>
        /// <param name="onRetryAsync">The action to call asynchronously on each retry.</param>
        /// <returns>The policy instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException">retryCount;Value must be greater than or equal to zero.</exception>
        /// <exception cref="ArgumentNullException">
        ///     sleepDurationProvider
        ///     or
        ///     onRetryAsync
        /// </exception>
        public static AsyncRetryPolicy UnityWaitAndRetryAsync(this PolicyBuilder policyBuilder, int retryCount,
            Func<int, Context, TimeSpan> sleepDurationProvider, Func<Exception, TimeSpan, int, Context, Task> onRetryAsync)
        {
            if (sleepDurationProvider == null) throw new ArgumentNullException(nameof(sleepDurationProvider));
            return policyBuilder.UnityWaitAndRetryAsync(
                retryCount,
                (i, _, ctx) => sleepDurationProvider(i, ctx),
                onRetryAsync);
        }

        /// <summary>
        ///     Builds an <see cref="AsyncRetryPolicy" /> that will wait and retry <paramref name="retryCount" /> times
        ///     calling <paramref name="onRetryAsync" /> on each retry with the raised exception, the current sleep duration, retry count, and context data.
        ///     On each retry, the duration to wait is calculated by calling <paramref name="sleepDurationProvider" /> with
        ///     the current retry number (1 for first retry, 2 for second etc), previous exception and execution context.
        /// </summary>
        /// <param name="policyBuilder">The policy builder.</param>
        /// <param name="retryCount">The retry count.</param>
        /// <param name="sleepDurationProvider">The function that provides the duration to wait for for a particular retry attempt.</param>
        /// <param name="onRetryAsync">The action to call asynchronously on each retry.</param>
        /// <returns>The policy instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException">retryCount;Value must be greater than or equal to zero.</exception>
        /// <exception cref="ArgumentNullException">
        ///     sleepDurationProvider
        ///     or
        ///     onRetryAsync
        /// </exception>
        public static AsyncRetryPolicy UnityWaitAndRetryAsync(this PolicyBuilder policyBuilder, int retryCount,
            Func<int, Exception, Context, TimeSpan> sleepDurationProvider, Func<Exception, TimeSpan, int, Context, Task> onRetryAsync)
        {
            if (retryCount < 0) throw new ArgumentOutOfRangeException(nameof(retryCount), "Value must be greater than or equal to zero.");
            if (sleepDurationProvider == null) throw new ArgumentNullException(nameof(sleepDurationProvider));
            if (onRetryAsync == null) throw new ArgumentNullException(nameof(onRetryAsync));

            return CreateUnityWaitAndRetryAsync(
                policyBuilder,
                onRetryAsync,
                retryCount,
                sleepDurationProvider
            );
        }

        /// <summary>
        ///     Builds an <see cref="AsyncRetryPolicy" /> that will wait and retry as many times as there are provided
        ///     <paramref name="sleepDurations" />
        ///     On each retry, the duration to wait is the current <paramref name="sleepDurations" /> item.
        /// </summary>
        /// <param name="policyBuilder">The policy builder.</param>
        /// <param name="sleepDurations">The sleep durations to wait for on each retry.</param>
        /// <returns>The policy instance.</returns>
        public static AsyncRetryPolicy UnityWaitAndRetryAsync(this PolicyBuilder policyBuilder, IEnumerable<TimeSpan> sleepDurations)
        {
            Action<Exception, TimeSpan, int, Context> doNothing = (_, __, ___, ____) => { };

            return policyBuilder.UnityWaitAndRetryAsync(sleepDurations, doNothing);
        }

        /// <summary>
        ///     Builds an <see cref="AsyncRetryPolicy" /> that will wait and retry as many times as there are provided
        ///     <paramref name="sleepDurations" />
        ///     calling <paramref name="onRetry" /> on each retry with the raised exception and the current sleep duration.
        ///     On each retry, the duration to wait is the current <paramref name="sleepDurations" /> item.
        /// </summary>
        /// <param name="policyBuilder">The policy builder.</param>
        /// <param name="sleepDurations">The sleep durations to wait for on each retry.</param>
        /// <param name="onRetry">The action to call on each retry.</param>
        /// <returns>The policy instance.</returns>
        /// <exception cref="ArgumentNullException">
        ///     sleepDurations
        ///     or
        ///     onRetry
        /// </exception>
        public static AsyncRetryPolicy UnityWaitAndRetryAsync(this PolicyBuilder policyBuilder, IEnumerable<TimeSpan> sleepDurations, Action<Exception, TimeSpan> onRetry)
        {
            if (onRetry == null) throw new ArgumentNullException(nameof(onRetry));

            return policyBuilder.UnityWaitAndRetryAsync(
                sleepDurations,
#pragma warning disable 1998 // async method has no awaits, will run synchronously
            onRetryAsync: async (outcome, timespan, _, __) => onRetry(outcome, timespan)
#pragma warning restore 1998
        );
        }

        /// <summary>
        ///     Builds an <see cref="AsyncRetryPolicy" /> that will wait and retry as many times as there are provided
        ///     <paramref name="sleepDurations" />
        ///     calling <paramref name="onRetryAsync" /> on each retry with the raised exception and the current sleep duration.
        ///     On each retry, the duration to wait is the current <paramref name="sleepDurations" /> item.
        /// </summary>
        /// <param name="policyBuilder">The policy builder.</param>
        /// <param name="sleepDurations">The sleep durations to wait for on each retry.</param>
        /// <param name="onRetryAsync">The action to call asynchronously on each retry.</param>
        /// <returns>The policy instance.</returns>
        /// <exception cref="ArgumentNullException">
        ///     sleepDurations
        ///     or
        ///     onRetryAsync
        /// </exception>
        public static AsyncRetryPolicy UnityWaitAndRetryAsync(this PolicyBuilder policyBuilder, IEnumerable<TimeSpan> sleepDurations, Func<Exception, TimeSpan, Task> onRetryAsync)
        {
            if (onRetryAsync == null) throw new ArgumentNullException(nameof(onRetryAsync));

            return policyBuilder.UnityWaitAndRetryAsync(
                sleepDurations,
                onRetryAsync: (outcome, timespan, _, __) => onRetryAsync(outcome, timespan)
            );
        }

        /// <summary>
        ///     Builds an <see cref="AsyncRetryPolicy" /> that will wait and retry as many times as there are provided
        ///     <paramref name="sleepDurations" />
        ///     calling <paramref name="onRetry" /> on each retry with the raised exception, the current sleep duration and context data.
        ///     On each retry, the duration to wait is the current <paramref name="sleepDurations" /> item.
        /// </summary>
        /// <param name="policyBuilder">The policy builder.</param>
        /// <param name="sleepDurations">The sleep durations to wait for on each retry.</param>
        /// <param name="onRetry">The action to call on each retry.</param>
        /// <returns>The policy instance.</returns>
        /// <exception cref="ArgumentNullException">
        ///     sleepDurations
        ///     or
        ///     onRetry
        /// </exception>
        public static AsyncRetryPolicy UnityWaitAndRetryAsync(this PolicyBuilder policyBuilder, IEnumerable<TimeSpan> sleepDurations, Action<Exception, TimeSpan, Context> onRetry)
        {
            if (onRetry == null) throw new ArgumentNullException(nameof(onRetry));

            return policyBuilder.UnityWaitAndRetryAsync(
                sleepDurations,
#pragma warning disable 1998 // async method has no awaits, will run synchronously
            onRetryAsync: async (outcome, timespan, _, ctx) => onRetry(outcome, timespan, ctx)
#pragma warning restore 1998
        );
        }

        /// <summary>
        ///     Builds an <see cref="AsyncRetryPolicy" /> that will wait and retry as many times as there are provided
        ///     <paramref name="sleepDurations" />
        ///     calling <paramref name="onRetryAsync" /> on each retry with the raised exception, the current sleep duration and context data.
        ///     On each retry, the duration to wait is the current <paramref name="sleepDurations" /> item.
        /// </summary>
        /// <param name="policyBuilder">The policy builder.</param>
        /// <param name="sleepDurations">The sleep durations to wait for on each retry.</param>
        /// <param name="onRetryAsync">The action to call asynchronously on each retry.</param>
        /// <returns>The policy instance.</returns>
        /// <exception cref="ArgumentNullException">
        ///     sleepDurations
        ///     or
        ///     onRetryAsync
        /// </exception>
        public static AsyncRetryPolicy UnityWaitAndRetryAsync(this PolicyBuilder policyBuilder, IEnumerable<TimeSpan> sleepDurations, Func<Exception, TimeSpan, Context, Task> onRetryAsync)
        {
            if (onRetryAsync == null) throw new ArgumentNullException(nameof(onRetryAsync));

            return policyBuilder.UnityWaitAndRetryAsync(
                sleepDurations,
                onRetryAsync: (outcome, timespan, _, ctx) => onRetryAsync(outcome, timespan, ctx)
            );
        }

        /// <summary>
        ///     Builds an <see cref="AsyncRetryPolicy" /> that will wait and retry as many times as there are provided
        ///     <paramref name="sleepDurations" />
        ///     calling <paramref name="onRetry" /> on each retry with the raised exception, the current sleep duration, retry count, and context data.
        ///     On each retry, the duration to wait is the current <paramref name="sleepDurations" /> item.
        /// </summary>
        /// <param name="policyBuilder">The policy builder.</param>
        /// <param name="sleepDurations">The sleep durations to wait for on each retry.</param>
        /// <param name="onRetry">The action to call on each retry.</param>
        /// <returns>The policy instance.</returns>
        /// <exception cref="ArgumentNullException">
        ///     sleepDurations
        ///     or
        ///     onRetry
        /// </exception>
        public static AsyncRetryPolicy UnityWaitAndRetryAsync(this PolicyBuilder policyBuilder, IEnumerable<TimeSpan> sleepDurations, Action<Exception, TimeSpan, int, Context> onRetry)
        {
            if (onRetry == null) throw new ArgumentNullException(nameof(onRetry));

            return policyBuilder.UnityWaitAndRetryAsync(
                sleepDurations,
    #pragma warning disable 1998 // async method has no awaits, will run synchronously
                onRetryAsync: async (outcome, timespan, i, ctx) => onRetry(outcome, timespan, i, ctx)
    #pragma warning restore 1998
            );
        }

        /// <summary>
        ///     Builds an <see cref="AsyncRetryPolicy" /> that will wait and retry as many times as there are provided
        ///     <paramref name="sleepDurations" />
        ///     calling <paramref name="onRetryAsync" /> on each retry with the raised exception, the current sleep duration, retry count, and context data.
        ///     On each retry, the duration to wait is the current <paramref name="sleepDurations" /> item.
        /// </summary>
        /// <param name="policyBuilder">The policy builder.</param>
        /// <param name="sleepDurations">The sleep durations to wait for on each retry.</param>
        /// <param name="onRetryAsync">The action to call asynchronously on each retry.</param>
        /// <returns>The policy instance.</returns>
        /// <exception cref="ArgumentNullException">
        ///     sleepDurations
        ///     or
        ///     onRetryAsync
        /// </exception>
        public static AsyncRetryPolicy UnityWaitAndRetryAsync(this PolicyBuilder policyBuilder, IEnumerable<TimeSpan> sleepDurations, Func<Exception, TimeSpan, int, Context, Task> onRetryAsync)
        {
            if (sleepDurations == null) throw new ArgumentNullException(nameof(sleepDurations));
            if (onRetryAsync == null) throw new ArgumentNullException(nameof(onRetryAsync));

            return CreateUnityWaitAndRetryAsync(
                policyBuilder,
                onRetryAsync,
                sleepDurations
            );
        }

        private static AsyncRetryPolicy CreateUnityWaitAndRetryAsync(PolicyBuilder policyBuilder, Func<Exception, TimeSpan, int, Context, Task> onRetryAsync, IEnumerable<TimeSpan> sleepDurations)
        {
            return CreateUnityWaitAndRetryAsync(
                policyBuilder, 
                onRetryAsync, 
                sleepDurations.Count(),
                sleepDurations);
        }
        private static AsyncRetryPolicy CreateUnityWaitAndRetryAsync(PolicyBuilder policyBuilder, Func<Exception, TimeSpan, int, Context, Task> onRetryAsync, int retryCount, IEnumerable<TimeSpan> sleepDurations)
        {
            return CreateUnityWaitAndRetryAsync(
                policyBuilder,
                onRetryAsync,
                retryCount: retryCount,
                sleepDurationsProvider: (retry, _, __) => sleepDurations.ElementAt(retry - 1));
        }

        private static AsyncRetryPolicy CreateUnityWaitAndRetryAsync(PolicyBuilder policyBuilder, Func<Exception, TimeSpan, int, Context, Task> onRetryAsync, int retryCount, Func<int, Exception, Context, TimeSpan> sleepDurationsProvider)
        {
            return policyBuilder.RetryAsync(retryCount, async (ex, retry, context) =>
            {
                var timeSpan = sleepDurationsProvider(retry, ex, context);
                var time = timeSpan.TotalSeconds;
                var timeElapsed = 0f;
                while (timeElapsed < time)
                {
                    await Task.Yield();
                    timeElapsed += UnityEngine.Time.deltaTime;
                }

                await onRetryAsync(ex, timeSpan, retry, context);
            });
        }
    }
}