﻿#if !UNITY_EDITOR || CSHARP_7_OR_LATER

#if !PROTO_PROMISE_PROGRESS_DISABLE
#define PROMISE_PROGRESS
#else
#undef PROMISE_PROGRESS
#endif

using NUnit.Framework;
using System.Threading;

namespace Proto.Promises.Tests.Threading
{
    public class DeferredConcurrencyTests
    {
        [TearDown]
        public void Teardown()
        {
            TestHelper.Cleanup();
        }

#if PROMISE_PROGRESS
        [Test]
        public void DeferredReportProgressMayBeCalledConcurrently_void0()
        {
            float progress1 = float.NaN;
            float progress2 = float.NaN;
            var deferred = Promise.NewDeferred();
            deferred.Promise
                .Progress(v => progress1 = v)
                .Progress(v => progress2 = v)
                .Forget();

            var threadHelper = new ThreadHelper();
            threadHelper.ExecuteParallelActionsWithOffsets(true,
                // Setup
                () => progress1 = progress2 = float.NaN,
                // Teardown
                () =>
                {
                    // Progress isn't reported until manager handles it.
                    Assert.IsNaN(progress1);
                    Assert.IsNaN(progress2);
                    Promise.Manager.HandleCompletesAndProgress();
                    // Both progress should be the same.
                    Assert.AreEqual(progress1, progress2);
                    // Each progress is reported concurrently, so we can't know which stuck.
                    // Just check to make sure any of them stuck, so it should be >= min and <= max.
                    Assert.Greater(progress1, 0.2f - TestHelper.progressEpsilon);
                    Assert.LessOrEqual(progress1, 0.4f);
                },
                // Parallel Actions
                () => deferred.ReportProgress(0.2f),
                () => deferred.ReportProgress(0.3f),
                () => deferred.ReportProgress(0.4f)
            );

            deferred.Resolve();
            Promise.Manager.HandleCompletesAndProgress();
        }

        [Test]
        public void DeferredReportProgressMayBeCalledConcurrently_void1()
        {
            float progress1 = float.NaN;
            float progress2 = float.NaN;
            var cancelationSource = CancelationSource.New();
            var deferred = Promise.NewDeferred(cancelationSource.Token);
            deferred.Promise
                .Progress(v => progress1 = v)
                .Progress(v => progress2 = v)
                .Forget();

            var threadHelper = new ThreadHelper();
            threadHelper.ExecuteParallelActionsWithOffsets(true,
                // Setup
                () => progress1 = progress2 = float.NaN,
                // Teardown
                () =>
                {
                    // Progress isn't reported until manager handles it.
                    Assert.IsNaN(progress1);
                    Assert.IsNaN(progress2);
                    Promise.Manager.HandleCompletesAndProgress();
                    // Both progress should be the same.
                    Assert.AreEqual(progress1, progress2);
                    // Each progress is reported concurrently, so we can't know which stuck.
                    // Just check to make sure any of them stuck, so it should be >= min and <= max.
                    Assert.Greater(progress1, 0.2f - TestHelper.progressEpsilon);
                    Assert.LessOrEqual(progress1, 0.4f);
                },
                // Parallel Actions
                () => deferred.ReportProgress(0.2f),
                () => deferred.ReportProgress(0.3f),
                () => deferred.ReportProgress(0.4f)
            );

            deferred.Resolve();
            cancelationSource.Dispose();
            Promise.Manager.HandleCompletesAndProgress();
        }

        [Test]
        public void DeferredReportProgressMayBeCalledConcurrently_T0()
        {
            float progress1 = float.NaN;
            float progress2 = float.NaN;
            var deferred = Promise.NewDeferred<int>();
            deferred.Promise
                .Progress(v => progress1 = v)
                .Progress(v => progress2 = v)
                .Forget();

            var threadHelper = new ThreadHelper();
            threadHelper.ExecuteParallelActionsWithOffsets(true,
                // Setup
                () => progress1 = progress2 = float.NaN,
                // Teardown
                () =>
                {
                    // Progress isn't reported until manager handles it.
                    Assert.IsNaN(progress1);
                    Assert.IsNaN(progress2);
                    Promise.Manager.HandleCompletesAndProgress();
                    // Both progress should be the same.
                    Assert.AreEqual(progress1, progress2);
                    // Each progress is reported concurrently, so we can't know which stuck.
                    // Just check to make sure any of them stuck, so it should be >= min and <= max.
                    Assert.Greater(progress1, 0.2f - TestHelper.progressEpsilon);
                    Assert.LessOrEqual(progress1, 0.4f);
                },
                // Parallel Actions
                () => deferred.ReportProgress(0.2f),
                () => deferred.ReportProgress(0.3f),
                () => deferred.ReportProgress(0.4f)
            );

            deferred.Resolve(1);
            Promise.Manager.HandleCompletesAndProgress();
        }

        [Test]
        public void DeferredReportProgressMayBeCalledConcurrently_T1()
        {
            float progress1 = float.NaN;
            float progress2 = float.NaN;
            var cancelationSource = CancelationSource.New();
            var deferred = Promise.NewDeferred<int>(cancelationSource.Token);
            deferred.Promise
                .Progress(v => progress1 = v)
                .Progress(v => progress2 = v)
                .Forget();

            var threadHelper = new ThreadHelper();
            threadHelper.ExecuteParallelActionsWithOffsets(true,
                // Setup
                () => progress1 = progress2 = float.NaN,
                // Teardown
                () =>
                {
                    // Progress isn't reported until manager handles it.
                    Assert.IsNaN(progress1);
                    Assert.IsNaN(progress2);
                    Promise.Manager.HandleCompletesAndProgress();
                    // Both progress should be the same.
                    Assert.AreEqual(progress1, progress2);
                    // Each progress is reported concurrently, so we can't know which stuck.
                    // Just check to make sure any of them stuck, so it should be >= min and <= max.
                    Assert.Greater(progress1, 0.2f - TestHelper.progressEpsilon);
                    Assert.LessOrEqual(progress1, 0.4f);
                },
                // Parallel Actions
                () => deferred.ReportProgress(0.2f),
                () => deferred.ReportProgress(0.3f),
                () => deferred.ReportProgress(0.4f)
            );

            deferred.Resolve(1);
            cancelationSource.Dispose();
            Promise.Manager.HandleCompletesAndProgress();
        }
#endif

        [Test]
        public void DeferredResolveMayNotBeCalledConcurrently_void0()
        {
            int invokedCount = 0;
            var deferred = Promise.NewDeferred();
            deferred.Promise
                .Then(() => { Interlocked.Increment(ref invokedCount); })
                .Forget();
            int failedTryResolveCount = 0;

            var threadHelper = new ThreadHelper();
            threadHelper.ExecuteMultiActionParallel(() =>
            {
                if (!deferred.TryResolve())
                {
                    Interlocked.Increment(ref failedTryResolveCount);
                }
            });

            Assert.AreEqual(ThreadHelper.multiThreadCount - 1, failedTryResolveCount); // TryResolve should succeed once.
            Assert.Zero(invokedCount); // Callback isn't executed until manager handles it.
            Promise.Manager.HandleCompletesAndProgress();
            Assert.AreEqual(1, invokedCount);
        }

        [Test]
        public void DeferredResolveMayNotBeCalledConcurrently_void1()
        {
            int invokedCount = 0;
            var cancelationSource = CancelationSource.New();
            var deferred = Promise.NewDeferred(cancelationSource.Token);
            deferred.Promise
                .Then(() => { Interlocked.Increment(ref invokedCount); })
                .Forget();
            int failedTryResolveCount = 0;

            var threadHelper = new ThreadHelper();
            threadHelper.ExecuteMultiActionParallel(() =>
            {
                if (!deferred.TryResolve())
                {
                    Interlocked.Increment(ref failedTryResolveCount);
                }
            });

            Assert.AreEqual(ThreadHelper.multiThreadCount - 1, failedTryResolveCount); // TryResolve should succeed once.
            Assert.Zero(invokedCount); // Callback isn't executed until manager handles it.
            Promise.Manager.HandleCompletesAndProgress();
            Assert.AreEqual(1, invokedCount);

            cancelationSource.Dispose();
        }

        [Test]
        public void DeferredResolveMayNotBeCalledConcurrently_T0()
        {
            int invokedCount = 0;
            var deferred = Promise.NewDeferred<int>();
            deferred.Promise
                .Then(v => { Interlocked.Increment(ref invokedCount); })
                .Forget();
            int failedTryResolveCount = 0;

            var threadHelper = new ThreadHelper();
            threadHelper.ExecuteMultiActionParallel(() =>
            {
                if (!deferred.TryResolve(1))
                {
                    Interlocked.Increment(ref failedTryResolveCount);
                }
            });

            Assert.AreEqual(ThreadHelper.multiThreadCount - 1, failedTryResolveCount); // TryResolve should succeed once.
            Assert.Zero(invokedCount); // Callback isn't executed until manager handles it.
            Promise.Manager.HandleCompletesAndProgress();
            Assert.AreEqual(1, invokedCount);
        }

        [Test]
        public void DeferredResolveMayNotBeCalledConcurrently_T1()
        {
            int invokedCount = 0;
            var cancelationSource = CancelationSource.New();
            var deferred = Promise.NewDeferred<int>(cancelationSource.Token);
            deferred.Promise
                .Then(v => { Interlocked.Increment(ref invokedCount); })
                .Forget();
            int failedTryResolveCount = 0;

            var threadHelper = new ThreadHelper();
            threadHelper.ExecuteMultiActionParallel(() =>
            {
                if (!deferred.TryResolve(1))
                {
                    Interlocked.Increment(ref failedTryResolveCount);
                }
            });

            Assert.AreEqual(ThreadHelper.multiThreadCount - 1, failedTryResolveCount); // TryResolve should succeed once.
            Assert.Zero(invokedCount); // Callback isn't executed until manager handles it.
            Promise.Manager.HandleCompletesAndProgress();
            Assert.AreEqual(1, invokedCount);

            cancelationSource.Dispose();
        }

        [Test]
        public void DeferredRejectMayNotBeCalledConcurrently_void0()
        {
            int invokedCount = 0;
            var deferred = Promise.NewDeferred();
            deferred.Promise
                .Catch(() => { Interlocked.Increment(ref invokedCount); })
                .Forget();
            int failedTryResolveCount = 0;

            var threadHelper = new ThreadHelper();
            threadHelper.ExecuteMultiActionParallel(() =>
            {
                if (!deferred.TryReject("Reject"))
                {
                    Interlocked.Increment(ref failedTryResolveCount);
                }
            });

            Assert.AreEqual(ThreadHelper.multiThreadCount - 1, failedTryResolveCount); // TryResolve should succeed once.
            Assert.Zero(invokedCount); // Callback isn't executed until manager handles it.
            Promise.Manager.HandleCompletesAndProgress();
            Assert.AreEqual(1, invokedCount);
        }

        [Test]
        public void DeferredRejectMayNotBeCalledConcurrently_void1()
        {
            int invokedCount = 0;
            var cancelationSource = CancelationSource.New();
            var deferred = Promise.NewDeferred(cancelationSource.Token);
            deferred.Promise
                .Catch(() => { Interlocked.Increment(ref invokedCount); })
                .Forget();
            int failedTryResolveCount = 0;

            var threadHelper = new ThreadHelper();
            threadHelper.ExecuteMultiActionParallel(() =>
            {
                if (!deferred.TryReject("Reject"))
                {
                    Interlocked.Increment(ref failedTryResolveCount);
                }
            });

            Assert.AreEqual(ThreadHelper.multiThreadCount - 1, failedTryResolveCount); // TryResolve should succeed once.
            Assert.Zero(invokedCount); // Callback isn't executed until manager handles it.
            Promise.Manager.HandleCompletesAndProgress();
            Assert.AreEqual(1, invokedCount);

            cancelationSource.Dispose();
        }

        [Test]
        public void DeferredRejectMayNotBeCalledConcurrently_T0()
        {
            int invokedCount = 0;
            var deferred = Promise.NewDeferred<int>();
            deferred.Promise
                .Catch(() => { Interlocked.Increment(ref invokedCount); })
                .Forget();
            int failedTryResolveCount = 0;

            var threadHelper = new ThreadHelper();
            threadHelper.ExecuteMultiActionParallel(() =>
            {
                if (!deferred.TryReject("Reject"))
                {
                    Interlocked.Increment(ref failedTryResolveCount);
                }
            });

            Assert.AreEqual(ThreadHelper.multiThreadCount - 1, failedTryResolveCount); // TryResolve should succeed once.
            Assert.Zero(invokedCount); // Callback isn't executed until manager handles it.
            Promise.Manager.HandleCompletesAndProgress();
            Assert.AreEqual(1, invokedCount);
        }

        [Test]
        public void DeferredRejectMayNotBeCalledConcurrently_T1()
        {
            int invokedCount = 0;
            var cancelationSource = CancelationSource.New();
            var deferred = Promise.NewDeferred<int>(cancelationSource.Token);
            deferred.Promise
                .Catch(() => { Interlocked.Increment(ref invokedCount); })
                .Forget();
            int failedTryResolveCount = 0;

            var threadHelper = new ThreadHelper();
            threadHelper.ExecuteMultiActionParallel(() =>
            {
                if (!deferred.TryReject("Reject"))
                {
                    Interlocked.Increment(ref failedTryResolveCount);
                }
            });

            Assert.AreEqual(ThreadHelper.multiThreadCount - 1, failedTryResolveCount); // TryResolve should succeed once.
            Assert.Zero(invokedCount); // Callback isn't executed until manager handles it.
            Promise.Manager.HandleCompletesAndProgress();
            Assert.AreEqual(1, invokedCount);

            cancelationSource.Dispose();
        }

#if PROMISE_PROGRESS
        [Test]
        public void DeferredMayReportProgressAndPromiseMaySubscribeProgressConcurrently_void0()
        {
            float expected = 0.1f;
            float progress = float.NaN;
            Promise.Deferred deferred = default(Promise.Deferred);
            Promise promise = default(Promise);

            var threadHelper = new ThreadHelper();
            threadHelper.ExecuteParallelActionsWithOffsets(false,
                // Setup
                () =>
                {
                    progress = float.NaN;
                    deferred = Promise.NewDeferred();
                    promise = deferred.Promise;
                },
                // Teardown
                () =>
                {
                    Assert.IsNaN(progress); // Progress isn't reported until manager handles it.
                    Promise.Manager.HandleCompletesAndProgress();
                    Assert.AreEqual(expected, progress, TestHelper.progressEpsilon);
                    deferred.Resolve();
                },
                // Parallel Actions
                () => deferred.ReportProgress(expected),
                () => promise.Progress(v => progress = v).Forget()
            );
        }

        [Test]
        public void DeferredMayReportProgressAndPromiseMaySubscribeProgressConcurrently_void1()
        {
            float expected = 0.1f;
            float progress = float.NaN;
            CancelationSource cancelationSource = default(CancelationSource);
            Promise.Deferred deferred = default(Promise.Deferred);
            Promise promise = default(Promise);

            var threadHelper = new ThreadHelper();
            threadHelper.ExecuteParallelActionsWithOffsets(false,
                // Setup
                () =>
                {
                    progress = float.NaN;
                    cancelationSource = CancelationSource.New();
                    deferred = Promise.NewDeferred(cancelationSource.Token);
                    promise = deferred.Promise;
                },
                // Teardown
                () =>
                {
                    Assert.IsNaN(progress); // Progress isn't reported until manager handles it.
                    Promise.Manager.HandleCompletesAndProgress();
                    Assert.AreEqual(expected, progress, TestHelper.progressEpsilon);
                    deferred.Resolve();
                    cancelationSource.Dispose();
                },
                // Parallel Actions
                () => deferred.ReportProgress(expected),
                () => promise.Progress(v => progress = v).Forget()
            );
        }

        [Test]
        public void DeferredMayReportProgressAndPromiseMaySubscribeProgressConcurrently_T0()
        {
            float expected = 0.1f;
            float progress = float.NaN;
            Promise<int>.Deferred deferred = default(Promise<int>.Deferred);
            Promise promise = default(Promise);

            var threadHelper = new ThreadHelper();
            threadHelper.ExecuteParallelActionsWithOffsets(false,
                // Setup
                () =>
                {
                    progress = float.NaN;
                    deferred = Promise.NewDeferred<int>();
                    promise = deferred.Promise;
                },
                // Teardown
                () =>
                {
                    Assert.IsNaN(progress); // Progress isn't reported until manager handles it.
                    Promise.Manager.HandleCompletesAndProgress();
                    Assert.AreEqual(expected, progress, TestHelper.progressEpsilon);
                    deferred.Resolve(1);
                },
                // Parallel Actions
                () => deferred.ReportProgress(expected),
                () => promise.Progress(v => progress = v).Forget()
            );
        }

        [Test]
        public void DeferredMayReportProgressAndPromiseMaySubscribeProgressConcurrently_T1()
        {
            float expected = 0.1f;
            float progress = float.NaN;
            CancelationSource cancelationSource = default(CancelationSource);
            Promise<int>.Deferred deferred = default(Promise<int>.Deferred);
            Promise promise = default(Promise);

            var threadHelper = new ThreadHelper();
            threadHelper.ExecuteParallelActionsWithOffsets(false,
                // Setup
                () =>
                {
                    progress = float.NaN;
                    cancelationSource = CancelationSource.New();
                    deferred = Promise.NewDeferred<int>(cancelationSource.Token);
                    promise = deferred.Promise;
                },
                // Teardown
                () =>
                {
                    Assert.IsNaN(progress); // Progress isn't reported until manager handles it.
                    Promise.Manager.HandleCompletesAndProgress();
                    Assert.AreEqual(expected, progress, TestHelper.progressEpsilon);
                    deferred.Resolve(1);
                    cancelationSource.Dispose();
                },
                // Parallel Actions
                () => deferred.ReportProgress(expected),
                () => promise.Progress(v => progress = v).Forget()
            );
        }
#endif

        [Test]
        public void DeferredMayBeBeResolvedAndPromiseAwaitedConcurrently_void0()
        {
            Promise.Deferred deferred = default(Promise.Deferred);
            Promise promise = default(Promise);

            int invokedCount = 0;

            var threadHelper = new ThreadHelper();
            threadHelper.ExecuteParallelActionsWithOffsets(false,
                // Setup
                () =>
                {
                    invokedCount = 0;
                    deferred = Promise.NewDeferred();
                    promise = deferred.Promise;
                },
                // Teardown
                () =>
                {
                    Promise.Manager.HandleCompletes();
                    Assert.AreEqual(1, invokedCount);
                },
                // Parallel Actions
                () => deferred.Resolve(),
                () => promise.Then(() => { Interlocked.Increment(ref invokedCount); }).Forget()
            );
        }

        [Test]
        public void DeferredMayBeBeResolvedAndPromiseAwaitedConcurrently_void1()
        {
            CancelationSource cancelationSource = default(CancelationSource);
            Promise.Deferred deferred = default(Promise.Deferred);
            Promise promise = default(Promise);

            int invokedCount = 0;

            var threadHelper = new ThreadHelper();
            threadHelper.ExecuteParallelActionsWithOffsets(false,
                // Setup
                () =>
                {
                    invokedCount = 0;
                    cancelationSource = CancelationSource.New();
                    deferred = Promise.NewDeferred(cancelationSource.Token);
                    promise = deferred.Promise;
                },
                // Teardown
                () =>
                {
                    cancelationSource.Dispose();
                    Promise.Manager.HandleCompletes();
                    Assert.AreEqual(1, invokedCount);
                },
                // Parallel Actions
                () => deferred.Resolve(),
                () => promise.Then(() => { Interlocked.Increment(ref invokedCount); }).Forget()
            );
        }

        [Test]
        public void DeferredMayBeBeResolvedAndPromiseAwaitedConcurrently_T0()
        {
            Promise<int>.Deferred deferred = default(Promise<int>.Deferred);
            Promise<int> promise = default(Promise<int>);

            int invokedCount = 0;

            var threadHelper = new ThreadHelper();
            threadHelper.ExecuteParallelActionsWithOffsets(false,
                // Setup
                () =>
                {
                    invokedCount = 0;
                    deferred = Promise.NewDeferred<int>();
                    promise = deferred.Promise;
                },
                // Teardown
                () =>
                {
                    Promise.Manager.HandleCompletes();
                    Assert.AreEqual(1, invokedCount);
                },
                // Parallel Actions
                () => deferred.Resolve(1),
                () => promise.Then(v => { Interlocked.Increment(ref invokedCount); }).Forget()
            );
        }

        [Test]
        public void DeferredMayBeBeResolvedAndPromiseAwaitedConcurrently_T1()
        {
            CancelationSource cancelationSource = default(CancelationSource);
            Promise<int>.Deferred deferred = default(Promise<int>.Deferred);
            Promise<int> promise = default(Promise<int>);

            int invokedCount = 0;

            var threadHelper = new ThreadHelper();
            threadHelper.ExecuteParallelActionsWithOffsets(false,
                // Setup
                () =>
                {
                    invokedCount = 0;
                    cancelationSource = CancelationSource.New();
                    deferred = Promise.NewDeferred<int>(cancelationSource.Token);
                    promise = deferred.Promise;
                },
                // Teardown
                () =>
                {
                    cancelationSource.Dispose();
                    Promise.Manager.HandleCompletes();
                    Assert.AreEqual(1, invokedCount);
                },
                // Parallel Actions
                () => deferred.Resolve(1),
                () => promise.Then(v => { Interlocked.Increment(ref invokedCount); }).Forget()
            );
        }

        [Test]
        public void DeferredMayBeBeRejectedAndPromiseAwaitedConcurrently_void0()
        {
            Promise.Deferred deferred = default(Promise.Deferred);
            Promise promise = default(Promise);

            int invokedCount = 0;

            var threadHelper = new ThreadHelper();
            threadHelper.ExecuteParallelActionsWithOffsets(false,
                // Setup
                () =>
                {
                    invokedCount = 0;
                    deferred = Promise.NewDeferred();
                    promise = deferred.Promise;
                },
                // Teardown
                () =>
                {
                    Promise.Manager.HandleCompletes();
                    Assert.AreEqual(1, invokedCount);
                },
                // Parallel Actions
                () => deferred.Reject("Reject"),
                () => promise.Catch(() => { Interlocked.Increment(ref invokedCount); }).Forget()
            );
        }

        [Test]
        public void DeferredMayBeBeRejectedAndPromiseAwaitedConcurrently_void1()
        {
            CancelationSource cancelationSource = default(CancelationSource);
            Promise.Deferred deferred = default(Promise.Deferred);
            Promise promise = default(Promise);

            int invokedCount = 0;

            var threadHelper = new ThreadHelper();
            threadHelper.ExecuteParallelActionsWithOffsets(false,
                // Setup
                () =>
                {
                    invokedCount = 0;
                    cancelationSource = CancelationSource.New();
                    deferred = Promise.NewDeferred(cancelationSource.Token);
                    promise = deferred.Promise;
                },
                // Teardown
                () =>
                {
                    cancelationSource.Dispose();
                    Promise.Manager.HandleCompletes();
                    Assert.AreEqual(1, invokedCount);
                },
                // Parallel Actions
                () => deferred.Reject("Reject"),
                () => promise.Catch(() => { Interlocked.Increment(ref invokedCount); }).Forget()
            );
        }

        [Test]
        public void DeferredMayBeBeRejectedAndPromiseAwaitedConcurrently_T0()
        {
            Promise<int>.Deferred deferred = default(Promise<int>.Deferred);
            Promise<int> promise = default(Promise<int>);

            int invokedCount = 0;

            var threadHelper = new ThreadHelper();
            threadHelper.ExecuteParallelActionsWithOffsets(false,
                // Setup
                () =>
                {
                    invokedCount = 0;
                    deferred = Promise.NewDeferred<int>();
                    promise = deferred.Promise;
                },
                // Teardown
                () =>
                {
                    Promise.Manager.HandleCompletes();
                    Assert.AreEqual(1, invokedCount);
                },
                // Parallel Actions
                () => deferred.Reject("Reject"),
                () => promise.Catch(() => { Interlocked.Increment(ref invokedCount); }).Forget()
            );
        }

        [Test]
        public void DeferredMayBeBeRejectedAndPromiseAwaitedConcurrently_T1()
        {
            CancelationSource cancelationSource = default(CancelationSource);
            Promise<int>.Deferred deferred = default(Promise<int>.Deferred);
            Promise<int> promise = default(Promise<int>);

            int invokedCount = 0;

            var threadHelper = new ThreadHelper();
            threadHelper.ExecuteParallelActionsWithOffsets(false,
                // Setup
                () =>
                {
                    invokedCount = 0;
                    cancelationSource = CancelationSource.New();
                    deferred = Promise.NewDeferred<int>(cancelationSource.Token);
                    promise = deferred.Promise;
                },
                // Teardown
                () =>
                {
                    cancelationSource.Dispose();
                    Promise.Manager.HandleCompletes();
                    Assert.AreEqual(1, invokedCount);
                },
                // Parallel Actions
                () => deferred.Reject("Reject"),
                () => promise.Catch(() => { Interlocked.Increment(ref invokedCount); }).Forget()
            );
        }

        [Test]
        public void DeferredMayBeBeCanceledAndPromiseAwaitedConcurrently_void0()
        {
            CancelationSource cancelationSource = default(CancelationSource);
            Promise.Deferred deferred = default(Promise.Deferred);
            Promise promise = default(Promise);

            int invokedCount = 0;

            var threadHelper = new ThreadHelper();
            threadHelper.ExecuteParallelActionsWithOffsets(false,
                // Setup
                () =>
                {
                    invokedCount = 0;
                    cancelationSource = CancelationSource.New();
                    deferred = Promise.NewDeferred(cancelationSource.Token);
                    promise = deferred.Promise;
                },
                // Teardown
                () =>
                {
                    cancelationSource.Dispose();
                    Promise.Manager.HandleCompletes();
                    Assert.AreEqual(1, invokedCount);
                },
                // Parallel Actions
                () => cancelationSource.Cancel("Cancel"),
                () => promise.CatchCancelation(_ => { Interlocked.Increment(ref invokedCount); }).Forget()
            );
        }

        [Test]
        public void DeferredMayBeBeCanceledAndPromiseAwaitedConcurrently_void1()
        {
            CancelationSource cancelationSource = default(CancelationSource);
            Promise.Deferred deferred = default(Promise.Deferred);
            Promise promise = default(Promise);

            int invokedCount = 0;

            var threadHelper = new ThreadHelper();
            threadHelper.ExecuteParallelActionsWithOffsets(false,
                // Setup
                () =>
                {
                    invokedCount = 0;
                    cancelationSource = CancelationSource.New();
                    deferred = Promise.NewDeferred(cancelationSource.Token);
                    promise = deferred.Promise;
                },
                // Teardown
                () =>
                {
                    cancelationSource.Dispose();
                    Promise.Manager.HandleCompletes();
                    Assert.AreEqual(1, invokedCount);
                },
                // Parallel Actions
                () => cancelationSource.Cancel(),
                () => promise.CatchCancelation(_ => { Interlocked.Increment(ref invokedCount); }).Forget()
            );
        }

        [Test]
        public void DeferredMayBeBeCanceledAndPromiseAwaitedConcurrently_T0()
        {
            CancelationSource cancelationSource = default(CancelationSource);
            Promise<int>.Deferred deferred = default(Promise<int>.Deferred);
            Promise<int> promise = default(Promise<int>);

            int invokedCount = 0;

            var threadHelper = new ThreadHelper();
            threadHelper.ExecuteParallelActionsWithOffsets(false,
                // Setup
                () =>
                {
                    invokedCount = 0;
                    cancelationSource = CancelationSource.New();
                    deferred = Promise.NewDeferred<int>(cancelationSource.Token);
                    promise = deferred.Promise;
                },
                // Teardown
                () =>
                {
                    cancelationSource.Dispose();
                    Promise.Manager.HandleCompletes();
                    Assert.AreEqual(1, invokedCount);
                },
                // Parallel Actions
                () => cancelationSource.Cancel("Cancel"),
                () => promise.CatchCancelation(_ => { Interlocked.Increment(ref invokedCount); }).Forget()
            );
        }

        [Test]
        public void DeferredMayBeBeCanceledAndPromiseAwaitedConcurrently_T1()
        {
            CancelationSource cancelationSource = default(CancelationSource);
            Promise<int>.Deferred deferred = default(Promise<int>.Deferred);
            Promise<int> promise = default(Promise<int>);

            int invokedCount = 0;

            var threadHelper = new ThreadHelper();
            threadHelper.ExecuteParallelActionsWithOffsets(false,
                // Setup
                () =>
                {
                    invokedCount = 0;
                    cancelationSource = CancelationSource.New();
                    deferred = Promise.NewDeferred<int>(cancelationSource.Token);
                    promise = deferred.Promise;
                },
                // Teardown
                () =>
                {
                    cancelationSource.Dispose();
                    Promise.Manager.HandleCompletes();
                    Assert.AreEqual(1, invokedCount);
                },
                // Parallel Actions
                () => cancelationSource.Cancel(),
                () => promise.CatchCancelation(_ => { Interlocked.Increment(ref invokedCount); }).Forget()
            );
        }
    }
}

#endif