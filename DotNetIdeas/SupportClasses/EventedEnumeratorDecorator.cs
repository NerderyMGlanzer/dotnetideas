using System;
using System.Collections;
using System.Collections.Generic;

namespace DotNetIdeas.SupportClasses
{
    public class EventedEnumeratorDecorator<T> : IEnumerator<T>
    {
        private readonly IEnumerator<T> _enumerator;

        public event Action<bool, T> OnMoveNext;
        public event Action OnDispose;

        public EventedEnumeratorDecorator(IEnumerator<T> enumerator)
        {
            _enumerator = enumerator;
        }

        public void Dispose()
        {
            _enumerator.Dispose();
            if (OnDispose != null)
                OnDispose();
        }

        public bool MoveNext()
        {
            bool next = _enumerator.MoveNext();
            if (OnMoveNext != null)
                OnMoveNext(next, Current);
            return next;
        }

        public void Reset()
        {
            _enumerator.Reset();
        }

        public T Current { get { return _enumerator.Current; } }

        object IEnumerator.Current
        {
            get { return Current; }
        }
    }
}