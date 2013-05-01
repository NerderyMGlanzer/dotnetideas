using System;
using System.Collections.Generic;
using System.Linq;
using DotNetIdeas.SupportClasses;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DotNetIdeas
{
    /// <summary>
    /// Demonstration of properties and behavior of IQueryable
    /// TODO: Provide LINQ to SQL or Entity Framework examples
    /// </summary>
    [TestClass]
    public class QueryableTests
    {
        [TestMethod]
        public void t1_Queryables_are_part_of_Linq()
        {
            Assert.IsTrue(typeof (IQueryable).FullName.Contains("System.Linq"));
        }

        [TestMethod]
        public void t2_Queryables_are_also_Enumerable()
        {
            int[] ints = new[] {1, 2, 3};

            // An array is not IQueryable
            Assert.IsFalse(ints is IQueryable<int>);

            // Can convert any IEnumerable<T> using the AsQueryable() extension method
            IQueryable<int> query = ints.AsQueryable();

            Assert.IsTrue(query is IQueryable<int>);

            using (IEnumerator<int> enumerator = query.GetEnumerator())
                while (enumerator.MoveNext())
                    Console.WriteLine(enumerator.Current);
        }

        [TestMethod]
        public void t3_Enumerables_do_not_invoke_enumerator_when_building_query()
        {
            bool enumerableCreated = false;
            EventedEnumerable<int> enumerable = new EventedEnumerable<int>(new[] {1, 2, 3});

            // Have evented enumerable notify when it's creating an enumerator
            enumerable.OnEnumeratorCreating += () =>
                {
                    enumerableCreated = true;
                    Console.WriteLine("Enumerator created");
                };

            IEnumerable<int> ints = enumerable.Where(i => i < 3);
            ints = ints.Where(i => i > 1);

            Console.WriteLine("Enumerator has not been created, no read of data has happened yet");
            Assert.IsFalse(enumerableCreated);

            Console.WriteLine("Invoke enumerator by requesting data from enumerable");
            ints.ToList().ForEach(Console.WriteLine);

            Console.WriteLine("Enumerator has been created");
            Assert.IsTrue(enumerableCreated);
        }
    }
}