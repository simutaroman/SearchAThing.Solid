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
using System.Collections;
using System.Collections.Generic;

namespace SearchAThing
{
    
    namespace Core
    {

        public class CircularEnumerator<D> : IEnumerator<D>
        {

            IEnumerator<D> enumerator;

            public CircularEnumerator(IEnumerable<D> en)
            {
                enumerator = en.GetEnumerator();
            }

            public D Current
            {
                get
                {
                    return enumerator.Current;
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    return enumerator.Current;
                }
            }

            public void Dispose()
            {
                enumerator.Dispose();
            }

            public bool MoveNext()
            {
                if (!enumerator.MoveNext())
                {
                    enumerator.Reset();
                    return enumerator.MoveNext();
                }
                return true;
            }

            public void Reset()
            {
                enumerator.Reset();
            }
        }

        public class CircularEnumerable<D> : IEnumerable<D>
        {
            IEnumerable<D> enumerable;

            public CircularEnumerable(IEnumerable<D> _enumerable)
            {
                enumerable = _enumerable;
            }

            public IEnumerator<D> GetEnumerator()
            {
                return new CircularEnumerator<D>(enumerable);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return new CircularEnumerator<D>(enumerable);
            }
        }

    }

    public static partial class Extensions
    {

        public static IEnumerable<D> AsCircularEnumerable<D>(this IEnumerable<D> enumerable)
        {
            return new CircularEnumerable<D>(enumerable);
        }

        public static IEnumerator<D> AsCircularEnumerator<D>(this IEnumerable<D> enumerable)
        {
            return enumerable.AsCircularEnumerable().GetEnumerator();
        }        

    }

}
