using System;
using System.Collections;
using System.Collections.Generic;

namespace DotNetIdeas.SupportClasses
{
    /// <summary>
    ///     Demo enumerator provides visibility into enumeration actions
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DemoEnumerator<T> : IEnumerator<T>
    {
        private int _index = -1;
        private List<T> _list;

        public DemoEnumerator(IEnumerable<T> items)
        {
            _list = new List<T>(items);
        }

        public void Dispose()
        {
            _list = null;
        }

        public bool MoveNext()
        {
            if (_list.Count <= _index)
            {
                Console.WriteLine("No more items in enumeration");
                return false;
            }

            _index++;
            Current = _list[_index];
            Console.WriteLine("Moving to item {0} of {1}", _index, _list.Count);
            return true;
        }

        public void Reset()
        {
            _index = -1;
            Current = default(T);
        }

        public T Current { get; private set; }

        object IEnumerator.Current
        {
            get { return Current; }
        }
    }
}