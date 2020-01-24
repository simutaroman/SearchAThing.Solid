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
using System.Collections;
using System.Collections.Generic;

namespace SearchAThing.Core
{

    public class CircularList<D>
    {
        D[] items;
        int beginIdx = 0;
        int curIdx = 0;

        /// <summary>
        /// Items in the circular list from oldest to newest.
        /// </summary>
        public IEnumerable<D> Items
        {
            get
            {
                return new CircularListEnumerable(this);
            }
        }

        /// <summary>
        /// Size of the circular list.
        /// When adding elements more than this size existing oldest elements will be overwritten.
        /// </summary>
        public int SizeMax { get; private set; }

        /// <summary>
        /// Gets count of items in the circular list.
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        /// Add given item to the circular list.
        /// </summary>  
        public void Add(D data)
        {
            if (Count < SizeMax)
            {
                items[curIdx++] = data;
                ++Count;
            }
            else // use of circular logic
            {
                ++beginIdx;
                if (beginIdx >= SizeMax) beginIdx = 0;

                if (curIdx >= SizeMax) curIdx = 0;

                items[curIdx++] = data;
            }
        }

        /// <summary>
        /// Returns the item by given index.
        /// Note that the index 0 corrspond to the oldest inserted (max Count-th item).
        /// </summary>  
        public D GetItem(int index)
        {
            if (index >= Count)
                throw new IndexOutOfRangeException();

            var i = beginIdx + index;
            if (i >= SizeMax) i = i % SizeMax;

            return items[i];
        }

        public CircularList(int size)
        {
            SizeMax = size;
            items = new D[SizeMax];
        }

        #region CircularListEnumerable
        class CircularListEnumerable : IEnumerable<D>
        {
            CircularList<D> circularList;

            public IEnumerator<D> GetEnumerator()
            {
                return new CircularListEnumerator(circularList);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public CircularListEnumerable(CircularList<D> cl)
            {
                circularList = cl;
            }
        }
        #endregion

        #region CircularListEnumerator
        class CircularListEnumerator : IEnumerator<D>
        {
            int pos = -1;
            CircularList<D> circularList;

            public D Current
            {
                get
                {
                    return circularList.GetItem(pos);
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    return Current;
                }
            }

            public CircularListEnumerator(CircularList<D> cl)
            {
                circularList = cl;
            }

            public void Dispose()
            {                                
            }

            public bool MoveNext()
            {               
                ++pos;
                return (pos < circularList.Count);
            }

            public void Reset()
            {
                pos = -1;
            }

        }
        #endregion

    }

}
