﻿using NUnit.Framework;
using System;
using System.Collections;
using UnityEngine.TestTools;

namespace Proto.Promises.Tests
{
    public class APlus_2_2_TheThenMethod
    {
        // These will only pass in DEBUG mode.
        public class _2_2_1_BothOnResolveAndOnRejectedAreOptionalArgument
        {
            [Test]
            public void _2_2_1_1_IfOnFulfilledIsNullThrow()
            {
                var deferred = Promise.NewDeferred();

                Assert.AreEqual(Promise.State.Pending, deferred.State);

                var promise = deferred.Promise;

                Assert.Throws<ArgumentNullException>(() =>
                {
                    promise.Then(default(Action));
                });
                Assert.Throws<ArgumentNullException>(() => {
                    promise.Then(default(Func<int>));
                });
                Assert.Throws<ArgumentNullException>(() => {
                    promise.Then(default(Func<Promise>));
                });
                Assert.Throws<ArgumentNullException>(() => {
                    promise.Then(default(Func<Promise<int>>));
                });
                Assert.Throws<ArgumentNullException>(() => {
                    promise.ThenDefer(default(Func<Action<Promise.Deferred>>));
                });
                Assert.Throws<ArgumentNullException>(() => {
                    promise.ThenDefer<int>(default(Func<Action<Promise<int>.Deferred>>));
                });

                Assert.Throws<ArgumentNullException>(() => {
                    promise.Then(default(Action), () => { });
                });
                Assert.Throws<ArgumentNullException>(() => {
                    promise.Then(default(Action), (string failValue) => { });
                });
                Assert.Throws<ArgumentNullException>(() => {
                    promise.Then<string>(default(Action), () => { });
                });

                Assert.Throws<ArgumentNullException>(() => {
                    promise.Then(default(Func<int>), () => default(int));
                });
                Assert.Throws<ArgumentNullException>(() => {
                    promise.Then(default(Func<int>), (string failValue) => default(int));
                });
                Assert.Throws<ArgumentNullException>(() => {
                    promise.Then<int, string>(default(Func<int>), () => default(int));
                });

                Assert.Throws<ArgumentNullException>(() => {
                    promise.Then(default(Func<Promise>), () => default(Promise));
                });
                Assert.Throws<ArgumentNullException>(() => {
                    promise.Then(default(Func<Promise>), (string failValue) => default(Promise));
                });
                Assert.Throws<ArgumentNullException>(() => {
                    promise.Then<string>(default(Func<Promise>), () => default(Promise));
                });

                Assert.Throws<ArgumentNullException>(() => {
                    promise.Then(default(Func<Promise<int>>), () => default(Promise<int>));
                });
                Assert.Throws<ArgumentNullException>(() => {
                    promise.Then(default(Func<Promise<int>>), (string failValue) => default(Promise<int>));
                });
                Assert.Throws<ArgumentNullException>(() => {
                    promise.Then<int, string>(default(Func<Promise<int>>), () => default(Promise<int>));
                });

                Assert.Throws<ArgumentNullException>(() => {
                    promise.ThenDefer(default(Func<Action<Promise.Deferred>>), () => default(Action<Promise.Deferred>));
                });
                Assert.Throws<ArgumentNullException>(() => {
                    promise.ThenDefer(default(Func<Action<Promise.Deferred>>), (string failValue) => default(Action<Promise.Deferred>));
                });
                Assert.Throws<ArgumentNullException>(() => {
                    promise.ThenDefer<string>(default(Func<Action<Promise.Deferred>>), () => default(Action<Promise.Deferred>));
                });

                Assert.Throws<ArgumentNullException>(() => {
                    promise.ThenDefer<int>(default(Func<Action<Promise<int>.Deferred>>), () => default(Action<Promise<int>.Deferred>));
                });
                Assert.Throws<ArgumentNullException>(() => {
                    promise.ThenDefer<int, string>(default(Func<Action<Promise<int>.Deferred>>), failValue => default(Action<Promise<int>.Deferred>));
                });
                Assert.Throws<ArgumentNullException>(() => {
                    promise.ThenDefer<int, string>(default(Func<Action<Promise<int>.Deferred>>), () => default(Action<Promise<int>.Deferred>));
                });

                deferred.Resolve();

                var deferredInt = Promise.NewDeferred<int>();

                Assert.AreEqual(Promise.State.Pending, deferredInt.State);

                var promiseInt = deferredInt.Promise;

                Assert.Throws<ArgumentNullException>(() => {
                    promiseInt.Then(default(Action<int>));
                });
                Assert.Throws<ArgumentNullException>(() => {
                    promiseInt.Then(default(Func<int, int>));
                });
                Assert.Throws<ArgumentNullException>(() => {
                    promiseInt.Then(default(Func<int, Promise>));
                });
                Assert.Throws<ArgumentNullException>(() => {
                    promiseInt.Then(default(Func<int, Promise<int>>));
                });
                Assert.Throws<ArgumentNullException>(() => {
                    promiseInt.Then(default(Func<int, Action<Promise.Deferred>>));
                });
                Assert.Throws<ArgumentNullException>(() => {
                    promiseInt.Then(default(Func<int, Action<Promise<int>.Deferred>>));
                });

                Assert.Throws<ArgumentNullException>(() => {
                    promiseInt.Then(default(Action<int>), () => { });
                });
                Assert.Throws<ArgumentNullException>(() => {
                    promiseInt.Then(default(Action<int>), (string failValue) => { });
                });
                Assert.Throws<ArgumentNullException>(() => {
                    promiseInt.Then<string>(default(Action<int>), () => { });
                });

                Assert.Throws<ArgumentNullException>(() => {
                    promiseInt.Then(default(Func<int, int>), () => default(int));
                });
                Assert.Throws<ArgumentNullException>(() => {
                    promiseInt.Then(default(Func<int, int>), (string failValue) => default(int));
                });
                Assert.Throws<ArgumentNullException>(() => {
                    promiseInt.Then<int, string>(default(Func<int, int>), () => default(int));
                });

                Assert.Throws<ArgumentNullException>(() => {
                    promiseInt.Then(default(Func<int, Promise>), () => default(Promise));
                });
                Assert.Throws<ArgumentNullException>(() => {
                    promiseInt.Then(default(Func<int, Promise>), (string failValue) => default(Promise));
                });
                Assert.Throws<ArgumentNullException>(() => {
                    promiseInt.Then<string>(default(Func<int, Promise>), () => default(Promise));
                });

                Assert.Throws<ArgumentNullException>(() => {
                    promiseInt.Then(default(Func<int, Promise<int>>), () => default(Promise<int>));
                });
                Assert.Throws<ArgumentNullException>(() => {
                    promiseInt.Then(default(Func<int, Promise<int>>), (string failValue) => default(Promise<int>));
                });
                Assert.Throws<ArgumentNullException>(() => {
                    promiseInt.Then<int, string>(default(Func<int, Promise<int>>), () => default(Promise<int>));
                });

                Assert.Throws<ArgumentNullException>(() => {
                    promiseInt.ThenDefer(default(Func<int, Action<Promise.Deferred>>), () => default(Action<Promise.Deferred>));
                });
                Assert.Throws<ArgumentNullException>(() => {
                    promiseInt.ThenDefer(default(Func<int, Action<Promise.Deferred>>), (string failValue) => default(Action<Promise.Deferred>));
                });
                Assert.Throws<ArgumentNullException>(() => {
                    promiseInt.ThenDefer<string>(default(Func<int, Action<Promise.Deferred>>), () => default(Action<Promise.Deferred>));
                });

                Assert.Throws<ArgumentNullException>(() => {
                    promiseInt.ThenDefer<int>(default(Func<int, Action<Promise<int>.Deferred>>), () => default(Action<Promise<int>.Deferred>));
                });
                Assert.Throws<ArgumentNullException>(() => {
                    promiseInt.ThenDefer<int, string>(default(Func<int, Action<Promise<int>.Deferred>>), failValue => default(Action<Promise<int>.Deferred>));
                });
                Assert.Throws<ArgumentNullException>(() => {
                    promiseInt.ThenDefer<int, string>(default(Func<int, Action<Promise<int>.Deferred>>), () => default(Action<Promise<int>.Deferred>));
                });

                deferredInt.Resolve(0);

                // Clean up.
                Promise.Manager.HandleCompletes();
                LogAssert.NoUnexpectedReceived();
            }

            [Test]
            public void _2_2_1_2_IfOnRejectedIsNullThrow()
            {
                var deferred = Promise.NewDeferred();

                Assert.AreEqual(Promise.State.Pending, deferred.State);

                var promise = deferred.Promise;

                Assert.Throws<ArgumentNullException>(() => {
                    promise.Catch(default(Action));
                });
                Assert.Throws<ArgumentNullException>(() => {
                    promise.Catch<string>(default(Action));
                });
                Assert.Throws<ArgumentNullException>(() => {
                    promise.Catch(default(Action<string>));
                });

                Assert.Throws<ArgumentNullException>(() => {
                    promise.Catch(default(Func<Promise>));
                });
                Assert.Throws<ArgumentNullException>(() => {
                    promise.Catch<string>(default(Func<Promise>));
                });
                Assert.Throws<ArgumentNullException>(() => {
                    promise.Catch(default(Func<string, Promise>));
                });

                Assert.Throws<ArgumentNullException>(() => {
                    promise.CatchDefer(default(Func<Action<Promise.Deferred>>));
                });
                Assert.Throws<ArgumentNullException>(() => {
                    promise.CatchDefer<string>(default(Func<Action<Promise.Deferred>>));
                });
                Assert.Throws<ArgumentNullException>(() => {
                    promise.CatchDefer(default(Func<string, Action<Promise.Deferred>>));
                });

                Assert.Throws<ArgumentNullException>(() => {
                    promise.Then(() => { }, default(Action));
                });
                Assert.Throws<ArgumentNullException>(() => {
                    promise.Then<string>(() => { }, default(Action));
                });
                Assert.Throws<ArgumentNullException>(() => {
                    promise.Then(() => { }, default(Action<string>));
                });

                Assert.Throws<ArgumentNullException>(() => {
                    promise.Then(() => default(Promise), default(Func<Promise>));
                });
                Assert.Throws<ArgumentNullException>(() => {
                    promise.Then<string>(() => default(Promise), default(Func<Promise>));
                });
                Assert.Throws<ArgumentNullException>(() => {
                    promise.Then(() => default(Promise), default(Func<string, Promise>));
                });

                Assert.Throws<ArgumentNullException>(() => {
                    promise.ThenDefer(() => default(Action<Promise.Deferred>), default(Func<Action<Promise.Deferred>>));
                });
                Assert.Throws<ArgumentNullException>(() => {
                    promise.ThenDefer<string>(() => default(Action<Promise.Deferred>), default(Func<Action<Promise.Deferred>>));
                });
                Assert.Throws<ArgumentNullException>(() => {
                    promise.ThenDefer(() => default(Action<Promise.Deferred>), default(Func<string, Action<Promise.Deferred>>));
                });

                Assert.Throws<ArgumentNullException>(() => {
                    promise.Then(() => "string", default(Func<string>));
                });
                Assert.Throws<ArgumentNullException>(() => {
                    promise.Then<string, Exception>(() => "string", default(Func<string>));
                });
                Assert.Throws<ArgumentNullException>(() => {
                    promise.Then(() => "string", default(Func<Exception, string>));
                });

                Assert.Throws<ArgumentNullException>(() => {
                    promise.Then(() => default(Promise<string>), default(Func<Promise<string>>));
                });
                Assert.Throws<ArgumentNullException>(() => {
                    promise.Then<string, Exception>(() => default(Promise<string>), default(Func<Promise<string>>));
                });
                Assert.Throws<ArgumentNullException>(() => {
                    promise.Then(() => default(Promise<string>), default(Func<Exception, Promise<string>>));
                });

                Assert.Throws<ArgumentNullException>(() => {
                    promise.ThenDefer<string>(() => default(Action<Promise<string>.Deferred>), default(Func<Action<Promise<string>.Deferred>>));
                });
                Assert.Throws<ArgumentNullException>(() => {
                    promise.ThenDefer<string, Exception>(() => default(Action<Promise<string>.Deferred>), default(Func<Action<Promise<string>.Deferred>>));
                });
                Assert.Throws<ArgumentNullException>(() => {
                    promise.ThenDefer<string, Exception>(() => default(Action<Promise<string>.Deferred>), default(Func<Exception, Action<Promise<string>.Deferred>>));
                });

                deferred.Resolve();

                var deferredInt = Promise.NewDeferred<int>();

                Assert.AreEqual(Promise.State.Pending, deferredInt.State);

                var promiseInt = deferredInt.Promise;

                Assert.Throws<ArgumentNullException>(() => {
                    promiseInt.Catch(default(Func<int>));
                });
                Assert.Throws<ArgumentNullException>(() => {
                    promiseInt.Catch<string>(default(Func<int>));
                });
                Assert.Throws<ArgumentNullException>(() => {
                    promiseInt.Catch(default(Func<string, int>));
                });

                Assert.Throws<ArgumentNullException>(() => {
                    promiseInt.Catch(default(Func<Promise<int>>));
                });
                Assert.Throws<ArgumentNullException>(() => {
                    promiseInt.Catch<string>(default(Func<Promise<int>>));
                });
                Assert.Throws<ArgumentNullException>(() => {
                    promiseInt.Catch(default(Func<string, Promise<int>>));
                });

                Assert.Throws<ArgumentNullException>(() => {
                    promiseInt.CatchDefer(default(Func<Action<Promise<int>.Deferred>>));
                });
                Assert.Throws<ArgumentNullException>(() => {
                    promiseInt.CatchDefer<string>(default(Func<Action<Promise<int>.Deferred>>));
                });
                Assert.Throws<ArgumentNullException>(() => {
                    promiseInt.CatchDefer(default(Func<string, Action<Promise<int>.Deferred>>));
                });

                Assert.Throws<ArgumentNullException>(() => {
                    promiseInt.Then((int x) => { }, default(Action));
                });
                Assert.Throws<ArgumentNullException>(() => {
                    promiseInt.Then<string>((int x) => { }, default(Action));
                });
                Assert.Throws<ArgumentNullException>(() => {
                    promiseInt.Then((int x) => { }, default(Action<string>));
                });

                Assert.Throws<ArgumentNullException>(() => {
                    promiseInt.Then((int x) => default(Promise), default(Func<Promise>));
                });
                Assert.Throws<ArgumentNullException>(() => {
                    promiseInt.Then<string>((int x) => default(Promise), default(Func<Promise>));
                });
                Assert.Throws<ArgumentNullException>(() => {
                    promiseInt.Then((int x) => default(Promise), default(Func<string, Promise>));
                });

                Assert.Throws<ArgumentNullException>(() => {
                    promiseInt.ThenDefer((int x) => default(Action<Promise.Deferred>), default(Func<Action<Promise.Deferred>>));
                });
                Assert.Throws<ArgumentNullException>(() => {
                    promiseInt.ThenDefer<string>(() => default(Action<Promise.Deferred>), default(Func<Action<Promise.Deferred>>));
                });
                Assert.Throws<ArgumentNullException>(() => {
                    promiseInt.ThenDefer((int x) => default(Action<Promise.Deferred>), default(Func<string, Action<Promise.Deferred>>));
                });

                Assert.Throws<ArgumentNullException>(() => {
                    promiseInt.Then((int x) => "string", default(Func<string>));
                });
                Assert.Throws<ArgumentNullException>(() => {
                    promiseInt.Then<string, Exception>((int x) => "string", default(Func<string>));
                });
                Assert.Throws<ArgumentNullException>(() => {
                    promiseInt.Then((int x) => "string", default(Func<Exception, string>));
                });

                Assert.Throws<ArgumentNullException>(() => {
                    promiseInt.Then((int x) => default(Promise<string>), default(Func<Promise<string>>));
                });
                Assert.Throws<ArgumentNullException>(() => {
                    promiseInt.Then<string, Exception>((int x) => default(Promise<string>), default(Func<Promise<string>>));
                });
                Assert.Throws<ArgumentNullException>(() => {
                    promiseInt.Then((int x) => default(Promise<string>), default(Func<Exception, Promise<string>>));
                });

                Assert.Throws<ArgumentNullException>(() => {
                    promiseInt.ThenDefer<string>((int x) => default(Action<Promise<string>.Deferred>), default(Func<Action<Promise<string>.Deferred>>));
                });
                Assert.Throws<ArgumentNullException>(() => {
                    promiseInt.ThenDefer<string, Exception>((int x) => default(Action<Promise<string>.Deferred>), default(Func<Action<Promise<string>.Deferred>>));
                });
                Assert.Throws<ArgumentNullException>(() => {
                    promiseInt.ThenDefer<string, Exception>((int x) => default(Action<Promise<string>.Deferred>), default(Func<Exception, Action<Promise<string>.Deferred>>));
                });

                deferredInt.Resolve(0);

                // Clean up.
                Promise.Manager.HandleCompletes();
                LogAssert.NoUnexpectedReceived();
            }
        }

        public class IfOnFulfilledIsAFunction_2_2_2
        {
            [Test]
            public void _2_2_2_1_ItMustBeCalledAfterPromiseIsFulfilledWithPromisesValueAsItsFirstArgument()
            {
                var promisedValue = 100;
                var resolved = false;
                var deferredInt = Promise.NewDeferred<int>();
                Assert.AreEqual(Promise.State.Pending, deferredInt.State);
                TestHelper.AddCallbacks(deferredInt.Promise, v => {
                    Assert.AreEqual(promisedValue, v);
                    resolved = true;
                }, null);
                deferredInt.Resolve(promisedValue);
                Promise.Manager.HandleCompletes();

                Assert.True(resolved);

                // Clean up.
                Promise.Manager.HandleCompletes();
                LogAssert.NoUnexpectedReceived();
            }

            [Test]
            public void _2_2_2_2_ItMustNotBeCalledBeforePromiseIsFulfilled()
            {
                var resolved = false;
                var deferredInt = Promise.NewDeferred<int>();
                Assert.AreEqual(Promise.State.Pending, deferredInt.State);
                TestHelper.AddCallbacks(deferredInt.Promise, v => resolved = true, s => Assert.Fail("Promise was rejected when it should have been resolved."));
                var deferred = Promise.NewDeferred();
                TestHelper.AddCallbacks(deferred.Promise, () => resolved = true, s => Assert.Fail("Promise was rejected when it should have been resolved."));
                Promise.Manager.HandleCompletes();

                Assert.False(resolved);

                deferredInt.Resolve(100);
                deferred.Resolve();
                Promise.Manager.HandleCompletes();

                Assert.True(resolved);

                // Clean up.
                Promise.Manager.HandleCompletes();
                LogAssert.NoUnexpectedReceived();
            }

            [Test]
            public void _2_2_2_3_ItMustNotBeCalledMoreThanOnce()
            {
                var deferred = Promise.NewDeferred();
                var deferredInt = Promise.NewDeferred<int>();
                Assert.AreEqual(Promise.State.Pending, deferred.State);
                Assert.AreEqual(Promise.State.Pending, deferredInt.State);
                deferred.Retain();
                deferredInt.Retain();
                var resolveCount = 0;
                TestHelper.AddCallbacks(deferred.Promise, () => ++resolveCount, s => Assert.Fail("Promise was rejected when it should have been resolved."));
                TestHelper.AddCallbacks(deferredInt.Promise, x => ++resolveCount, s => Assert.Fail("Promise was rejected when it should have been resolved."));
                deferred.Resolve();
                deferredInt.Resolve(0);
                Promise.Manager.HandleCompletes();
                LogAssert.Expect(UnityEngine.LogType.Warning, "Deferred.Resolve - Deferred is not in the pending state.");
                deferred.Resolve();
                LogAssert.Expect(UnityEngine.LogType.Warning, "Deferred.Resolve - Deferred is not in the pending state.");
                deferredInt.Resolve(100);
                deferred.Release();
                deferredInt.Release();
                Promise.Manager.HandleCompletes();

                Assert.AreEqual(TestHelper.resolveVoidCallbacks + TestHelper.resolveTCallbacks, resolveCount);

                // Clean up.
                Promise.Manager.HandleCompletes();
                LogAssert.NoUnexpectedReceived();
            }
        }

        public class _2_2_3_IfOnRejectedIsAFunction
        {
            [Test]
            public void _2_2_3_1_ItMustBeCalledAfterPromiseIsRejectedWithPromisesReasonAsItsFirstArgument()
            {
                var rejectReason = "Fail value";
                var errored = false;
                Action<string> callback = (string reason) => {
                    Assert.AreEqual(rejectReason, reason);
                    errored = true;
                };
                var deferredInt = Promise.NewDeferred<int>();
                Assert.AreEqual(Promise.State.Pending, deferredInt.State);
                TestHelper.AddCallbacks(deferredInt.Promise, v => Assert.Fail("Promise was resolved when it should have been rejected."), callback, rejectReason);
                var deferred = Promise.NewDeferred();
                Assert.AreEqual(Promise.State.Pending, deferred.State);
                TestHelper.AddCallbacks(deferred.Promise, () => Assert.Fail("Promise was resolved when it should have been rejected."), callback, rejectReason);
                deferredInt.Reject(rejectReason);
                deferred.Reject(rejectReason);
                Promise.Manager.HandleCompletes();

                Assert.True(errored);

                // Clean up.
                Promise.Manager.HandleCompletes();
                LogAssert.NoUnexpectedReceived();
            }

            [Test]
            public void _2_2_3_2_ItMustNotBeCalledBeforePromiseIsRejected()
            {
                var errored = false;
                Action<string> callback = (string reason) => errored = true;
                var deferredInt = Promise.NewDeferred<int>();
                Assert.AreEqual(Promise.State.Pending, deferredInt.State);
                TestHelper.AddCallbacks(deferredInt.Promise, v => Assert.Fail("Promise was resolved when it should have been rejected."), callback);
                var deferred = Promise.NewDeferred();
                Assert.AreEqual(Promise.State.Pending, deferred.State);
                TestHelper.AddCallbacks(deferred.Promise, () => Assert.Fail("Promise was resolved when it should have been rejected."), callback);
                Promise.Manager.HandleCompletes();

                Assert.False(errored);

                deferredInt.Reject("Fail value");
                deferred.Reject("Fail value");
                Promise.Manager.HandleCompletes();

                Assert.True(errored);

                // Clean up.
                Promise.Manager.HandleCompletes();
                LogAssert.NoUnexpectedReceived();
            }

            [Test]
            public void _2_2_3_3_ItMustNotBeCalledMoreThanOnce()
            {
                var deferred = Promise.NewDeferred();
                var deferredInt = Promise.NewDeferred<int>();
                Assert.AreEqual(Promise.State.Pending, deferred.State);
                Assert.AreEqual(Promise.State.Pending, deferredInt.State);
                deferred.Retain();
                deferredInt.Retain();
                var errorCount = 0;
                TestHelper.AddCallbacks(deferred.Promise, () => Assert.Fail("Promise was resolved when it should have been rejected."), x => ++errorCount);
                TestHelper.AddCallbacks(deferredInt.Promise, v => Assert.Fail("Promise was resolved when it should have been rejected."), x => ++errorCount);
                deferred.Reject("Fail value");
                deferredInt.Reject("Fail value");
                Promise.Manager.HandleCompletes();

                LogAssert.Expect(UnityEngine.LogType.Warning, "Deferred.Reject - Deferred is not in the pending state.");
                deferred.Reject("Fail value");
                LogAssert.Expect(UnityEngine.LogType.Warning, "Deferred.Reject - Deferred is not in the pending state.");
                deferredInt.Reject("Fail value");
                deferred.Release();
                deferredInt.Release();
                Assert.Throws<AggregateException>(Promise.Manager.HandleCompletes);

                Assert.AreEqual(TestHelper.rejectVoidCallbacks + TestHelper.rejectTCallbacks, errorCount);

                // Clean up.
                Promise.Manager.HandleCompletes();
                LogAssert.NoUnexpectedReceived();
            }
        }

        // I interpret this to mean it's the last thing that is ran in the frame.
        // That is implemented during runtime by using Unity's execution order on a Monobehaviour to call Promise.Manager.HandleCompletes().
        // Since this is an editor test, I call it directly after waiting 1 frame.
        [UnityTest]
        public IEnumerator _2_2_4_OnFulfilledMustNotBeCalledUntilTheExecutionContextStackContainsOnlyPlatformCode()
        {
            var deferred = Promise.NewDeferred();
            Assert.AreEqual(Promise.State.Pending, deferred.State);
            var deferredInt = Promise.NewDeferred<int>();
            Assert.AreEqual(Promise.State.Pending, deferredInt.State);

            bool resolved = false;
            TestHelper.AddCallbacks(deferred.Promise, () => resolved = true, s => Assert.Fail("Promise was rejected when it should have been resolved."));
            TestHelper.AddCallbacks(deferredInt.Promise, v => resolved = true, s => Assert.Fail("Promise was rejected when it should have been resolved."));
            deferred.Resolve();
            deferredInt.Resolve(0);
            Assert.False(resolved);
            yield return null;
            Promise.Manager.HandleCompletes();
            Assert.True(resolved);

            // Clean up.
            Promise.Manager.HandleCompletes();
            LogAssert.NoUnexpectedReceived();
        }

        [UnityTest]
        public IEnumerator _2_2_4_OnRejectedMustNotBeCalledUntilTheExecutionContextStackContainsOnlyPlatformCode()
        {
            var deferred = Promise.NewDeferred();
            Assert.AreEqual(Promise.State.Pending, deferred.State);
            var deferredInt = Promise.NewDeferred<int>();
            Assert.AreEqual(Promise.State.Pending, deferredInt.State);

            bool errored = false;
            TestHelper.AddCallbacks(deferred.Promise, () => Assert.Fail("Promise was resolved when it should have been rejected."), s => errored = true);
            TestHelper.AddCallbacks(deferredInt.Promise, v => Assert.Fail("Promise was resolved when it should have been rejected."), s => errored = true);
            deferred.Reject("Fail value");
            deferredInt.Reject("Fail value");
            Assert.False(errored);
            yield return null;
            Promise.Manager.HandleCompletes();
            Assert.True(errored);

            // Clean up.
            Promise.Manager.HandleCompletes();
            LogAssert.NoUnexpectedReceived();
        }

        // Not relevant for C#
        // 2.2.5 onFulfilled and onRejected must be called as functions (i.e. with no this value)

        public class _2_2_6_ThenMayBeCalledMultipleTimesOnTheSamePromise
        {
            [Test]
            public void _2_2_6_1_IfWhenPromiseIsFulfilledAllRespectiveOnFulfilledCallbacksMustExecuteInTheOrderOfTheirOriginatingCallsToThen()
            {
                var deferred = Promise.NewDeferred();
                var deferredInt = Promise.NewDeferred<int>();
                Assert.AreEqual(Promise.State.Pending, deferred.State);
                Assert.AreEqual(Promise.State.Pending, deferredInt.State);

                int order = 0;
                int counter = 0;

                TestHelper.AddCallbacks(deferred.Promise, () => {
                    Assert.AreEqual(0, order);
                    if (++counter == TestHelper.resolveVoidCallbacks)
                    {
                        counter = 0;
                        ++order;
                    }
                }, s => Assert.Fail("Promise was rejected when it should have been resolved."));

                TestHelper.AddCallbacks(deferred.Promise, () => {
                    Assert.AreEqual(1, order);
                    if (++counter == TestHelper.resolveVoidCallbacks)
                    {
                        counter = 0;
                        ++order;
                    }
                }, s => Assert.Fail("Promise was rejected when it should have been resolved."));

                TestHelper.AddCallbacks(deferred.Promise, () => {
                    Assert.AreEqual(2, order);
                    if (++counter == TestHelper.resolveVoidCallbacks)
                    {
                        counter = 0;
                        ++order;
                    }
                }, s => Assert.Fail("Promise was rejected when it should have been resolved."));

                int orderT = 0;
                int counterT = 0;

                TestHelper.AddCallbacks(deferredInt.Promise, value => {
                    Assert.AreEqual(0, orderT);
                    if (++counterT == TestHelper.resolveTCallbacks)
                    {
                        counterT = 0;
                        ++orderT;
                    }
                }, s => Assert.Fail("Promise was rejected when it should have been resolved."));

                TestHelper.AddCallbacks(deferredInt.Promise, value => {
                    Assert.AreEqual(1, orderT);
                    if (++counterT == TestHelper.resolveTCallbacks)
                    {
                        counterT = 0;
                        ++orderT;
                    }
                }, s => Assert.Fail("Promise was rejected when it should have been resolved."));

                TestHelper.AddCallbacks(deferredInt.Promise, value => {
                    Assert.AreEqual(2, orderT);
                    if (++counterT == TestHelper.resolveTCallbacks)
                    {
                        counterT = 0;
                        ++orderT;
                    }
                }, s => Assert.Fail("Promise was rejected when it should have been resolved."));
                deferred.Resolve();
                deferredInt.Resolve(100);
                Promise.Manager.HandleCompletes();

                Assert.AreEqual(3, order);
                Assert.AreEqual(3, orderT);

                // Clean up.
                Promise.Manager.HandleCompletes();
                LogAssert.NoUnexpectedReceived();
            }

            [Test]
            public void _2_2_6_2_IfWhenPromiseIsRejectedAllRespectiveOnRejectedCallbacksMustExecuteInTheOrderOfTheirOriginatingCallsToThenOrCatch()
            {
                var deferred = Promise.NewDeferred();
                var deferredInt = Promise.NewDeferred<int>();
                Assert.AreEqual(Promise.State.Pending, deferred.State);
                Assert.AreEqual(Promise.State.Pending, deferredInt.State);

                int order = 0;
                int counter = 0;

                TestHelper.AddCallbacks(deferred.Promise, () => Assert.Fail("Promise was resolved when it should have been rejected."), failValue => {
                    Assert.AreEqual(0, order);
                    if (++counter == TestHelper.rejectVoidCallbacks)
                    {
                        counter = 0;
                        ++order;
                    }
                });

                TestHelper.AddCallbacks(deferred.Promise, () => Assert.Fail("Promise was resolved when it should have been rejected."), failValue => {
                    Assert.AreEqual(1, order);
                    if (++counter == TestHelper.rejectVoidCallbacks)
                    {
                        counter = 0;
                        ++order;
                    }
                });

                TestHelper.AddCallbacks(deferred.Promise, () => Assert.Fail("Promise was resolved when it should have been rejected."), failValue => {
                    Assert.AreEqual(2, order);
                    if (++counter == TestHelper.rejectVoidCallbacks)
                    {
                        counter = 0;
                        ++order;
                    }
                });

                int orderT = 0;
                int counterT = 0;

                TestHelper.AddCallbacks(deferredInt.Promise, v => Assert.Fail("Promise was resolved when it should have been rejected."), failValue => {
                    Assert.AreEqual(0, orderT);
                    if (++counterT == TestHelper.rejectTCallbacks)
                    {
                        counterT = 0;
                        ++orderT;
                    }
                });

                TestHelper.AddCallbacks(deferredInt.Promise, v => Assert.Fail("Promise was resolved when it should have been rejected."), failValue => {
                    Assert.AreEqual(1, orderT);
                    if (++counterT == TestHelper.rejectTCallbacks)
                    {
                        counterT = 0;
                        ++orderT;
                    }
                });

                TestHelper.AddCallbacks(deferredInt.Promise, v => Assert.Fail("Promise was resolved when it should have been rejected."), failValue => {
                    Assert.AreEqual(2, orderT);
                    if (++counterT == TestHelper.rejectTCallbacks)
                    {
                        counterT = 0;
                        ++orderT;
                    }
                });
                deferred.Reject("Fail value");
                deferredInt.Reject("Fail value");
                Promise.Manager.HandleCompletes();

                Assert.AreEqual(3, order);
                Assert.AreEqual(3, orderT);

                // Clean up.
                Promise.Manager.HandleCompletes();
                LogAssert.NoUnexpectedReceived();
            }
        }

        public class ThenMustReturnAPromise_2_2_7
        {
            // 2.2.7.1 Promise Resolution Procedure in 2.3

            [Test]
            public void _2_2_7_2_IfOnFulfilledThrowsAnExceptionEPromise2MustBeRejectedWithEAsTheReason()
            {
                var deferred = Promise.NewDeferred();
                var deferredInt = Promise.NewDeferred<int>();
                Assert.AreEqual(Promise.State.Pending, deferred.State);
                Assert.AreEqual(Promise.State.Pending, deferredInt.State);

                Exception expected = new Exception("Fail value");

                TestHelper.AddCallbacks(deferred.Promise, () => { throw expected; }, s => Assert.Fail("Promise was rejected when it should have been resolved."), onError: e => Assert.AreEqual(expected, e));
                TestHelper.AddCallbacks(deferredInt.Promise, v => { throw expected; }, s => Assert.Fail("Promise was rejected when it should have been resolved."), onError: e => Assert.AreEqual(expected, e));

                deferred.Resolve();
                deferredInt.Resolve(100);
                Promise.Manager.HandleCompletes();

                // Clean up.
                Promise.Manager.HandleCompletes();
                LogAssert.NoUnexpectedReceived();
            }

            [Test]
            public void _2_2_7_2_IfOnRejectedThrowsAnExceptionEPromise2MustBeRejectedWithEAsTheReason()
            {
                var deferred = Promise.NewDeferred();
                var deferredInt = Promise.NewDeferred<int>();
                Assert.AreEqual(Promise.State.Pending, deferred.State);
                Assert.AreEqual(Promise.State.Pending, deferredInt.State);

                Exception expected = new Exception("Fail value");

                TestHelper.AddCallbacks(deferred.Promise, () => Assert.Fail("Promise was resolved when it should have been rejected."), v => { throw expected; }, onError: e => Assert.AreEqual(expected, e));
                TestHelper.AddCallbacks(deferredInt.Promise, v => Assert.Fail("Promise was resolved when it should have been rejected."), v => { throw expected; }, onError: e => Assert.AreEqual(expected, e));

                deferred.Reject("Fail value");
                deferredInt.Reject("Fail value");
                Promise.Manager.HandleCompletes();

                // Clean up.
                Promise.Manager.HandleCompletes();
                LogAssert.NoUnexpectedReceived();
            }

            [Test]
            public void _2_2_7_3_IfOnFulfilledIsNotAFunctionAndPromise1IsFulfilledPromise2MustBeFulfilledWithTheSameValueAsPromise1()
            {
                var deferred = Promise.NewDeferred();
                var deferredInt = Promise.NewDeferred<int>();
                Assert.AreEqual(Promise.State.Pending, deferred.State);
                Assert.AreEqual(Promise.State.Pending, deferredInt.State);

                int expected = 100;
                int counter = 0;

                var promise1Void = deferred.Promise.Catch(() => { Assert.Fail("Promise was rejected when it should have been resolved."); return; });
                var promise1Int = deferredInt.Promise.Catch(() => { Assert.Fail("Promise was rejected when it should have been resolved."); return 50; });

                TestHelper.AddCallbacks(promise1Void, () => ++counter, s => Assert.Fail("Promise was rejected when it should have been resolved."));
                TestHelper.AddCallbacks(promise1Int, v => Assert.AreEqual(expected, v), s => Assert.Fail("Promise was rejected when it should have been resolved."));

                deferred.Resolve();
                deferredInt.Resolve(expected);
                Promise.Manager.HandleCompletes();

                Assert.AreEqual(counter, TestHelper.resolveVoidCallbacks);

                // Clean up.
                Promise.Manager.HandleCompletes();
                LogAssert.NoUnexpectedReceived();
            }

            [Test]
            public void _2_2_7_4_IfOnRejectedIsNotAFunctionAndPromise1IsRejectedPromise2MustBeRejectedWithTheSameReasonAsPromise1()
            {
                var deferred = Promise.NewDeferred();
                var deferredInt = Promise.NewDeferred<int>();
                Assert.AreEqual(Promise.State.Pending, deferred.State);
                Assert.AreEqual(Promise.State.Pending, deferredInt.State);

                string expected = "Fail value";

                var promise1Void = deferred.Promise.Then(() => { Assert.Fail("Promise was rejected when it should have been resolved."); return; });
                var promise1Int = deferredInt.Promise.Then(() => { Assert.Fail("Promise was rejected when it should have been resolved."); return 50; });

                TestHelper.AddCallbacks(promise1Void, () => Assert.Fail("Promise was rejected when it should have been resolved."), e => Assert.AreEqual(expected, e), expected);
                TestHelper.AddCallbacks(promise1Int, v => Assert.Fail("Promise was rejected when it should have been resolved."), e => Assert.AreEqual(expected, e), expected);

                deferred.Reject(expected);
                deferredInt.Reject(expected);
                Promise.Manager.HandleCompletes();

                // Clean up.
                Promise.Manager.HandleCompletes();
                LogAssert.NoUnexpectedReceived();
            }
        }
    }
}