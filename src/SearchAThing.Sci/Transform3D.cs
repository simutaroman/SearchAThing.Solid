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

using sVector3D = Microsoft.Xna.Framework.Vector3;//System.Windows.Media.Media3D.Vector3D;
using sMatrix3D = Microsoft.Xna.Framework.Matrix;//System.Windows.Media.Media3D.Matrix3D;
using sQuaternion = Microsoft.Xna.Framework.Quaternion;//System.Windows.Media.Media3D.Quaternion;
using SearchAThing;
using Microsoft.Xna.Framework;

namespace SearchAThing.Sci
{

    public class Transform3D
    {

        sMatrix3D m;

        static sVector3D sXAxis = new sVector3D(1, 0, 0);
        static sVector3D sYAxis = new sVector3D(0, 1, 0);
        static sVector3D sZAxis = new sVector3D(0, 0, 1);

        public sMatrix3D TransformMatrix { get { return m; } }

        public Transform3D()
        {
            m = sMatrix3D.Identity;
        }

        public void RotateAboutXAxis(double angleRad)
        {
            m = m * sMatrix3D.CreateFromQuaternion(sQuaternion.CreateFromAxisAngle(sXAxis, (float)angleRad));
            //m.Rotate(new sQuaternion(sXAxis, angleRad.ToDeg()));
        }

        public void RotateAboutYAxis(double angleRad)
        {
            m = m * sMatrix3D.CreateFromQuaternion(sQuaternion.CreateFromAxisAngle(sYAxis, (float)angleRad));
            //m.Rotate(new sQuaternion(sYAxis, angleRad.ToDeg()));
        }

        public void RotateAboutZAxis(double angleRad)
        {
            m = m * sMatrix3D.CreateFromQuaternion(sQuaternion.CreateFromAxisAngle(sZAxis, (float)angleRad));
            //m.Rotate(new sQuaternion(sZAxis, angleRad.ToDeg()));
        }

        public void RotateAboutAxis(Vector3D axis, double angleRad)
        {
            var v = new sVector3D((float)axis.X, (float)axis.Y, (float)axis.Z);
            m = m * sMatrix3D.CreateFromQuaternion(sQuaternion.CreateFromAxisAngle(v, (float)angleRad));
            //m.Rotate(new sQuaternion(new sVector3D(axis.X, axis.Y, axis.Z), angleRad.ToDeg()));
        }

        public Vector3D Apply(Vector3D v)
        {
            return sVector3D.Transform(v.ToSystemVector3D(), m).ToVector3D();
            //return m.Transform(v.ToSystemVector3D()).ToVector3D();
        }

    }

}
