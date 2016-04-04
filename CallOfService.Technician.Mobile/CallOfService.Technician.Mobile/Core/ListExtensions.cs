using System;
using System.Collections.Generic;
using System.Linq;

namespace CallOfService.Technician.Mobile.Core
{
    public static class ListExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> list, Action<T> action)
        {
            var enumerable = list as T[] ?? list.ToArray();
            var size = enumerable.Length;

            for (var i = 0; i < size; i++)
            {
                action(enumerable[i]);
            }
        }
    }
}
