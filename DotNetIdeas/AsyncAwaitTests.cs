using System;
using System.Threading;
using System.Threading.Tasks;
using DotNetIdeas.SupportClasses;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DotNetIdeas
{
    [TestClass]
    public class AsyncAwaitTests
    {
        private int _counter;
        private const int _millisecondsDelay = 100;

        private int GetExpensiveResource()
        {
            Thread.Sleep(_millisecondsDelay);
            return _counter++;
        }

        private async Task<int> GetExpensiveResourceAsync()
        {
            await Task.Delay(_millisecondsDelay);
            return _counter++;
        }

        [TestInitialize]
        public void TestInitialize()
        {
            _counter = 0;
        }

        [TestMethod]
        public void t1_non_awaited_tasks_take_sequential_time()
        {
            long elapsedMs = 0;
            using (new DisposableStopwatch(ms => elapsedMs = ms))
                for (int i = 0; i < 5; i++)
                    Console.WriteLine(GetExpensiveResource());

            const long expectedMinMs = 450;
            Assert.IsTrue(elapsedMs > expectedMinMs);

            Console.WriteLine("Elapsed milliseconds: {0}", elapsedMs);
        }

        [TestMethod]
        public async Task t2_async_await_does_not_equal_parallelism()
        {
            long elapsedMs = 0;
            using (new DisposableStopwatch(ms => elapsedMs = ms))
                for (int i = 0; i < 5; i++)
                    Console.WriteLine(await GetExpensiveResourceAsync());

            const long expectedMinMs = 450;
            Assert.IsTrue(elapsedMs > expectedMinMs);

            Console.WriteLine("Elapsed milliseconds: {0}", elapsedMs);
        }

        #region Can use async/await in conjunction with events

        public class Workflow
        {
            public event Action Approved;
            public event Action<string> LegalReviewed;
            public event Action<string> ManagerReviewed;

            public void Approve()
            {
                Console.WriteLine("Firing Approved event");
                if (Approved != null)
                    Approved();
            }

            public void LegalReview(string comments)
            {
                Console.WriteLine("Firing LegalReviewed event");
                if (LegalReviewed != null)
                    LegalReviewed(comments);
            }

            public void ManagerReview(string comments)
            {
                Console.WriteLine("Firing ManagerReviewed event");
                if (ManagerReviewed != null)
                    ManagerReviewed(comments);
            }

            public async Task WhenApproved()
            {
                TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();
                Action lambda = () => tcs.TrySetResult(null);

                try
                {
                    Approved += lambda;
                    await tcs.Task;
                }
                finally
                {
                    Approved -= lambda;
                }
            }

            public async Task<string> WhenManagerReviewed()
            {
                TaskCompletionSource<string> tcs = new TaskCompletionSource<string>();
                Action<string> lambda = comments => tcs.TrySetResult(comments);

                try
                {
                    ManagerReviewed += lambda;
                    return await tcs.Task;
                }
                finally
                {
                    ManagerReviewed -= lambda;
                }
            }

            public async Task<string> WhenLegalReviewed()
            {
                TaskCompletionSource<string> tcs = new TaskCompletionSource<string>();
                Action<string> lambda = comments => tcs.TrySetResult(comments);

                try
                {
                    LegalReviewed += lambda;
                    return await tcs.Task;
                }
                finally
                {
                    LegalReviewed -= lambda;
                }
            }
        }

        /// <summary>
        /// Inspired by async/await video outlining a pattern for WPF use case
        /// http://channel9.msdn.com/Series/Three-Essential-Tips-for-Async/Lucian03-TipsForAsyncThreadsAndDatabinding
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task t3_async_can_await_events()
        {
            Workflow w = new Workflow();

            // Simulate people working on workflow (on separate thread)
            Task.Run(() =>
                {
                    Thread.Sleep(100);
                    w.ManagerReview("Looks great! -manager");
                    w.LegalReview("We're covered. -legal");
                    w.Approve();
                });

            // Business logic separated from workflow capabilities
            Console.WriteLine("Awaiting reviews");
            await Task.WhenAll(
                w.WhenLegalReviewed(),
                w.WhenManagerReviewed());
            Console.WriteLine("Have all reviews");

            Console.WriteLine("Awaiting approval");
            await w.WhenApproved();
            Console.WriteLine("Have final approval, workflow finished");
        }

        #endregion
    }
}