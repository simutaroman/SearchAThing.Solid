#region SearchAThing.Core, Copyright(C) 2015-2016 Lorenzo Delana, License under MIT
/*
* The MIT License(MIT)
* Copyright(c) 2016 Lorenzo Delana, https://searchathing.com
*
* Permission is hereby granted, free of charge, to any person obtaining a
* copy of this software and associated documentation files (the "Software"),
* to deal in the Software without restriction, including without limitation
* the rights to use, copy, modify, merge, publish, distribute, sublicense,
* and/or sell copies of the Software, and to permit persons to whom the
* Software is furnished to do so, subject to the following conditions:
*
* The above copyright notice and this permission notice shall be included in
* all copies or substantial portions of the Software.
*
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
* FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
* DEALINGS IN THE SOFTWARE.
*/
#endregion

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using static System.Math;
using System.Linq;

namespace SearchAThing
{

    public static partial class Extensions
    {

        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> set)
        {
            return new ObservableCollection<T>(set);
        }

        /// <summary>
        /// overloaded method that allow you to process something while populate obc
        /// </summary>        
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> set, Func<T, T> doSomeJob)
        {
            return new ObservableCollection<T>(set.Select(x => doSomeJob(x)));
        }

        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> set)
        {
            return new HashSet<T>(set);
        }

        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> set, IEqualityComparer<T> cmp)
        {
            return new HashSet<T>(set, cmp);
        }

        /// <summary>
        /// move next and retrieve current from the given enumerator
        /// </summary>        
        public static T Next<T>(this IEnumerator<T> en)
        {
            en.MoveNext();
            return en.Current;
        }

        /// <summary>
        /// create a rotated list where lst[N-1] satisfy the given lastRule, and lst[0] satisfy firstRule        
        /// </summary>        
        public static IEnumerable<T> RotateListUntil<T>(this IReadOnlyList<T> lst, Func<T, bool> lastRule, Func<T, bool> firstRule)
        {
            var idxFirstRule = -1;
            for (int i = 0; i < lst.Count; ++i)
            {
                if (i == 0)
                {
                    if (firstRule(lst[i]) && lastRule(lst[lst.Count - 1]))
                    {
                        idxFirstRule = i;
                        break;
                    }
                }
                else
                {
                    if (lastRule(lst[i - 1]) && firstRule(lst[i]))
                    {
                        idxFirstRule = i;
                        break;
                    }
                }
            }

            if (idxFirstRule == -1) yield break;

            var j = idxFirstRule;
            var cnt = lst.Count;
            while (cnt > 0)
            {
                yield return lst[j];
                ++j; if (j == lst.Count) j = 0;

                --cnt;
            }
        }

        /// <summary>
        /// from a IList retrieve last n items
        /// </summary>        
        public static IEnumerable<T> TakeLast<T>(this IReadOnlyList<T> lst, int n)
        {
            var idx = lst.Count - n;

            while (idx < lst.Count)
            {
                yield return lst[idx];

                ++idx;
            }
        }

    }

}
