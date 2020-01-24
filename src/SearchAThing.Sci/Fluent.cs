#region SearchAThing.Sci, Copyright(C) 2016-2017 Lorenzo Delana, License under MIT
/*
* The MIT License(MIT)
* Copyright(c) 2016-2017 Lorenzo Delana, https://searchathing.com
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

using static System.Math;
using System.Collections.Generic;

namespace SearchAThing
{

    public static partial class Extensions
    {

        /// <summary>
        /// Return the min distance between two adiacent number
        /// given from all of the given ordered set of numbers.        
        /// </summary>                
        /// <returns>0 if empty set or 1 element. min distance otherwise.</returns>
        public static double MinDistance(this IEnumerable<double> orderedNumbers)
        {
            var min = double.MaxValue;
            var en = orderedNumbers.GetEnumerator();
            if (!en.MoveNext()) return 0.0;
            var x = en.Current;
            if (!en.MoveNext()) return 0.0;

            do
            {
                var y = en.Current;
                var dst = Abs(y - x);
                if (dst < min) min = dst;
                x = y;
            }
            while (en.MoveNext());

            return min;
        }

    }

}
