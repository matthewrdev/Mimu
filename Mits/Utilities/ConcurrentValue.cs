using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Mits.Utilities
{
    /// <summary>
    /// A helper class that provides thread safety to a <typeparamref name="TValue"/>.
    /// <para/>
    /// When using any method on the <see cref="ConcurrentValue{TValue}"/> that accepts a predicate, limit the work inside the predicate to as small as possible.
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    [DebuggerDisplay("{value}")]
    public class ConcurrentValue<TValue>
    {
        private readonly object valueLock = new object();
        private TValue value;

        readonly Logging.ILogger log;

        public ConcurrentValue()
        {
        }

        public ConcurrentValue(TValue value,
                               bool accessAuditing = false,
                               string elementName = "")
        {
            this.value = value;

            if (accessAuditing)
            {
                if (string.IsNullOrWhiteSpace(elementName))
                {
                    throw new ArgumentException($"'{nameof(elementName)}' cannot be null or whitespace.", nameof(elementName));
                }

                log = Logging.Logger.Create(GetType().Name + "." + elementName);
            }
        }

        public void Set(TValue value, [CallerMemberName] string caller = "")
        {
            try
            {
                log?.Debug($"SetValue.Start ({caller} {value})");
                lock (valueLock)
                {
                    this.value = value;
                }
            }
            finally
            {
                log?.Debug($"SetValue.End ({caller} {value})");
            }

        }

        /// <summary>
        /// Mutates the inner value by reference.
        /// <para/>
        /// Use when <typeparamref name="TValue"/> is mutable/reference data type.
        /// </summary>
        /// <param name="mutator"></param>
        public void Mutate(Action<TValue> mutator, [CallerMemberName] string caller = "")
        {
            if (mutator is null)
            {
                throw new ArgumentNullException(nameof(mutator));
            }

            try
            {
                log?.Debug($"Mutate.Start<Action> ({caller})");
                lock (valueLock)
                {
                    mutator(value);
                }
            }
            finally
            {
                log?.Debug($"Mutate.End<Action> ({caller})");
            }
        }

        /// <summary>
        /// Mutates the inner value by returning a new value via the <paramref name="mutator"/> and re-assigning it.
        /// <para/>
        /// Use when <typeparamref name="TValue"/> is a value type such as <see cref="string"/> or <see cref="int"/>.
        /// </summary>
        /// <param name="mutator"></param>
        public void Mutate(Func<TValue, TValue> mutator, [CallerMemberName] string caller = "")
        {
            if (mutator is null)
            {
                throw new ArgumentNullException(nameof(mutator));
            }

            try
            {
                log?.Debug($"Mutate.Start<Func> ({caller})");
                lock (valueLock)
                {
                    value = mutator(value);
                }
            }
            finally
            {
                log?.Debug($"Mutate.End<Func> ({caller})");
            }
        }

        public TValue Get([CallerMemberName] string caller = "")
        {
            try
            {
                log?.Debug($"Get.Start ({caller})");
                lock (valueLock)
                {
                    return value;
                }
            }
            finally
            {
                log?.Debug($"Get.End ({caller})");
            }
        }

        public TResult Get<TResult>(Func<TValue, TResult> predicate, [CallerMemberName] string caller = "")
        {
            if (predicate is null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            try
            {
                log?.Debug($"Get<Predicate>.Start ({caller})");

                lock (valueLock)
                {
                    return predicate(value);
                }
            }
            finally
            {
                log?.Debug($"Get<Predicate>.End ({caller})");
            }
        }
    }
}
