using System;
using System.Collections.Generic;
using System.Linq;
using DotNetIdeas.SupportClasses;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DotNetIdeas
{
    /// <summary>
    ///     Demonstrates usage of the yield C# keyword
    /// </summary>
    [TestClass]
    public class YieldTests
    {
        #region Sequential yield

        /// <summary>
        ///     Will generate integers in sequence.
        /// </summary>
        /// <returns></returns>
        private IEnumerable<uint> YieldInts(uint max = 10)
        {
            uint i = 0;
            while (i < max)
                yield return i++;
        }

        [TestMethod]
        public void t1_yield_behaves_like_returning_IEnumerable()
        {
            YieldInts().ForEach(Console.WriteLine);
        }

        #endregion

        #region Alternating yield (multiple yield points)

        private IEnumerable<uint> YieldAlternatingInts(uint max = 10)
        {
            for (int i = 0; i < max; i++)
            {
                yield return 0;
                yield return 1;
            }
        }

        [TestMethod]
        public void t2_can_use_multiple_yield_points()
        {
            YieldAlternatingInts().ForEach(Console.WriteLine);
        }

        #endregion

        #region yield to avoid expensive computation

        private IEnumerable<string> YieldExpensiveResources()
        {
            string[] key = new[] {"One", "Two", "Three", "Four", "Five"};

            foreach (string s in key)
            {
                Console.WriteLine("Computing resource for key {0}", s);
                yield return string.Format("Expensive resource {0}", s);
            }
        }

        [TestMethod]
        public void t3_yield_may_be_used_to_avoid_work_by_using_take()
        {
            Console.WriteLine("Computation of Four and Five are avoided");
            Console.WriteLine(YieldExpensiveResources().First(s => s.Contains("Three")));
        }

        #endregion
    }
}