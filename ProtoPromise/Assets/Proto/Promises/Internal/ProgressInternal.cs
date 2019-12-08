﻿#if PROTO_PROMISE_DEBUG_ENABLE || (!PROTO_PROMISE_DEBUG_DISABLE && DEBUG)
#define PROMISE_DEBUG
#endif
#if !PROTO_PROMISE_CANCEL_DISABLE
#define PROMISE_CANCEL
#endif
#if !PROTO_PROMISE_PROGRESS_DISABLE
#define PROMISE_PROGRESS
#endif

#pragma warning disable RECS0096 // Type parameter is never used
#pragma warning disable IDE0018 // Inline variable declaration
#pragma warning disable IDE0034 // Simplify 'default' expression
#pragma warning disable CS0618 // Type or member is obsolete
#pragma warning disable RECS0029 // Warns about property or indexer setters and event adders or removers that do not use the value parameter

using System;
using Proto.Utils;

namespace Proto.Promises
{
    partial class Promise
    {
#if PROMISE_DEBUG || PROMISE_PROGRESS
        protected virtual void BorrowPassthroughs(ref ValueLinkedStack<Internal.PromisePassThrough> borrower) { }

        protected static void ExchangePassthroughs(ref ValueLinkedStack<Internal.PromisePassThrough> from, ref ValueLinkedStack<Internal.PromisePassThrough> to)
        {
            // Remove this.passThroughs before adding to passThroughs. They are re-added by the caller.
            while (from.IsNotEmpty)
            {
                var passThrough = from.Pop();
                if (passThrough.Owner != null && passThrough.Owner._state != State.Pending)
                {
                    // The owner already completed.
                    passThrough.Release();
                }
                else
                {
                    to.Push(passThrough);
                }
            }
        }

        partial class Internal
        {
            partial class AllPromise0
            {
                protected override void BorrowPassthroughs(ref ValueLinkedStack<PromisePassThrough> borrower)
                {
                    ExchangePassthroughs(ref passThroughs, ref borrower);
                }
            }

            partial class AllPromise<T>
            {
                protected override void BorrowPassthroughs(ref ValueLinkedStack<PromisePassThrough> borrower)
                {
                    ExchangePassthroughs(ref passThroughs, ref borrower);
                }
            }

            partial class RacePromise0
            {
                protected override void BorrowPassthroughs(ref ValueLinkedStack<PromisePassThrough> borrower)
                {
                    ExchangePassthroughs(ref passThroughs, ref borrower);
                }
            }

            partial class RacePromise<T>
            {
                protected override void BorrowPassthroughs(ref ValueLinkedStack<PromisePassThrough> borrower)
                {
                    ExchangePassthroughs(ref passThroughs, ref borrower);
                }
            }

            partial class FirstPromise0
            {
                protected override void BorrowPassthroughs(ref ValueLinkedStack<PromisePassThrough> borrower)
                {
                    ExchangePassthroughs(ref passThroughs, ref borrower);
                }
            }

            partial class FirstPromise<T>
            {
                protected override void BorrowPassthroughs(ref ValueLinkedStack<PromisePassThrough> borrower)
                {
                    ExchangePassthroughs(ref passThroughs, ref borrower);
                }
            }
        }
#endif

        // Calls to these get compiled away when PROGRESS is undefined.
        partial void SetDepth(Promise next);
        partial void ResetDepth();

        partial void ResolveProgressListeners();
        partial void CancelProgressListeners();

        static partial void ValidateProgress(int skipFrames);
        static partial void InvokeProgressListeners();
        static partial void ClearPooledProgress();

#if !PROMISE_PROGRESS
        protected void ReportProgress(float progress) { }

        static protected void ThrowProgressException(int skipFrames)
        {
            throw new InvalidOperationException("Progress is disabled. Remove PROTO_PROMISE_PROGRESS_DISABLE from your compiler symbols to enable progress reports.", GetFormattedStacktrace(skipFrames + 1));
        }

        static partial void ValidateProgress(int skipFrames)
        {
            ThrowProgressException(skipFrames + 1);
        }

        private void SubscribeProgress(Action<float> onProgress, int skipFrames)
        {
            ThrowProgressException(skipFrames + 1);
        }
#else
        protected ValueLinkedStackZeroGC<Internal.IProgressListener> _progressListeners;
        private Internal.UnsignedFixed32 _waitDepthAndProgress;

        static partial void ClearPooledProgress()
        {
            ValueLinkedStackZeroGC<Internal.IProgressListener>.ClearPooledNodes();
            ValueLinkedStackZeroGC<Internal.IInvokable>.ClearPooledNodes();
        }

        // All and Race promises return a value depending on the promises they are waiting on. Other promises return 1.
        protected virtual uint GetIncrementMultiplier()
        {
            return 1u;
        }

        partial void ResolveProgressListeners()
        {
            uint increment = _waitDepthAndProgress.GetDifferenceToNextWholeAsUInt32() * GetIncrementMultiplier();
            while (_progressListeners.IsNotEmpty)
            {
                _progressListeners.Pop().ResolveOrIncrementProgress(this, increment);
            }
        }

        partial void CancelProgressListeners()
        {
            uint increment = _waitDepthAndProgress.GetDifferenceToNextWholeAsUInt32() * GetIncrementMultiplier();
            while (_progressListeners.IsNotEmpty)
            {
                _progressListeners.Pop().CancelOrIncrementProgress(this, increment);
            }
        }

        protected void ReportProgress(float progress)
        {
            if (progress >= 1f | _state != State.Pending)
            {
                // Don't report progress 1.0, that will be reported automatically when the promise is resolved.
                return;
            }

            uint increment = _waitDepthAndProgress.AssignNewDecimalPartAndGetDifferenceAsUInt32(progress);
            foreach (var progressListener in _progressListeners)
            {
                progressListener.IncrementProgress(this, increment);
            }
        }

        partial void ResetDepth()
        {
            _waitDepthAndProgress = default(Internal.UnsignedFixed32);
        }

        partial void SetDepth(Promise next)
        {
            next.SetDepth(_waitDepthAndProgress);
        }

        protected virtual void SetDepth(Internal.UnsignedFixed32 previousDepth)
        {
            _waitDepthAndProgress = previousDepth;
        }

        protected virtual bool SubscribeProgressAndContinueLoop(ref Internal.IProgressListener progressListener, out Promise previous)
        {
            progressListener.Retain();
            _progressListeners.Push(progressListener);
            return (previous = _rejectedOrCanceledValueOrPrevious as Promise) != null;
        }

        protected virtual bool SubscribeProgressIfWaiterAndContinueLoop(ref Internal.IProgressListener progressListener, out Promise previous, ref ValueLinkedStack<Internal.PromisePassThrough> passThroughs)
        {
            return (previous = _rejectedOrCanceledValueOrPrevious as Promise) != null;
        }

        protected void SubscribeProgress(Action<float> onProgress, int skipFrames)
        {
            ValidateOperation(this, skipFrames + 1);
            ValidateArgument(onProgress, "onProgress", skipFrames + 1);

            if (_state == State.Pending)
            {
                Internal.IProgressListener progressListener = Internal.ProgressDelegate.GetOrCreate(onProgress, this, skipFrames + 1);

                // Directly add to listeners for this promise.
                // Sets promise to the one this is waiting on. Returns false if not waiting on another promise.
                Promise promise;
                if (!SubscribeProgressAndContinueLoop(ref progressListener, out promise))
                {
                    // This is the root of the promise tree.
                    progressListener.SetInitialAmount(_waitDepthAndProgress);
                    return;
                }

                SubscribeProgressToBranchesAndRoots(promise, progressListener);
            }
            else if (_state == State.Resolved)
            {
                AddToHandleQueueBack(Internal.ProgressDelegate.GetOrCreate(onProgress, this, skipFrames + 1));
            }

            // Don't report progress if the promise is canceled or rejected.
        }

        private static void SubscribeProgressToBranchesAndRoots(Promise promise, Internal.IProgressListener progressListener)
        {
            // This allows us to subscribe progress to AllPromises and RacePromises iteratively instead of recursively
            ValueLinkedStack<Internal.PromisePassThrough> passThroughs = new ValueLinkedStack<Internal.PromisePassThrough>();

        Repeat:
            SubscribeProgressToChain(promise, progressListener, ref passThroughs);

            if (passThroughs.IsNotEmpty)
            {
                // passThroughs are removed from their targets before adding to passThroughs. Add them back here.
                var passThrough = passThroughs.Pop();
                promise = passThrough.Owner;
                progressListener = passThrough;
                passThrough.Target.ReAdd(passThrough);
                goto Repeat;
            }
        }

        private static void SubscribeProgressToChain(Promise promise, Internal.IProgressListener progressListener, ref ValueLinkedStack<Internal.PromisePassThrough> passThroughs)
        {
            Promise next;
            // If the promise is not waiting on another promise (is the root), it sets next to null, does not add the listener, and returns false.
            // If the promise is waiting on another promise that is not its previous, it adds the listener, transforms progresslistener, sets next to the one it's waiting on, and returns true.
            // Otherwise, it sets next to its previous, adds the listener only if it is a WaitPromise, and returns true.
            while (promise.SubscribeProgressIfWaiterAndContinueLoop(ref progressListener, out next, ref passThroughs))
            {
                promise = next;
            }

            // promise is the root of the promise tree.
            switch (promise._state)
            {
                case State.Pending:
                    {
                        progressListener.SetInitialAmount(promise._waitDepthAndProgress);
                        break;
                    }
                case State.Resolved:
                    {
                        progressListener.SetInitialAmount(promise._waitDepthAndProgress.GetIncrementedWholeTruncated());
                        break;
                    }
                default: // Rejected or Canceled:
                    {
                        progressListener.Retain();
                        progressListener.CancelOrIncrementProgress(promise, promise._waitDepthAndProgress.GetIncrementedWholeTruncated().ToUInt32());
                        break;
                    }
            }
        }

        // Handle progress.
        private static ValueLinkedQueueZeroGC<Internal.IInvokable> _progressQueue;
        private static bool _runningProgress;

        private static void AddToFrontOfProgressQueue(Internal.IInvokable progressListener)
        {
            _progressQueue.Push(progressListener);
        }

        private static void AddToBackOfProgressQueue(Internal.IInvokable progressListener)
        {
            _progressQueue.Enqueue(progressListener);
        }

        static partial void InvokeProgressListeners()
        {
            if (_runningProgress)
            {
                // HandleProgress is running higher in the program stack, so just return.
                return;
            }

            _runningProgress = true;

            // Cancels are high priority, make sure those delegates are invoked before anything else.
            HandleCanceled();

            while (_progressQueue.IsNotEmpty)
            {
                _progressQueue.DequeueRisky().Invoke();

                HandleCanceled();
            }

            _progressQueue.ClearLast();
            _runningProgress = false;
        }

        partial class Internal
        {
            /// <summary>
            /// Max Whole Number: 2^(32-<see cref="Config.ProgressDecimalBits"/>)
            /// Precision: 1/(2^<see cref="Config.ProgressDecimalBits"/>)
            /// </summary>
            public struct UnsignedFixed32
            {
                private const uint DecimalMax = 1u << Config.ProgressDecimalBits;
                private const uint DecimalMask = DecimalMax - 1u;
                private const uint WholeMask = ~DecimalMask;

                private uint _value;

                public UnsignedFixed32(uint wholePart)
                {
                    _value = wholePart << Config.ProgressDecimalBits;
                }

                public UnsignedFixed32(float decimalPart)
                {
                    // Don't bother rounding, we don't want to accidentally round to 1.0.
                    _value = (uint) (decimalPart * DecimalMax);
                }

                public uint WholePart { get { return _value >> Config.ProgressDecimalBits; } }
                private double DecimalPart { get { return (double) DecimalPartAsUInt32 / (double) DecimalMax; } }
                private uint DecimalPartAsUInt32 { get { return _value & DecimalMask; } }

                public uint ToUInt32()
                {
                    return _value;
                }

                public double ToDouble()
                {
                    return (double) WholePart + DecimalPart;
                }

                public uint AssignNewDecimalPartAndGetDifferenceAsUInt32(float decimalPart)
                {
                    uint oldDecimalPart = DecimalPartAsUInt32;
                    // Don't bother rounding, we don't want to accidentally round to 1.0.
                    uint newDecimalPart = (uint) (decimalPart * DecimalMax);
                    _value = (_value & WholeMask) | newDecimalPart;
                    return newDecimalPart - oldDecimalPart;
                }

                public uint GetDifferenceToNextWholeAsUInt32()
                {
                    return DecimalMax - DecimalPartAsUInt32;
                }

                public UnsignedFixed32 GetIncrementedWholeTruncated()
                {
#if PROMISE_DEBUG
                    checked
#endif
                    {
                        return new UnsignedFixed32()
                        {
                            _value = (_value & WholeMask) + (1u << Config.ProgressDecimalBits)
                        };
                    }
                }

                public void Increment(uint increment)
                {
                    _value += increment;
                }

                public static bool operator >(UnsignedFixed32 a, UnsignedFixed32 b)
                {
                    return a._value > b._value;
                }

                public static bool operator <(UnsignedFixed32 a, UnsignedFixed32 b)
                {
                    return a._value < b._value;
                }
            }

            public interface IInvokable
            {
                void Invoke();
            }

            public interface IProgressListener
            {
                void SetInitialAmount(UnsignedFixed32 amount);
                void IncrementProgress(Promise sender, uint amount);
                void ResolveOrIncrementProgress(Promise sender, uint amount);
                void CancelOrIncrementProgress(Promise sender, uint amount);
                void Retain();
            }

            partial interface IMultiTreeHandleable
            {
                void IncrementProgress(uint increment, UnsignedFixed32 senderAmount, UnsignedFixed32 ownerAmount);
                void CancelOrIncrementProgress(uint increment, UnsignedFixed32 senderAmount, UnsignedFixed32 ownerAmount);
            }

            public sealed class ProgressDelegate : IProgressListener, IInvokable, ITreeHandleable, IStacktraceable
            {
#if PROMISE_DEBUG
                string IStacktraceable.Stacktrace { get; set; }
#endif
                ITreeHandleable ILinked<ITreeHandleable>.Next { get; set; }

                private Action<float> _onProgress;
                private Promise _owner;
                private UnsignedFixed32 _current;
                uint _retainCounter;
                private bool _handling;
                private bool _done;
                private bool _suspended;
                private bool _canceled;

                private static ValueLinkedStackZeroGC<IProgressListener> _pool;

                static ProgressDelegate()
                {
                    OnClearPool += () => _pool.ClearAndDontRepool();
                }

                private ProgressDelegate() { }

                public static ProgressDelegate GetOrCreate(Action<float> onProgress, Promise owner, int skipFrames)
                {
                    var progress = _pool.IsNotEmpty ? (ProgressDelegate) _pool.Pop() : new ProgressDelegate();
                    progress._onProgress = onProgress;
                    progress._owner = owner;
                    progress._handling = false;
                    progress._done = false;
                    progress._suspended = false;
                    progress._canceled = false;
                    progress._current = default(UnsignedFixed32);
                    SetCreatedStacktrace(progress, skipFrames + 1);
                    return progress;
                }

                private void InvokeAndCatch(Action<float> callback, float progress)
                {
                    try
                    {
                        callback.Invoke(progress);
                    }
                    catch (Exception e)
                    {
                        UnhandledExceptionException unhandledException = UnhandledExceptionException.GetOrCreate(e);
                        SetStacktraceFromCreated(this, unhandledException);
                        AddRejectionToUnhandledStack(unhandledException);
                    }
                }

                void IInvokable.Invoke()
                {
                    _handling = false;
                    if (_done)
                    {
                        Dispose();
                        return;
                    }
                    if (_suspended | _canceled)
                    {
                        return;
                    }

                    // Calculate the normalized progress for the depth that the listener was added.
                    // Use double for better precision.
                    double expected = _owner._waitDepthAndProgress.WholePart + 1u;
                    InvokeAndCatch(_onProgress, (float) (_current.ToDouble() / expected));
                }

                private void IncrementProgress(Promise sender, uint amount)
                {
                    _current.Increment(amount);
                    _suspended = false;
                    if (!_handling & !_canceled)
                    {
                        _handling = true;
                        // This is called by the promise in reverse order that listeners were added, adding to the front reverses that and puts them in proper order.
                        AddToFrontOfProgressQueue(this);
                    }
                }

                void IProgressListener.IncrementProgress(Promise sender, uint amount)
                {
                    IncrementProgress(sender, amount);
                }

                void IProgressListener.ResolveOrIncrementProgress(Promise sender, uint amount)
                {
                    if (sender == _owner)
                    {
                        if (_canceled)
                        {
                            MarkOrDispose();
                        }
                        else
                        {
                            // Add to the owner's branches to invoke this with a value of 1.
                            _owner._nextBranches.Push(this);
                            _canceled = true;
                        }
                    }
                    else
                    {
                        IncrementProgress(sender, amount);
                        Release();
                    }
                }

                void IProgressListener.SetInitialAmount(UnsignedFixed32 amount)
                {
                    _current = amount;
                    _handling = true;
                    // Always add new listeners to the back.
                    AddToBackOfProgressQueue(this);
                }

                void IProgressListener.CancelOrIncrementProgress(Promise sender, uint amount)
                {
                    if (sender == _owner)
                    {
                        _canceled = true;
                        Release();
                    }
                    else
                    {
                        _suspended = true;
                        _current.Increment(amount);
                    }
                }

                void IProgressListener.Retain()
                {
                    ++_retainCounter;
                }

                private void MarkOrDispose()
                {
                    if (_handling)
                    {
                        // Mark done so Invoke will dispose.
                        _done = true;
                    }
                    else
                    {
                        // Dispose only if it's not in the progress queue.
                        Dispose();
                    }
                }

                private void Release()
                {
                    if (--_retainCounter == 0)
                    {
                        MarkOrDispose();
                    }
                }

                private void Dispose()
                {
                    _onProgress = null;
                    _owner = null;
                    if (Config.ObjectPooling != PoolType.None)
                    {
                        _pool.Push(this);
                    }
                }

                void ITreeHandleable.Handle()
                {
                    InvokeAndCatch(_onProgress, 1f);
                    _retainCounter = 0;
                    MarkOrDispose();
                }

                void ITreeHandleable.Cancel() { throw new System.InvalidOperationException(); }
            }

            partial class PromiseWaitPromise<TPromise> : IProgressListener, IInvokable
            {
                // This is used to avoid rounding errors when normalizing the progress.
                private UnsignedFixed32 _currentAmount;
                private bool _invokingProgress;
                private bool _secondPrevious;
                private bool _suspended;

                protected override void Reset(int skipFrames)
                {
                    base.Reset(skipFrames + 1);
                    _invokingProgress = false;
                    _secondPrevious = false;
                    _suspended = false;
                }

                protected override bool SubscribeProgressAndContinueLoop(ref IProgressListener progressListener, out Promise previous)
                {
                    // This is guaranteed to be pending.
                    bool firstSubscribe = _progressListeners.IsEmpty;
                    progressListener.Retain();
                    _progressListeners.Push(progressListener);
                    previous = _rejectedOrCanceledValueOrPrevious as Promise;
                    if (_secondPrevious)
                    {
                        if (firstSubscribe)
                        {
                            // Subscribe this to the returned promise.
                            progressListener = this;
                        }
                        return firstSubscribe;
                    }
                    return true;
                }

                protected override bool SubscribeProgressIfWaiterAndContinueLoop(ref IProgressListener progressListener, out Promise previous, ref ValueLinkedStack<PromisePassThrough> passThroughs)
                {
                    if (_state != State.Pending)
                    {
                        previous = null;
                        return false;
                    }
                    return SubscribeProgressAndContinueLoop(ref progressListener, out previous);
                }

                protected override sealed void SetDepth(UnsignedFixed32 previousDepth)
                {
                    _waitDepthAndProgress = previousDepth.GetIncrementedWholeTruncated();
                }

                void IInvokable.Invoke()
                {
                    if (_state != State.Pending | _suspended)
                    {
                        return;
                    }

                    _invokingProgress = false;

                    // Calculate the normalized progress for the depth of the returned promise.
                    // Use double for better precision.
                    double expected = ((Promise) _rejectedOrCanceledValueOrPrevious)._waitDepthAndProgress.WholePart + 1u;
                    float progress = (float) (_currentAmount.ToDouble() / expected);

                    uint increment = _waitDepthAndProgress.AssignNewDecimalPartAndGetDifferenceAsUInt32(progress);

                    foreach (var progressListener in _progressListeners)
                    {
                        progressListener.IncrementProgress(this, increment);
                    }
                }

                void IProgressListener.SetInitialAmount(UnsignedFixed32 amount)
                {
                    _currentAmount = amount;
                    _invokingProgress = true;
                    AddToFrontOfProgressQueue(this);
                }

                private void IncrementProgress(uint amount)
                {
                    _suspended = false;
                    _currentAmount.Increment(amount);
                    if (!_invokingProgress)
                    {
                        _invokingProgress = true;
                        AddToFrontOfProgressQueue(this);
                    }
                }

                void IProgressListener.IncrementProgress(Promise sender, uint amount)
                {
                    IncrementProgress(amount);
                }

                void IProgressListener.ResolveOrIncrementProgress(Promise sender, uint amount)
                {
                    IncrementProgress(amount);
                    ReleaseWithoutDisposeCheck();
                }

                void IProgressListener.CancelOrIncrementProgress(Promise sender, uint amount)
                {
                    _suspended = true;
                    _currentAmount.Increment(amount);
                    ReleaseWithoutDisposeCheck();
                }
            }

            partial class PromiseWaitPromise<T, TPromise> : IProgressListener, IInvokable
            {
                // This is used to avoid rounding errors when normalizing the progress.
                private UnsignedFixed32 _currentAmount;
                private bool _invokingProgress;
                private bool _secondPrevious;
                private bool _suspended;

                protected override void Reset(int skipFrames)
                {
                    base.Reset(skipFrames + 1);
                    _invokingProgress = false;
                    _secondPrevious = false;
                    _suspended = false;
                }

                protected override bool SubscribeProgressAndContinueLoop(ref IProgressListener progressListener, out Promise previous)
                {
                    // This is guaranteed to be pending.
                    bool firstSubscribe = _progressListeners.IsEmpty;
                    progressListener.Retain();
                    _progressListeners.Push(progressListener);
                    previous = _rejectedOrCanceledValueOrPrevious as Promise;
                    if (_secondPrevious)
                    {
                        if (firstSubscribe)
                        {
                            // Subscribe this to the returned promise.
                            progressListener = this;
                        }
                        return firstSubscribe;
                    }
                    return true;
                }

                protected override bool SubscribeProgressIfWaiterAndContinueLoop(ref IProgressListener progressListener, out Promise previous, ref ValueLinkedStack<PromisePassThrough> passThroughs)
                {
                    if (_state != State.Pending)
                    {
                        previous = null;
                        return false;
                    }
                    return SubscribeProgressAndContinueLoop(ref progressListener, out previous);
                }

                protected override sealed void SetDepth(UnsignedFixed32 previousDepth)
                {
                    _waitDepthAndProgress = previousDepth.GetIncrementedWholeTruncated();
                }

                void IInvokable.Invoke()
                {
                    if (_state != State.Pending | _suspended)
                    {
                        return;
                    }

                    _invokingProgress = false;

                    // Calculate the normalized progress for the depth of the cached promise.
                    // Use double for better precision.
                    double expected = ((Promise) _rejectedOrCanceledValueOrPrevious)._waitDepthAndProgress.WholePart + 1u;
                    float progress = (float) (_currentAmount.ToDouble() / expected);

                    uint increment = _waitDepthAndProgress.AssignNewDecimalPartAndGetDifferenceAsUInt32(progress);

                    foreach (var progressListener in _progressListeners)
                    {
                        progressListener.IncrementProgress(this, increment);
                    }
                }

                void IProgressListener.SetInitialAmount(UnsignedFixed32 amount)
                {
                    _currentAmount = amount;
                    _invokingProgress = true;
                    AddToFrontOfProgressQueue(this);
                }

                private void IncrementProgress(uint amount)
                {
                    _suspended = false;
                    _currentAmount.Increment(amount);
                    if (!_invokingProgress)
                    {
                        _invokingProgress = true;
                        AddToFrontOfProgressQueue(this);
                    }
                }

                void IProgressListener.IncrementProgress(Promise sender, uint amount)
                {
                    IncrementProgress(amount);
                }

                void IProgressListener.ResolveOrIncrementProgress(Promise sender, uint amount)
                {
                    IncrementProgress(amount);
                    ReleaseWithoutDisposeCheck();
                }

                void IProgressListener.CancelOrIncrementProgress(Promise sender, uint amount)
                {
                    _suspended = true;
                    _currentAmount.Increment(amount);
                    ReleaseWithoutDisposeCheck();
                }
            }

            partial class PromiseWaitDeferred<TPromise>
            {
                protected override bool SubscribeProgressIfWaiterAndContinueLoop(ref IProgressListener progressListener, out Promise previous, ref ValueLinkedStack<PromisePassThrough> passThroughs)
                {
                    if (_state != State.Pending)
                    {
                        previous = null;
                        return false;
                    }
                    return SubscribeProgressAndContinueLoop(ref progressListener, out previous);
                }

                protected override sealed void SetDepth(UnsignedFixed32 previousDepth)
                {
                    _waitDepthAndProgress = previousDepth.GetIncrementedWholeTruncated();
                }
            }

            partial class PromiseWaitDeferred<T, TPromise>
            {
                protected override bool SubscribeProgressIfWaiterAndContinueLoop(ref IProgressListener progressListener, out Promise previous, ref ValueLinkedStack<PromisePassThrough> passThroughs)
                {
                    if (_state != State.Pending)
                    {
                        previous = null;
                        return false;
                    }
                    return SubscribeProgressAndContinueLoop(ref progressListener, out previous);
                }

                protected override sealed void SetDepth(UnsignedFixed32 previousDepth)
                {
                    _waitDepthAndProgress = previousDepth.GetIncrementedWholeTruncated();
                }
            }

            partial class PromisePassThrough : IProgressListener
            {
                void IProgressListener.SetInitialAmount(UnsignedFixed32 amount)
                {
                    Target.IncrementProgress(amount.ToUInt32(), amount, Owner._waitDepthAndProgress);
                }

                void IProgressListener.IncrementProgress(Promise sender, uint amount)
                {
                    Target.IncrementProgress(amount, sender._waitDepthAndProgress, Owner._waitDepthAndProgress);
                }

                void IProgressListener.ResolveOrIncrementProgress(Promise sender, uint amount)
                {
                    Release();
                }

                void IProgressListener.CancelOrIncrementProgress(Promise sender, uint amount)
                {
                    Target.CancelOrIncrementProgress(amount, sender._waitDepthAndProgress, Owner._waitDepthAndProgress);
                    Release();
                }
            }

            partial class AllPromise0 : IInvokable
            {
                // These are used to avoid rounding errors when normalizing the progress.
                private float _expected;
                private UnsignedFixed32 _currentAmount;
                private bool _invokingProgress;
                private bool _suspended;

                protected override void Reset(int skipFrames)
                {
#if PROMISE_DEBUG
                    checked
#endif
                    {
                        base.Reset(skipFrames + 1);
                        _currentAmount = default(UnsignedFixed32);
                        _invokingProgress = false;
                        _suspended = false;

                        uint expectedProgressCounter = 0;
                        uint maxWaitDepth = 0;
                        foreach (var passThrough in passThroughs)
                        {
                            uint waitDepth = passThrough.Owner._waitDepthAndProgress.WholePart;
                            expectedProgressCounter += waitDepth;
                            maxWaitDepth = Math.Max(maxWaitDepth, waitDepth);
                        }
                        _expected = expectedProgressCounter + _waitCount;

                        // Use the longest chain as this depth.
                        _waitDepthAndProgress = new UnsignedFixed32(maxWaitDepth);
                    }
                }

                partial void IncrementProgress(Promise feed)
                {
                    bool subscribedProgress = _progressListeners.IsNotEmpty;
                    uint increment = subscribedProgress ? feed._waitDepthAndProgress.GetDifferenceToNextWholeAsUInt32() : feed._waitDepthAndProgress.GetIncrementedWholeTruncated().ToUInt32();
                    IncrementProgress(increment);
                }

                protected override bool SubscribeProgressAndContinueLoop(ref IProgressListener progressListener, out Promise previous)
                {
                    // This is guaranteed to be pending.
                    previous = this;
                    return true;
                }

                protected override bool SubscribeProgressIfWaiterAndContinueLoop(ref IProgressListener progressListener, out Promise previous, ref ValueLinkedStack<PromisePassThrough> passThroughs)
                {
                    bool firstSubscribe = _progressListeners.IsEmpty;
                    progressListener.Retain();
                    _progressListeners.Push(progressListener);
                    if (firstSubscribe & _state == State.Pending)
                    {
                        BorrowPassthroughs(ref passThroughs);
                    }

                    previous = null;
                    return false;
                }

                void IMultiTreeHandleable.IncrementProgress(uint amount, UnsignedFixed32 senderAmount, UnsignedFixed32 ownerAmount)
                {
                    IncrementProgress(amount);
                }

                void IMultiTreeHandleable.CancelOrIncrementProgress(uint amount, UnsignedFixed32 senderAmount, UnsignedFixed32 ownerAmount)
                {
                    _suspended = true;
                    _currentAmount.Increment(amount);
                }

                private void IncrementProgress(uint amount)
                {
                    _suspended = false;
                    _currentAmount.Increment(amount);
                    if (!_invokingProgress & _state == State.Pending)
                    {
                        _invokingProgress = true;
                        AddToFrontOfProgressQueue(this);
                    }
                }

                protected override uint GetIncrementMultiplier()
                {
                    return _waitDepthAndProgress.WholePart + 1u;
                }

                void IInvokable.Invoke()
                {
                    if (_state != State.Pending | _suspended)
                    {
                        return;
                    }

                    _invokingProgress = false;

                    // Calculate the normalized progress for all the awaited promises.
                    // Use double for better precision.
                    float progress = (float) (_currentAmount.ToDouble() / _expected);

                    uint increment = _waitDepthAndProgress.AssignNewDecimalPartAndGetDifferenceAsUInt32(progress) * GetIncrementMultiplier();

                    foreach (var progressListener in _progressListeners)
                    {
                        progressListener.IncrementProgress(this, increment);
                    }
                }
            }

            partial class AllPromise<T> : IInvokable
            {
                // These are used to avoid rounding errors when normalizing the progress.
                private float _expected;
                private UnsignedFixed32 _currentAmount;
                private bool _invokingProgress;
                private bool _suspended;

                protected override void Reset(int skipFrames)
                {
#if PROMISE_DEBUG
                    checked
#endif
                    {
                        base.Reset(skipFrames + 1);
                        _currentAmount = default(UnsignedFixed32);
                        _invokingProgress = false;
                        _suspended = false;

                        uint expectedProgressCounter = 0;
                        uint maxWaitDepth = 0;
                        foreach (var passThrough in passThroughs)
                        {
                            uint waitDepth = passThrough.Owner._waitDepthAndProgress.WholePart;
                            expectedProgressCounter += waitDepth;
                            maxWaitDepth = Math.Max(maxWaitDepth, waitDepth);
                        }
                        _expected = expectedProgressCounter + _waitCount;

                        // Use the longest chain as this depth.
                        _waitDepthAndProgress = new UnsignedFixed32(maxWaitDepth);
                    }
                }

                partial void IncrementProgress(Promise feed)
                {
                    bool subscribedProgress = _progressListeners.IsNotEmpty;
                    uint increment = subscribedProgress ? feed._waitDepthAndProgress.GetDifferenceToNextWholeAsUInt32() : feed._waitDepthAndProgress.GetIncrementedWholeTruncated().ToUInt32();
                    IncrementProgress(increment);
                }

                protected override bool SubscribeProgressAndContinueLoop(ref IProgressListener progressListener, out Promise previous)
                {
                    // This is guaranteed to be pending.
                    previous = this;
                    return true;
                }

                protected override bool SubscribeProgressIfWaiterAndContinueLoop(ref IProgressListener progressListener, out Promise previous, ref ValueLinkedStack<PromisePassThrough> passThroughs)
                {
                    bool firstSubscribe = _progressListeners.IsEmpty;
                    progressListener.Retain();
                    _progressListeners.Push(progressListener);
                    if (firstSubscribe & _state == State.Pending)
                    {
                        BorrowPassthroughs(ref passThroughs);
                    }

                    previous = null;
                    return false;
                }

                void IMultiTreeHandleable.IncrementProgress(uint amount, UnsignedFixed32 senderAmount, UnsignedFixed32 ownerAmount)
                {
                    IncrementProgress(amount);
                }

                void IMultiTreeHandleable.CancelOrIncrementProgress(uint amount, UnsignedFixed32 senderAmount, UnsignedFixed32 ownerAmount)
                {
                    _suspended = true;
                    _currentAmount.Increment(amount);
                }

                private void IncrementProgress(uint amount)
                {
                    _suspended = false;
                    _currentAmount.Increment(amount);
                    if (!_invokingProgress & _state == State.Pending)
                    {
                        _invokingProgress = true;
                        AddToFrontOfProgressQueue(this);
                    }
                }

                protected override uint GetIncrementMultiplier()
                {
                    return _waitDepthAndProgress.WholePart + 1u;
                }

                void IInvokable.Invoke()
                {
                    if (_state != State.Pending | _suspended)
                    {
                        return;
                    }

                    _invokingProgress = false;

                    // Calculate the normalized progress for all the awaited promises.
                    // Use double for better precision.
                    float progress = (float) (_currentAmount.ToDouble() / _expected);

                    uint increment = _waitDepthAndProgress.AssignNewDecimalPartAndGetDifferenceAsUInt32(progress) * GetIncrementMultiplier();

                    foreach (var progressListener in _progressListeners)
                    {
                        progressListener.IncrementProgress(this, increment);
                    }
                }
            }

            partial class RacePromise0 : IInvokable
            {
                private UnsignedFixed32 _currentAmount;
                private bool _invokingProgress;
                private bool _suspended;

                protected override void Reset(int skipFrames)
                {
                    base.Reset(skipFrames + 1);
                    _currentAmount = default(UnsignedFixed32);
                    _invokingProgress = false;
                    _suspended = false;

                    uint minWaitDepth = uint.MaxValue;
                    foreach (var passThrough in passThroughs)
                    {
                        minWaitDepth = Math.Min(minWaitDepth, passThrough.Owner._waitDepthAndProgress.WholePart);
                    }

                    // Expect the shortest chain to finish first.
                    _waitDepthAndProgress = new UnsignedFixed32(minWaitDepth);
                }

                protected override bool SubscribeProgressAndContinueLoop(ref IProgressListener progressListener, out Promise previous)
                {
                    // This is guaranteed to be pending.
                    previous = this;
                    return true;
                }

                protected override bool SubscribeProgressIfWaiterAndContinueLoop(ref IProgressListener progressListener, out Promise previous, ref ValueLinkedStack<PromisePassThrough> passThroughs)
                {
                    bool firstSubscribe = _progressListeners.IsEmpty;
                    progressListener.Retain();
                    _progressListeners.Push(progressListener);
                    if (firstSubscribe & _state == State.Pending)
                    {
                        BorrowPassthroughs(ref passThroughs);
                    }

                    previous = null;
                    return false;
                }

                void IMultiTreeHandleable.IncrementProgress(uint amount, UnsignedFixed32 senderAmount, UnsignedFixed32 ownerAmount)
                {
                    _suspended = false;
                    // Use double for better precision.
                    float progress = (float) ((double) senderAmount.ToUInt32() * (double) GetIncrementMultiplier() / (double) ownerAmount.GetIncrementedWholeTruncated().ToUInt32());
                    var newAmount = new UnsignedFixed32(progress);
                    if (newAmount > _currentAmount)
                    {
                        _currentAmount = newAmount;
                        if (!_invokingProgress)
                        {
                            _invokingProgress = true;
                            AddToFrontOfProgressQueue(this);
                        }
                    }
                }

                void IMultiTreeHandleable.CancelOrIncrementProgress(uint amount, UnsignedFixed32 senderAmount, UnsignedFixed32 ownerAmount)
                {
                    _suspended = true;
                    // Use double for better precision.
                    float progress = (float) ((double) senderAmount.ToUInt32() * (double) GetIncrementMultiplier() / (double) ownerAmount.GetIncrementedWholeTruncated().ToUInt32());
                    var newAmount = new UnsignedFixed32(progress);
                    if (newAmount > _currentAmount)
                    {
                        _currentAmount = newAmount;
                    }
                }

                protected override uint GetIncrementMultiplier()
                {
                    return _waitDepthAndProgress.WholePart + 1u;
                }

                void IInvokable.Invoke()
                {
                    if (_state != State.Pending | _suspended)
                    {
                        return;
                    }

                    _invokingProgress = false;

                    uint multiplier = GetIncrementMultiplier();

                    // Calculate the normalized progress.
                    // Use double for better precision.
                    float progress = (float) (_currentAmount.ToDouble() / multiplier);

                    uint increment = _waitDepthAndProgress.AssignNewDecimalPartAndGetDifferenceAsUInt32(progress) * multiplier;

                    foreach (var progressListener in _progressListeners)
                    {
                        progressListener.IncrementProgress(this, increment);
                    }
                }
            }

            partial class RacePromise<T> : IInvokable
            {
                private UnsignedFixed32 _currentAmount;
                private bool _invokingProgress;
                private bool _suspended;

                protected override void Reset(int skipFrames)
                {
                    base.Reset(skipFrames + 1);
                    _currentAmount = default(UnsignedFixed32);
                    _invokingProgress = false;
                    _suspended = false;

                    uint minWaitDepth = uint.MaxValue;
                    foreach (var passThrough in passThroughs)
                    {
                        minWaitDepth = Math.Min(minWaitDepth, passThrough.Owner._waitDepthAndProgress.WholePart);
                    }

                    // Expect the shortest chain to finish first.
                    _waitDepthAndProgress = new UnsignedFixed32(minWaitDepth);
                }

                protected override bool SubscribeProgressAndContinueLoop(ref IProgressListener progressListener, out Promise previous)
                {
                    // This is guaranteed to be pending.
                    previous = this;
                    return true;
                }

                protected override bool SubscribeProgressIfWaiterAndContinueLoop(ref IProgressListener progressListener, out Promise previous, ref ValueLinkedStack<PromisePassThrough> passThroughs)
                {
                    bool firstSubscribe = _progressListeners.IsEmpty;
                    progressListener.Retain();
                    _progressListeners.Push(progressListener);
                    if (firstSubscribe & _state == State.Pending)
                    {
                        BorrowPassthroughs(ref passThroughs);
                    }

                    previous = null;
                    return false;
                }

                void IMultiTreeHandleable.IncrementProgress(uint amount, UnsignedFixed32 senderAmount, UnsignedFixed32 ownerAmount)
                {
                    _suspended = false;
                    // Use double for better precision.
                    float progress = (float) ((double) senderAmount.ToUInt32() * (double) GetIncrementMultiplier() / (double) ownerAmount.GetIncrementedWholeTruncated().ToUInt32());
                    var newAmount = new UnsignedFixed32(progress);
                    if (newAmount > _currentAmount)
                    {
                        _currentAmount = newAmount;
                        if (!_invokingProgress)
                        {
                            _invokingProgress = true;
                            AddToFrontOfProgressQueue(this);
                        }
                    }
                }

                void IMultiTreeHandleable.CancelOrIncrementProgress(uint amount, UnsignedFixed32 senderAmount, UnsignedFixed32 ownerAmount)
                {
                    _suspended = true;
                    // Use double for better precision.
                    float progress = (float) ((double) senderAmount.ToUInt32() * (double) GetIncrementMultiplier() / (double) ownerAmount.GetIncrementedWholeTruncated().ToUInt32());
                    var newAmount = new UnsignedFixed32(progress);
                    if (newAmount > _currentAmount)
                    {
                        _currentAmount = newAmount;
                    }
                }

                protected override uint GetIncrementMultiplier()
                {
                    return _waitDepthAndProgress.WholePart + 1u;
                }

                void IInvokable.Invoke()
                {
                    if (_state != State.Pending | _suspended)
                    {
                        return;
                    }

                    _invokingProgress = false;

                    uint multiplier = GetIncrementMultiplier();

                    // Calculate the normalized progress.
                    // Use double for better precision.
                    float progress = (float) (_currentAmount.ToDouble() / multiplier);

                    uint increment = _waitDepthAndProgress.AssignNewDecimalPartAndGetDifferenceAsUInt32(progress) * multiplier;

                    foreach (var progressListener in _progressListeners)
                    {
                        progressListener.IncrementProgress(this, increment);
                    }
                }
            }

            partial class SequencePromise0
            {
                // Only wrap the promise to normalize its progress. If we're not using progress, we can just use the promise as-is.
                static partial void GetFirstPromise(ref Promise promise, int skipFrames)
                {
                    var newPromise = _pool.IsNotEmpty ? (SequencePromise0) _pool.Pop() : new SequencePromise0();
                    newPromise.Reset(skipFrames + 1);
                    newPromise.ResetDepth();
                    newPromise.WaitFor(promise);
                    promise = newPromise;
                }
            }

            partial class FirstPromise0 : IInvokable
            {
                private UnsignedFixed32 _currentAmount;
                private bool _invokingProgress;
                private bool _suspended;

                protected override void Reset(int skipFrames)
                {
                    base.Reset(skipFrames + 1);
                    _currentAmount = default(UnsignedFixed32);
                    _invokingProgress = false;
                    _suspended = false;

                    uint minWaitDepth = uint.MaxValue;
                    foreach (var passThrough in passThroughs)
                    {
                        minWaitDepth = Math.Min(minWaitDepth, passThrough.Owner._waitDepthAndProgress.WholePart);
                    }

                    // Expect the shortest chain to finish first.
                    _waitDepthAndProgress = new UnsignedFixed32(minWaitDepth);
                }

                protected override bool SubscribeProgressAndContinueLoop(ref IProgressListener progressListener, out Promise previous)
                {
                    // This is guaranteed to be pending.
                    previous = this;
                    return true;
                }

                protected override bool SubscribeProgressIfWaiterAndContinueLoop(ref IProgressListener progressListener, out Promise previous, ref ValueLinkedStack<PromisePassThrough> passThroughs)
                {
                    bool firstSubscribe = _progressListeners.IsEmpty;
                    progressListener.Retain();
                    _progressListeners.Push(progressListener);
                    if (firstSubscribe & _state == State.Pending)
                    {
                        BorrowPassthroughs(ref passThroughs);
                    }

                    previous = null;
                    return false;
                }

                void IMultiTreeHandleable.IncrementProgress(uint amount, UnsignedFixed32 senderAmount, UnsignedFixed32 ownerAmount)
                {
                    _suspended = false;
                    // Use double for better precision.
                    float progress = (float) ((double) senderAmount.ToUInt32() * (double) GetIncrementMultiplier() / (double) ownerAmount.GetIncrementedWholeTruncated().ToUInt32());
                    var newAmount = new UnsignedFixed32(progress);
                    if (newAmount > _currentAmount)
                    {
                        _currentAmount = newAmount;
                        if (!_invokingProgress)
                        {
                            _invokingProgress = true;
                            AddToFrontOfProgressQueue(this);
                        }
                    }
                }

                void IMultiTreeHandleable.CancelOrIncrementProgress(uint amount, UnsignedFixed32 senderAmount, UnsignedFixed32 ownerAmount)
                {
                    _suspended = true;
                    // Use double for better precision.
                    float progress = (float) ((double) senderAmount.ToUInt32() * (double) GetIncrementMultiplier() / (double) ownerAmount.GetIncrementedWholeTruncated().ToUInt32());
                    var newAmount = new UnsignedFixed32(progress);
                    if (newAmount > _currentAmount)
                    {
                        _currentAmount = newAmount;
                    }
                }

                protected override uint GetIncrementMultiplier()
                {
                    return _waitDepthAndProgress.WholePart + 1u;
                }

                void IInvokable.Invoke()
                {
                    if (_state != State.Pending | _suspended)
                    {
                        return;
                    }

                    _invokingProgress = false;

                    uint multiplier = GetIncrementMultiplier();

                    // Calculate the normalized progress.
                    // Use double for better precision.
                    float progress = (float) (_currentAmount.ToDouble() / multiplier);

                    uint increment = _waitDepthAndProgress.AssignNewDecimalPartAndGetDifferenceAsUInt32(progress) * multiplier;

                    foreach (var progressListener in _progressListeners)
                    {
                        progressListener.IncrementProgress(this, increment);
                    }
                }
            }

            partial class FirstPromise<T> : IInvokable
            {
                private UnsignedFixed32 _currentAmount;
                private bool _invokingProgress;
                private bool _suspended;

                protected override void Reset(int skipFrames)
                {
                    base.Reset(skipFrames + 1);
                    _currentAmount = default(UnsignedFixed32);
                    _invokingProgress = false;
                    _suspended = false;

                    uint minWaitDepth = uint.MaxValue;
                    foreach (var passThrough in passThroughs)
                    {
                        minWaitDepth = Math.Min(minWaitDepth, passThrough.Owner._waitDepthAndProgress.WholePart);
                    }

                    // Expect the shortest chain to finish first.
                    _waitDepthAndProgress = new UnsignedFixed32(minWaitDepth);
                }

                protected override bool SubscribeProgressAndContinueLoop(ref IProgressListener progressListener, out Promise previous)
                {
                    // This is guaranteed to be pending.
                    previous = this;
                    return true;
                }

                protected override bool SubscribeProgressIfWaiterAndContinueLoop(ref IProgressListener progressListener, out Promise previous, ref ValueLinkedStack<PromisePassThrough> passThroughs)
                {
                    bool firstSubscribe = _progressListeners.IsEmpty;
                    progressListener.Retain();
                    _progressListeners.Push(progressListener);
                    if (firstSubscribe & _state == State.Pending)
                    {
                        BorrowPassthroughs(ref passThroughs);
                    }

                    previous = null;
                    return false;
                }

                void IMultiTreeHandleable.IncrementProgress(uint amount, UnsignedFixed32 senderAmount, UnsignedFixed32 ownerAmount)
                {
                    _suspended = false;
                    // Use double for better precision.
                    float progress = (float) ((double) senderAmount.ToUInt32() * (double) GetIncrementMultiplier() / (double) ownerAmount.GetIncrementedWholeTruncated().ToUInt32());
                    var newAmount = new UnsignedFixed32(progress);
                    if (newAmount > _currentAmount)
                    {
                        _currentAmount = newAmount;
                        if (!_invokingProgress)
                        {
                            _invokingProgress = true;
                            AddToFrontOfProgressQueue(this);
                        }
                    }
                }

                void IMultiTreeHandleable.CancelOrIncrementProgress(uint amount, UnsignedFixed32 senderAmount, UnsignedFixed32 ownerAmount)
                {
                    _suspended = true;
                    // Use double for better precision.
                    float progress = (float) ((double) senderAmount.ToUInt32() * (double) GetIncrementMultiplier() / (double) ownerAmount.GetIncrementedWholeTruncated().ToUInt32());
                    var newAmount = new UnsignedFixed32(progress);
                    if (newAmount > _currentAmount)
                    {
                        _currentAmount = newAmount;
                    }
                }

                protected override uint GetIncrementMultiplier()
                {
                    return _waitDepthAndProgress.WholePart + 1u;
                }

                void IInvokable.Invoke()
                {
                    if (_state != State.Pending | _suspended)
                    {
                        return;
                    }

                    _invokingProgress = false;

                    uint multiplier = GetIncrementMultiplier();

                    // Calculate the normalized progress.
                    // Use double for better precision.
                    float progress = (float) (_currentAmount.ToDouble() / multiplier);

                    uint increment = _waitDepthAndProgress.AssignNewDecimalPartAndGetDifferenceAsUInt32(progress) * multiplier;

                    foreach (var progressListener in _progressListeners)
                    {
                        progressListener.IncrementProgress(this, increment);
                    }
                }
            }
        }
#endif
    }

    partial class Promise<T>
    {
        static partial void ValidateProgress(int skipFrames);
#if PROMISE_PROGRESS
        /// <summary>
        /// Add a progress listener. <paramref name="onProgress"/> will be invoked with progress that is normalized between 0 and 1 from this and all previous waiting promises in the chain.
        /// Returns this.
        /// </summary>
        public new Promise<T> Progress(Action<float> onProgress)
        {
            SubscribeProgress(onProgress, 1);
            return this;
        }
#else
        static partial void ValidateProgress(int skipFrames)
        {
            ThrowProgressException(skipFrames + 1);
        }
#endif
    }
}