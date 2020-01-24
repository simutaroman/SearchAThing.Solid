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
using System.Globalization;
using System.Text;
using static System.Math;
using System.Linq;

namespace SearchAThing
{

    public static partial class Extensions
    {

        /// <summary>
        /// build a markdown table from the list of given objects using their properties as columns
        /// </summary>        
        public static string ToMarkdownTable(this IEnumerable<dynamic> objs)
        {
            var sb = new StringBuilder();

            var props = ((object)objs.First()).GetType().GetProperties();

            var colwidths = new int[props.Length];
            for (int i = 0; i < props.Length; ++i)
            {
                colwidths[i] = Max(colwidths[i], props[i].Name.Length);
            }
            foreach (var x in objs)
            {
                for (int i = 0; i < props.Length; ++i)
                {
                    var v = props[i].GetValue(x);

                    if (v != null) colwidths[i] = Max(colwidths[i], v.ToString().Length);
                }
            }

            for (int i = 0; i < props.Length; ++i)
            {
                var fmt = $" {{0,-{colwidths[i]}}} ";
                sb.Append(string.Format(fmt, props[i].Name));
                if (i != props.Length - 1) sb.Append('|');
            }
            sb.AppendLine();

            for (int i = 0; i < props.Length; ++i)
            {
                sb.Append("-".Repeat(colwidths[i] + 2));
                if (i != props.Length - 1) sb.Append('+');
            }
            sb.AppendLine();

            foreach (var o in objs)
            {
                for (int i = 0; i < props.Length; ++i)
                {
                    var fmt = $" {{0,-{colwidths[i]}}} ";

                    var v = props[i].GetValue(o);

                    if (v != null)
                        sb.Append(string.Format(fmt, v.ToString()));
                    else
                        sb.Append(string.Format(fmt, ""));

                    if (i != props.Length - 1) sb.Append('|');
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }


    }
}