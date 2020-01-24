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

using System;
using System.Collections.Generic;
using System.Linq;

namespace SearchAThing.Sci
{

    /// <summary>
    /// Scan a given domain of doubles, determine the midpoint ( Origin )
    /// and using the given tolerance it tests for integrity in conversion between values
    /// from double to Int64 and vice-versa.
    /// It can generate a Int64MapExceptionRange.
    /// </summary>
    public class Int64Map
    {

        public double Origin { get; private set; }
        public double Tolerance { get; private set; }

        /// <summary>
        /// use small tolerance to avoid lost of precision
        /// Note: too small tolerance can generate Int64MapExceptionRange
        /// </summary>        
        public Int64Map(double tol, IEnumerable<double> domainValues, bool selfCheckTolerance = true)
        {
            var min = double.MaxValue;
            var max = double.MinValue;
            Tolerance = tol;

            foreach (var x in domainValues)
            {
                if (x < min) min = x;
                if (x > max) max = x;
            }

            Origin = (min + max) / 2;

            // self check
            if (selfCheckTolerance)
            {
                foreach (var x in domainValues)
                {
                    var fromint = FromInt64(ToInt64(x));

                    if (!fromint.EqualsTol(tol, x)) throw new Int64MapExceptionRange($"can't fit given domain within Int64 types. Try bigger tolerance.");
                }
            }
        }
        
        public Int64 ToInt64(double x) { return (Int64)((x - Origin) / Tolerance); }
        public double FromInt64(Int64 x) { return (((double)x) * Tolerance) + Origin; }

    }

    public class Int64MapExceptionRange : Exception
    {

        public Int64MapExceptionRange(string msg) : base(msg)
        {
        }

    }

}
