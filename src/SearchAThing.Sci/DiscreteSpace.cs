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
using System.Linq;
using static System.Math;
using System.Collections.Generic;

namespace SearchAThing.Sci
{

    public class DiscreteSpaceItem<T>
    {

        public Vector3D Mean { get; private set; }

        public T Item { get; private set; }

        internal DiscreteSpaceItem(Vector3D pt)
        {
            Mean = pt;
        }

        public DiscreteSpaceItem(T _Item, Func<T, Vector3D> entPoint)
        {
            Item = _Item;
            Mean = entPoint(Item);
        }

        public DiscreteSpaceItem(T _Item, Func<T, IEnumerable<Vector3D>> entPoints)
        {
            Item = _Item;
            Mean = entPoints(Item).Mean();
        }

    }

    public class DiscreteSpaceItemComparer<T> : IComparer<DiscreteSpaceItem<T>>
    {
        double tol;
        int ord;

        public DiscreteSpaceItemComparer(double _tol, int _ord)
        {
            tol = _tol;
            ord = _ord;
        }

        public int Compare(DiscreteSpaceItem<T> x, DiscreteSpaceItem<T> y)
        {
            return x.Mean.GetOrd(ord).CompareTol(tol, y.Mean.GetOrd(ord));
        }
    }

    public class DiscreteSpace<T>
    {

        List<DiscreteSpaceItem<T>>[] sorted;
        DiscreteSpaceItemComparer<T>[] cmp;

        double tol;
        int spaceDim;

        DiscreteSpace(double _tol, List<DiscreteSpaceItem<T>> q, int _spaceDim)
        {
            tol = _tol;
            spaceDim = _spaceDim;            

            sorted = new List<DiscreteSpaceItem<T>>[spaceDim];
            cmp = new DiscreteSpaceItemComparer<T>[spaceDim];

            for (int ord = 0; ord < spaceDim; ++ord)
            {
                sorted[ord] = q.OrderBy(w => w.Mean.GetOrd(ord)).ToList();
                cmp[ord] = new DiscreteSpaceItemComparer<T>(tol, ord);
            }
        }

        /// <summary>
        /// Build a discrete space to search within GetItemsAt.
        /// spaceDim need to equals 3 when using vector in 3d
        /// </summary>        
        public DiscreteSpace(double _tol, IEnumerable<T> ents, Func<T, IEnumerable<Vector3D>> entPoints, int _spaceDim) :
            this(_tol, ents.Select(ent => new DiscreteSpaceItem<T>(ent, entPoints)).ToList(), _spaceDim)
        {            
        }

        /// <summary>
        /// Build a discrete space to search within GetItemsAt.
        /// spaceDim need to equals 3 when using vector in 3d
        /// </summary>        
        public DiscreteSpace(double _tol, IEnumerable<T> ents, Func<T, Vector3D> entPoint, int _spaceDim) :
            this(_tol, ents.Select(ent => new DiscreteSpaceItem<T>(ent, entPoint)).ToList(), _spaceDim)
        {
        }

        IEnumerable<T> GetItemsAt_R(int ord, double off, double maxDist)
        {
            var itosearch = new DiscreteSpaceItem<T>(Vector3D.Axis(ord) * off);

            // bin search return 0,(n-1) if found or negative number so that ~result is the index of 
            // the first element greather thant those we search for
            var bsr = sorted[ord].BinarySearch(itosearch, cmp[ord]);

            int idx = 0;
            if (bsr < 0)
                idx = ~bsr;
            else
                idx = bsr;

            DiscreteSpaceItem<T> dsi = null;

            // right search
            for (int i = idx; i < sorted[ord].Count && Abs((dsi = sorted[ord][i]).Mean.GetOrd(ord) - off) <= maxDist; ++i)
                yield return dsi.Item;

            // left search
            for (int i = idx - 1; i >= 0 && Abs((dsi = sorted[ord][i]).Mean.GetOrd(ord) - off) <= maxDist; --i)
                yield return dsi.Item;

        }

        public IEnumerable<T> GetItemsAt(Vector3D pt, double maxDist)
        {
            IEnumerable<T> set = null;

            for (int dim = 0; dim < spaceDim; ++dim)
            {
                var thisDimSet = GetItemsAt_R(dim, pt.GetOrd(dim), maxDist);

                if (dim == 0)
                    set = thisDimSet;
                else
                    set = set.Intersect(thisDimSet);
            }

            return set;
        }

    }

}
