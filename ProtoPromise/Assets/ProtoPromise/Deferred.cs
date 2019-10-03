﻿using System;

namespace Proto.Promises
{
	partial class Promise
    {
        public abstract partial class DeferredBase : ICancelableAny, IRetainable
        {
            public State State { get; protected set; }

            /// <summary>
            /// The promise that this controls.
            /// </summary>
            /// <value>The promise.</value>
            public virtual Promise Promise { get; protected set; }

#if CSHARP_7_3_OR_NEWER // Really C# 7.2, but this symbol is the closest Unity offers.
            private
#endif
            protected DeferredBase() { }

            /// <summary>
            /// Retain this instance and the linked <see cref="Promise"/>.
            /// <para/>This should always be paired with a call to <see cref="Release"/>
            /// </summary>
            public void Retain()
            {
                Promise.Retain();
            }

            /// <summary>
            /// Release this instance and the linked <see cref="Promise"/>.
            /// <para/>This should always be paired with a call to <see cref="Retain"/>
            /// </summary>
            public void Release()
            {
                Promise.Release();
            }

            /// <summary>
            /// Reject the linked <see cref="Promise"/> without a reason.
            /// <para/>NOTE: It is recommended to always reject with a reason!
            /// </summary>
            public abstract void Reject();

            /// <summary>
            /// Reject the linked <see cref="Promise"/> with <paramref name="reason"/>.
            /// </summary>
            public abstract void Reject<TReject>(TReject reason);
        }

        public abstract class Deferred : DeferredBase
        {
#if CSHARP_7_3_OR_NEWER // Really C# 7.2, but this symbol is the closest Unity offers.
            private
#endif
            protected Deferred() { }

            /// <summary>
            /// Resolve the linked <see cref="Promise"/>.
            /// </summary>
            public abstract void Resolve();
        }
    }

	public partial class Promise<T>
	{
		public abstract new class Deferred : DeferredBase
        {
            public new Promise<T> Promise { get { return (Promise<T>) base.Promise; } protected set { base.Promise = value; } }

#if CSHARP_7_3_OR_NEWER // Really C# 7.2, but this symbol is the closest Unity offers.
            private
#endif
            protected Deferred() { }

            /// <summary>
            /// Resolve the linked <see cref="Promise"/> with <paramref name="value"/>.
            /// </summary>
            public abstract void Resolve(T value);
        }
    }

    partial class Promise
    {
        partial class Internal
        {
            public sealed class DeferredInternal : Deferred
            {
                public DeferredInternal(Promise target)
                {
                    Promise = target;
                }

                public void Reset()
                {
                    State = State.Pending;
                }

                public override void ReportProgress(float progress)
                {
                    var promise = Promise;
                    ValidateProgress();
                    ValidateOperation(promise, 1);
                    ValidateProgress(progress, 1);

                    if (State != State.Pending)
                    {
                        Logger.LogWarning("Deferred.ReportProgress - Deferred is not in the pending state.");
                        return;
                    }

                    promise.ReportProgress(progress);
                }

                public override void Resolve()
                {
                    var promise = Promise;
                    ValidateOperation(promise, 1);

                    if (State == State.Pending)
                    {
                        promise.Release();
                    }
                    else
                    {
                        Logger.LogWarning("Deferred.Resolve - Deferred is not in the pending state.");
                        return;
                    }

                    State = State.Resolved;
                    promise.Resolve();
                }

                public override void Reject()
                {
                    var promise = Promise;
                    ValidateOperation(promise, 1);

                    if (State == State.Pending)
                    {
                        promise.Release();
                        State = State.Rejected;
                    }
                    else
                    {
                        Logger.LogWarning("Deferred.Reject - Deferred is not in the pending state.");
                    }

                    promise.Reject(1);
                }

                public override void Reject<TReject>(TReject reason)
                {
                    var promise = Promise;
                    ValidateOperation(promise, 1);

                    if (State == State.Pending)
                    {
                        promise.Release();
                        State = State.Rejected;
                    }
                    else
                    {
                        Logger.LogWarning("Deferred.Reject - Deferred is not in the pending state.");
                    }

                    promise.Reject(reason, 1);
                }

                public void RejectWithPromiseStacktrace(Exception exception)
                {
                    var promise = Promise;
                    var rejectValue = UnhandledExceptionException.GetOrCreate(exception);
                    _SetStackTraceFromCreated(promise, rejectValue);

                    if (State != State.Pending)
                    {
                        AddRejectionToUnhandledStack(rejectValue);
                        return;
                    }

                    State = State.Rejected;
                    promise.Release();
                    promise.RejectWithStateCheck(rejectValue);
                }
            }
        }
    }

    partial class Promise<T>
    {
        protected static new class Internal
        {
            public sealed class DeferredInternal : Deferred
            {
                public DeferredInternal(Promise<T> target)
                {
                    Promise = target;
                }

                public void Reset()
                {
                    State = State.Pending;
                }

                public override void ReportProgress(float progress)
                {
                    var promise = Promise;
                    ValidateProgress();
                    ValidateOperation(promise, 1);
                    ValidateProgress(progress, 1);

                    if (State != State.Pending)
                    {
                        Logger.LogWarning("Deferred.ReportProgress - Deferred is not in the pending state.");
                        return;
                    }

                    promise.ReportProgress(progress);
                }

                public override void Resolve(T value)
                {
                    var promise = Promise;
                    ValidateOperation(promise, 1);

                    if (State == State.Pending)
                    {
                        promise.Release();
                    }
                    else
                    {
                        Logger.LogWarning("Deferred.Resolve - Deferred is not in the pending state.");
                        return;
                    }

                    State = State.Resolved;
                    promise.Resolve(value);
                }

                public override void Reject()
                {
                    var promise = Promise;
                    ValidateOperation(promise, 1);

                    if (State == State.Pending)
                    {
                        promise.Release();
                        State = State.Rejected;
                    }
                    else
                    {
                        Logger.LogWarning("Deferred.Reject - Deferred is not in the pending state.");
                    }

                    promise.Reject(1);
                }

                public override void Reject<TReject>(TReject reason)
                {
                    var promise = Promise;
                    ValidateOperation(promise, 1);

                    if (State == State.Pending)
                    {
                        promise.Release();
                        State = State.Rejected;
                    }
                    else
                    {
                        Logger.LogWarning("Deferred.Reject - Deferred is not in the pending state.");
                    }

                    promise.Reject(reason, 1);
                }

                public void RejectWithPromiseStacktrace(Exception exception)
                {
                    var promise = Promise;
                    var rejectValue = Promises.Promise.Internal.UnhandledExceptionException.GetOrCreate(exception);
                    _SetStackTraceFromCreated(promise, rejectValue);

                    if (State != State.Pending)
                    {
                        AddRejectionToUnhandledStack(rejectValue);
                        return;
                    }

                    State = State.Rejected;
                    promise.Release();
                    promise.RejectWithStateCheck(rejectValue);
                }
            }
        }
    }
}