using System;
using System.Collections.Generic;
using System.Linq;

namespace DotNetIdeas.SupportClasses
{
    public static class EnumerableExtensions
    {
         public static IEnumerable<T> ForEach<T>(this IEnumerable<T> instance, Action<T> action)
         {
             instance = instance.ToList();
             foreach (T t in instance)
                 action(t);
             return instance;
         }
    }
}