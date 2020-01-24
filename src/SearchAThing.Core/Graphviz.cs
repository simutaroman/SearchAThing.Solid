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

using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SearchAThing
{

    public static partial class Extensions
    {

        /// <summary>
        /// create a dot (graphviz) diagram from given path
        /// ( rankdir can be set to "LR" )
        /// </summary>        
        public static string PathToDot(this string _path, string rankdir = null)
        {
            var sb = new StringBuilder();

            sb.AppendLine(@"digraph G {");
            if (rankdir != null) sb.AppendLine($"rankdir={rankdir}");

            var path_ids = new Dictionary<string, string>();

            Func<string, string> new_id = (path) =>
            {
                var id = $"id{path_ids.Count}";
                path_ids.Add(id, path);
                sb.AppendLine($"{id} [ label=\"{Path.GetFileName(path)}\" ]");
                return id;
            };

            Action<string, string> scan = null;

            scan = (parent_id, path) =>
            {
                if (parent_id == null) parent_id = new_id(path);

                if (Directory.Exists(path)) // its a dir                
                {
                    int i = 0;
                    foreach (var dir in Directory.GetDirectories(path))
                    {
                        var id = new_id(dir);

                        sb.AppendLine($"{parent_id} -> {id}");

                        scan(id, dir);

                        ++i;
                    }
                }

                foreach (var file in Directory.GetFiles(path))
                {
                    var id = new_id(file);

                    sb.AppendLine($"{parent_id} -> {id}");
                }
            };

            if (File.Exists(_path)) throw new Exception($"require path as folder");

            scan(null, _path);

            sb.Append("}");

            return sb.ToString();
        }

    }

}
