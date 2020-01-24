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

using System.Collections.Generic;
using System.Threading;

namespace SearchAThing
{

    namespace Core
    {

        /// <summary>
        /// Pattern to allow access the root document from the nested child objects.
        /// Example here : https://github.com/devel0/SearchAThing.Patterns/blob/master/src/MongoScopedChildren/Program.cs
        /// </summary>
        public static class ScopedChildrenManager
        {

            static Dictionary<int, List<IScopedChild>> scopedChildren = new Dictionary<int, List<IScopedChild>>();

            public static List<IScopedChild> GetScopedChildrenList()
            {
                List<IScopedChild> res = null;

                var thId = Thread.CurrentThread.ManagedThreadId;

                if (!scopedChildren.TryGetValue(thId, out res))
                {
                    res = new List<IScopedChild>();
                    scopedChildren.Add(thId, res);
                }

                return res;
            }

            public static void ClearScopedChildrenList()
            {
                GetScopedChildrenList().Clear();
            }

            public static void RegisterScopedChild(IScopedChild child)
            {
                GetScopedChildrenList().Add(child);
            }

        }

    }

}
