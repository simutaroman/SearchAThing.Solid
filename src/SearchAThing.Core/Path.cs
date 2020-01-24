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
using System.IO;
using System.Reflection;

namespace SearchAThing
{

    namespace Core
    {

        public static class Path
        {

            /// <summary>
            /// {AppData}/{namespace}/{assembly_name}
            /// </summary>
            public static string AppDataFolder(string ns)
            {
                return EnsureFolder(System.IO.Path.Combine(
                    System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ns),
                    Assembly.GetCallingAssembly().GetName().Name));

            }

            /// <summary>
            /// Ensure given folder path exists.
            /// </summary>        
            public static string EnsureFolder(this string path)
            {
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                return path;
            }

            /// <summary>
            /// Search given filename in the PATH
            /// </summary>
            /// <returns>null if not found</returns>
            public static string SearchInPath(this string filename)
            {
                if (File.Exists(filename)) return System.IO.Path.GetFullPath(filename);

                var paths = Environment.GetEnvironmentVariable("PATH");
                foreach (var path in paths.Split(System.IO.Path.PathSeparator))
                {                    
                    var pathname = System.IO.Path.Combine(path, filename);
                    if (File.Exists(pathname)) return pathname;
                }
                return null;
            }

        }

    }

    public static partial class Extensions
    {



    }

}
