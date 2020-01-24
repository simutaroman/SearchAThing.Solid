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

using SearchAThing.Core;
using System;
using System.Collections.Generic;
using static System.Math;

namespace SearchAThing
{

    namespace Core
    {

        public class TaggedObject<O, T>
        {

            public TaggedObject(O o, T t) { Obj = o; Tag = t; }

            public O Obj { get; set; }

            public T Tag { get; set; }

        }

        public class TaggedObject<O, T1, T2>
        {

            public TaggedObject(O o, T1 t1, T2 t2) { Obj = o; Tag1 = t1; Tag2 = t2; }

            public O Obj { get; set; }

            public T1 Tag1 { get; set; }
            public T2 Tag2 { get; set; }

        }

        public class TaggedObject<O, T1, T2, T3>
        {
            public TaggedObject(O o, T1 t1, T2 t2, T3 t3) { Obj = o; Tag1 = t1; Tag2 = t2; Tag3 = t3; }

            public O Obj { get; set; }

            public T1 Tag1 { get; set; }
            public T2 Tag2 { get; set; }
            public T3 Tag3 { get; set; }

        }

        public class ObjectContainer<T>
        {

            public T Obj { get; set; }

        }

        public static partial class Util
        {

            /// <summary>
            /// dummy fn evaluator
            /// it can be used to generate value type in anonymous property declarations            
            /// </summary>            
            public static T Eval<T>(Func<T> fn)
            {
                return fn();
            }

        }

    }

    public static partial class Extensions
    {

        public static TaggedObject<O, T> TaggedObject<O, T>(this O o, T t) { return new TaggedObject<O, T>(o, t); }
        public static TaggedObject<O, T1, T2> TaggedObject<O, T1, T2>(this O o, T1 t1, T2 t2) { return new TaggedObject<O, T1, T2>(o, t1, t2); }
        public static TaggedObject<O, T1, T2, T3> TaggedObject<O, T1, T2, T3>(this O o, T1 t1, T2 t2, T3 t3) { return new TaggedObject<O, T1, T2, T3>(o, t1, t2, t3); }

        public static R Eval<T, R>(this T o, Func<T, R> fn)
        {
            return fn(o);
        }

        /// <summary>
        /// searches on a valuetype IEnumerable and return a nullable null object if not found
        /// </summary>        
        public static T? FirstOrNull<T>(this IEnumerable<T> t, Func<T, bool> predicate) where T : struct
        {
            var en = t.GetEnumerator();

            while (en.MoveNext())
            {
                if (predicate(en.Current)) return en.Current;
            }

            return null;
        }

        /// <summary>
        /// sweep given list from given start_idx stepping with given step
        /// until bounds not exceeded or end function not reached
        /// </summary>        
        public static IEnumerable<T> Sweep<T>(this IReadOnlyList<T> lst, int start_idx, int step, Func<int, T, bool> end = null) where T : class
        {
            var idx = start_idx;
            while (true)
            {
                if (idx < 0 || idx >= lst.Count) yield break;

                if (end != null && end(idx, lst[idx])) yield break;

                yield return lst[idx];

                idx += step;
            }
        }

    }

}
