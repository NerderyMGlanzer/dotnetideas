using System;
using System.Collections;
using System.Collections.Generic;

namespace DotNetIdeas.SupportClasses
{
    /// <summary>
    ///     Demo queryable proivdes visibility into Queryable actions by using DemoEnumerator for enumeration.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EventedEnumerable<T> : IEnumerable<T>
    {
        private readonly List<T> _list;

        public EventedEnumerable(IEnumerable<T> enumerable)
        {
            _list = new List<T>(enumerable);
        }

        public event Action OnEnumeratorCreating;
        public event Action<bool, T> OnMoveNext;
        public event Action OnDispose;

        public IEnumerator<T> GetEnumerator()
        {
            if (OnEnumeratorCreating != null)
                OnEnumeratorCreating();

            EventedEnumeratorDecorator<T> decorator = new EventedEnumeratorDecorator<T>(_list.GetEnumerator());
            if (OnMoveNext != null)
                decorator.OnMoveNext += OnMoveNext;
            if (OnDispose != null)
                decorator.OnDispose += OnDispose;

            return decorator;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            if (OnEnumeratorCreating != null)
                OnEnumeratorCreating();

            return new EventedEnumeratorDecorator<T>(_list.GetEnumerator());
        }
    }
}