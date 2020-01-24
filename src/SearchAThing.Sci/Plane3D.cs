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

namespace SearchAThing.Sci
{

    public class Plane3D
    {

        public CoordinateSystem3D CS { get; private set; }
        
        /// <summary>
        /// XY(z) plane : top view
        /// </summary>
        public static Plane3D XY = new Plane3D(CoordinateSystem3D.XY);

        /// <summary>
        /// XZ(-y) plane : front view
        /// </summary>
        public static Plane3D XZ = new Plane3D(CoordinateSystem3D.XZ);        

        /// <summary>
        /// YZ(x) plane : side view
        /// </summary>
        public static Plane3D YZ = new Plane3D(CoordinateSystem3D.YZ);

        public Plane3D(CoordinateSystem3D cs)
        {
            CS = cs;            
        }

        public bool Contains(double tol, Vector3D pt)
        {
            return pt.ToUCS(CS).Z.EqualsTol(tol, 0);
        }

        public bool Contains(double tol, Line3D line)
        {
            return Contains(tol, line.From) && Contains(tol, line.To);
        }   

    }


}
