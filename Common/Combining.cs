/* 
    This file is part of crANNy. Copyright (C) 2017 Christian Rauch.
    Distributed under terms of the GPL3 license.
*/

namespace CRAI.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Diagnostics;
    using System.Collections.ObjectModel;
    using System.Collections;
    using System.Security.Cryptography;

    public static class Combining
    {
        [DebuggerStepThrough]
        public static List<T> ToRandomList<T>(this IEnumerable<T> setOriginal)
        {
            var set = setOriginal.ToList();

            var random = new Random();

            int count = set.Count;

            while (count > 1)
            {
                count--;
                var indexRandom = random.Next(count + 1);
                var value = set[indexRandom];
                set[indexRandom] = set[count];
                set[count] = value;
            }

            return set;
        }

        [DebuggerStepThrough]
        public static IEnumerable<T> Distinct<T, U>(this IEnumerable<T> items, Func<T, U> selector)
        {
            var itemsDistinct = new Dictionary<U, T>();

            foreach (T item in items)
            {
                var key = selector(item);
                itemsDistinct[key] = item;
            }

            return itemsDistinct.Values;
        }

        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> items)
        {
            return new ObservableCollection<T>(items);
        }

        public static ObservableCollection<T> AddRange<T>(
            this ObservableCollection<T> observableCollection, IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                observableCollection.Add(item);
            }

            return observableCollection;
        }

        private static Guid CombineTwoGuids(Guid guid1, Guid guid2)
        {
            const int byteCount = 16;
            byte[] destByte = new byte[byteCount];
            byte[] guid1Byte = guid1.ToByteArray();
            byte[] guid2Byte = guid2.ToByteArray();

            for (int i = 0; i < byteCount; i++)
            {
                destByte[i] = (byte)(guid1Byte[i] ^ guid2Byte[i]);
            }
            return new Guid(destByte);
        }

        public static Guid Combine(this Guid guid1, params Guid[] guids)
        {
            return guids.Aggregate(guid1, CombineTwoGuids);
        }

        [DebuggerStepThrough]
        public static bool IsEmpty(this Guid guid)
        {
            return guid == Guid.Empty;
        }

        [DebuggerStepThrough]
        public static bool IsIn<T>(this T value, IEnumerable<T> set)
        {
            return set.Contains(value);
        }

        [DebuggerStepThrough]
        public static bool IsIn<T>(this T value, params T[] array)
        {
            return array.Contains(value);
        }

        [DebuggerStepThrough]
        public static bool IsNotIn<T>(this T value, params T[] array)
        {
            return !IsIn(value, array);
        }

        [DebuggerStepThrough]
        public static bool IsNotIn<T>(this T value, IEnumerable<T> set)
        {
            return !IsIn(value, set);
        }

        [DebuggerStepThrough]
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> set)
        {
            return set == null || !set.Any();
        }

        [DebuggerStepThrough]
        public static T ElementAtOrLastOrDefault<T>(this IEnumerable<T> set, int position)
        {
            var count = set.Count();

            if(count == 0)
            {
                return default(T);
            }
            else if(position >= count)
            {
                return set.Last();
            }

            return set.ElementAtOrDefault(position);
        }

        [DebuggerStepThrough]
        public static IEnumerable<T> TrimEnd<T>(this IEnumerable<T> set)
        {
            var indexLast = set.Count() - 1;

            for (; indexLast > 0; indexLast--)
            {
                if (!Object.Equals(
                    set.ElementAt(indexLast),
                    set.ElementAt(indexLast - 1)))
                {
                    break;
                }
            }

            return set.Take(indexLast + 1);
        }

        [DebuggerStepThrough]
        public static TValue TryGetValueFallback<TKey, TValueBase, TValue>(
            this IDictionary<TKey, TValueBase> dictionary, TKey key, TValue fallback)
            where TValue : TValueBase
        {
            TValueBase value;

            if (!dictionary.TryGetValue(key, out value))
            {
                return fallback;
            }

            return (TValue)value;
        }
    }
}
