﻿#if !PROTO_PROMISE_CANCEL_DISABLE
#define PROMISE_CANCEL
#endif
#if !PROTO_PROMISE_PROGRESS_DISABLE
#define PROMISE_PROGRESS
#endif

using System;
using System.Linq;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Proto.Promises.Tests
{
    public class MergeTests
    {
        [Test]
        public void MergePromiseIsResolvedWhenAllPromisesAreResolved()
        {
            var deferred1 = Promise.NewDeferred<int>();
            var deferred2 = Promise.NewDeferred<string>();

            string success = "Success";
            bool resolved = false;

            Promise.Merge(deferred1.Promise, deferred2.Promise)
                .Then(values =>
                {
                    resolved = true;

                    Assert.AreEqual(1, values.Item1);
                    Assert.AreEqual(success, values.Item2);
                });

            deferred1.Resolve(1);
            deferred2.Resolve(success);

            Promise.Manager.HandleCompletes();

            Assert.AreEqual(true, resolved);

            // Clean up.
            GC.Collect();
            Promise.Manager.HandleCompletes();
            LogAssert.NoUnexpectedReceived();
        }

        [Test]
        public void MergePromiseIsResolvedWhenAllPromisesAreAlreadyResolved()
        {
            var promise1 = Promise.Resolved(1);
            string success = "Success";
            var promise2 = Promise.Resolved(success);

            promise1.Retain();
            promise2.Retain();
            Promise.Manager.HandleCompletes();

            bool resolved = false;

            Promise.Merge(promise1, promise2)
                .Then(values =>
                {
                    resolved = true;

                    Assert.AreEqual(1, values.Item1);
                    Assert.AreEqual(success, values.Item2);
                });

            promise1.Release();
            promise2.Release();
            Promise.Manager.HandleCompletes();

            Assert.AreEqual(true, resolved);

            // Clean up.
            GC.Collect();
            Promise.Manager.HandleCompletes();
            LogAssert.NoUnexpectedReceived();
        }

        [Test]
        public void MergePromiseIsRejectedWhenFirstPromiseIsRejected()
        {
            var deferred1 = Promise.NewDeferred<int>();
            var deferred2 = Promise.NewDeferred<string>();

            bool rejected = false;

            Promise.Merge(deferred1.Promise, deferred2.Promise)
                .Then(v => Assert.Fail("Promise was resolved when it should have been rejected."))
                .Catch<string>(e => { rejected = true; });

            deferred1.Reject("Error!");
            deferred2.Resolve("Success");

            Promise.Manager.HandleCompletes();

            Assert.AreEqual(true, rejected);

            // Clean up.
            GC.Collect();
            Promise.Manager.HandleCompletes();
            LogAssert.NoUnexpectedReceived();
        }

        [Test]
        public void MergePromiseIsRejectedWhenSecondPromiseIsRejected()
        {
            var deferred1 = Promise.NewDeferred<int>();
            var deferred2 = Promise.NewDeferred<string>();

            bool rejected = false;

            Promise.Merge(deferred1.Promise, deferred2.Promise)
                .Then(v => Assert.Fail("Promise was resolved when it should have been rejected."))
                .Catch<string>(e => { rejected = true; });

            deferred1.Resolve(2);
            deferred2.Reject("Error!");

            Promise.Manager.HandleCompletes();

            Assert.AreEqual(true, rejected);

            // Clean up.
            GC.Collect();
            Promise.Manager.HandleCompletes();
            LogAssert.NoUnexpectedReceived();
        }

        [Test]
        public void MergePromiseIsRejectedWhenBothPromisesAreRejected()
        {
            var deferred1 = Promise.NewDeferred<int>();
            var deferred2 = Promise.NewDeferred<string>();

            bool rejected = false;

            Promise.Merge(deferred1.Promise, deferred2.Promise)
                .Then(v => Assert.Fail("Promise was resolved when it should have been rejected."))
                .Catch<string>(e => { rejected = true; });

            deferred1.Reject("Error!");
            deferred2.Reject("Error!");

            // Only 1 rejection is caught, so expect an unhandled throw.
            Assert.Throws<AggregateException>(Promise.Manager.HandleCompletes);

            Assert.AreEqual(true, rejected);

            // Clean up.
            GC.Collect();
            Promise.Manager.HandleCompletes();
            LogAssert.NoUnexpectedReceived();
        }

        [Test]
        public void MergePromiseIsRejectedWhenAnyPromiseIsAlreadyRejected()
        {
            bool rejected = false;
            string rejection = "Error!";

            var deferred = Promise.NewDeferred<int>();
            var promise = Promise.Rejected<int, string>(rejection);

            promise.Retain();
            Promise.Manager.HandleCompletes();

            Promise.Merge(deferred.Promise, promise)
                .Then(v => Assert.Fail("Promise was resolved when it should have been rejected."))
                .Catch<string>(ex =>
                {
                    Assert.AreEqual(rejection, ex);
                    rejected = true;
                });

            deferred.Resolve(0);
            promise.Release();

            Promise.Manager.HandleCompletes();

            Assert.AreEqual(true, rejected);

            // Clean up.
            GC.Collect();
            Promise.Manager.HandleCompletes();
            LogAssert.NoUnexpectedReceived();
        }

#if PROMISE_CANCEL
        [Test]
        public void MergePromiseIsCanceledWhenFirstPromiseIsCanceled()
        {
            var deferred1 = Promise.NewDeferred<int>();
            var deferred2 = Promise.NewDeferred<string>();

            bool canceled = false;

            Promise.Merge(deferred1.Promise, deferred2.Promise)
                .Then(v => Assert.Fail("Promise was resolved when it should have been rejected."))
                .CatchCancelation<string>(e => canceled = true);

            deferred1.Cancel("Cancel!");
            deferred2.Resolve("Success");

            Promise.Manager.HandleCompletes();

            Assert.AreEqual(true, canceled);

            // Clean up.
            GC.Collect();
            Promise.Manager.HandleCompletes();
            LogAssert.NoUnexpectedReceived();
        }

        [Test]
        public void MergePromiseIsCanceledWhenSecondPromiseIsCanceled()
        {
            var deferred1 = Promise.NewDeferred<int>();
            var deferred2 = Promise.NewDeferred<string>();

            bool canceled = false;

            Promise.Merge(deferred1.Promise, deferred2.Promise)
                .Then(v => Assert.Fail("Promise was resolved when it should have been rejected."))
                .CatchCancelation<string>(e => canceled = true);

            deferred1.Resolve(2);
            deferred2.Cancel("Cancel!");

            Promise.Manager.HandleCompletes();

            Assert.AreEqual(true, canceled);

            // Clean up.
            GC.Collect();
            Promise.Manager.HandleCompletes();
            LogAssert.NoUnexpectedReceived();
        }

        [Test]
        public void MergePromiseIsCanceledWhenBothPromisesAreCanceled()
        {
            var deferred1 = Promise.NewDeferred<int>();
            var deferred2 = Promise.NewDeferred<string>();

            bool canceled = false;

            Promise.Merge(deferred1.Promise, deferred2.Promise)
                .Then(v => Assert.Fail("Promise was resolved when it should have been rejected."))
                .CatchCancelation<string>(e => canceled = true);

            deferred1.Cancel("Cancel!");
            deferred2.Cancel("Cancel!");

            Promise.Manager.HandleCompletes();

            Assert.AreEqual(true, canceled);

            // Clean up.
            GC.Collect();
            Promise.Manager.HandleCompletes();
            LogAssert.NoUnexpectedReceived();
        }

        [Test]
        public void MergePromiseIsCancelededWhenAnyPromiseIsAlreadyCanceled()
        {
            bool canceled = false;
            string cancelation = "Cancel!";

            var deferred = Promise.NewDeferred<int>();
            var promise = Promise.Canceled<int, string>(cancelation);

            promise.Retain();
            Promise.Manager.HandleCompletes();

            Promise.All(deferred.Promise, promise)
                .Then(v => Assert.Fail("Promise was resolved when it should have been rejected."))
                .CatchCancelation<string>(ex =>
                {
                    Assert.AreEqual(cancelation, ex);
                    canceled = true;
                });

            deferred.Resolve(0);
            promise.Release();

            Promise.Manager.HandleCompletes();

            Assert.AreEqual(true, canceled);

            // Clean up.
            GC.Collect();
            Promise.Manager.HandleCompletes();
            LogAssert.NoUnexpectedReceived();
        }
#endif

#if PROMISE_PROGRESS
        [Test]
        public void MergeProgressIsNormalized()
        {
            var deferred1 = Promise.NewDeferred<int>();
            var deferred2 = Promise.NewDeferred<string>();
            var deferred3 = Promise.NewDeferred<float>();
            var deferred4 = Promise.NewDeferred<bool>();

            float progress = float.NaN;

            Promise.Merge(deferred1.Promise, deferred2.Promise, deferred3.Promise, deferred4.Promise)
                .Progress(p => progress = p);

            Promise.Manager.HandleCompletesAndProgress();
            Assert.AreEqual(0f, progress, 0f);

            deferred1.ReportProgress(0.5f);
            Promise.Manager.HandleCompletesAndProgress();
            Assert.AreEqual(1f / 8f, progress, TestHelper.progressEpsilon);

            deferred1.Resolve(1);
            Promise.Manager.HandleCompletesAndProgress();
            Assert.AreEqual(2f / 8f, progress, TestHelper.progressEpsilon);

            deferred2.ReportProgress(0.5f);
            Promise.Manager.HandleCompletesAndProgress();
            Assert.AreEqual(3f / 8f, progress, TestHelper.progressEpsilon);

            deferred2.Resolve("Success");
            Promise.Manager.HandleCompletesAndProgress();
            Assert.AreEqual(4f / 8f, progress, TestHelper.progressEpsilon);

            deferred3.ReportProgress(0.5f);
            Promise.Manager.HandleCompletesAndProgress();
            Assert.AreEqual(5f / 8f, progress, TestHelper.progressEpsilon);

            deferred3.Resolve(2f);
            Promise.Manager.HandleCompletesAndProgress();
            Assert.AreEqual(6f / 8f, progress, TestHelper.progressEpsilon);

            deferred4.ReportProgress(0.5f);
            Promise.Manager.HandleCompletesAndProgress();
            Assert.AreEqual(7f / 8f, progress, TestHelper.progressEpsilon);

            deferred4.Resolve(true);
            Promise.Manager.HandleCompletesAndProgress();
            Assert.AreEqual(8f / 8f, progress, TestHelper.progressEpsilon);

            // Clean up.
            GC.Collect();
            Promise.Manager.HandleCompletesAndProgress();
            LogAssert.NoUnexpectedReceived();
        }
#endif
    }
}