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

using System.Collections.Generic;
using System.Text;
using System.Linq;
using SearchAThing.Sci;

namespace SearchAThing
{

    public static partial class Extensions
    {

        /// <summary>
        /// exports to a csv string some known fields
        /// note: not really a csv its a tab separated values for debug purpose
        /// just copy and paste
        /// </summary>        
        public static string ToCSV(this IEnumerable<object> lst)
        {
            var finalSb = new StringBuilder();

            if (!lst.Any()) return "";

            var qT = lst.First().GetType();

            var sbHeader = new StringBuilder();

            // header
            foreach (var x in qT.GetProperties())
            {                
                if (x.PropertyType == typeof(Vector3D))
                {                    
                    if (sbHeader.Length > 0) sbHeader.Append('\t');
                    sbHeader.Append($"{x.Name}.X\t{x.Name}.Y\t{x.Name}.Z");
                }
                else
                {                    
                    if (sbHeader.Length > 0) sbHeader.Append('\t');
                    sbHeader.Append($"{x.Name}");
                }                
            }

            finalSb.AppendLine(sbHeader.ToString());

            // data
            foreach (var o in lst)
            {
                var sbLine = new StringBuilder();

                foreach (var x in qT.GetProperties())
                {
                    if (x.PropertyType == typeof(string))
                    {
                        var s = (string)x.GetMethod.Invoke(o, null);

                        if (sbLine.Length > 0) sbLine.Append('\t');
                        sbLine.Append($"{s}");
                    }
                    else if (x.PropertyType == typeof(double))
                    {
                        var d = (double)x.GetMethod.Invoke(o, null);

                        if (sbLine.Length > 0) sbLine.Append('\t');
                        sbLine.Append($"{d}");
                    }
                    else if (x.PropertyType == typeof(Vector3D))
                    {
                        var v = (Vector3D)x.GetMethod.Invoke(o, null);

                        foreach (var c in v.Coordinates)
                        {
                            if (sbLine.Length > 0) sbLine.Append('\t');
                            sbLine.Append($"{c}");
                        }
                    }
                    else
                    {
                        var s = (object)x.GetMethod.Invoke(o, null);

                        if (sbLine.Length > 0) sbLine.Append('\t');
                        sbLine.Append($"{s.ToString()}");
                    }
                }

                finalSb.AppendLine(sbLine.ToString());
            }

            return finalSb.ToString();
        }

    }

}
