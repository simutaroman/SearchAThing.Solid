﻿#region SearchAThing.Sci, Copyright(C) 2016-2017 Lorenzo Delana, License under MIT
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
using SearchAThing.Sci;
using System.Text;
using System.Globalization;
using System.Linq;
using static System.Math;
using netDxf.Entities;
using Newtonsoft.Json;
using System;

namespace SearchAThing
{

    namespace Sci
    {

        public enum Line3DConstructMode { PointAndVector };

        public enum Line3DSegmentMode
        {
            /// <summary>
            /// infinite line
            /// </summary>
            None,

            /// <summary>
            /// Semi-line start at From
            /// </summary>
            From,

            /// <summary>
            /// Semi-line ending at To
            /// </summary>
            To,

            /// <summary>
            /// Segment from-to
            /// </summary>
            FromTo
        };

        public enum LineIntersectBehavior
        {
            MidPoint,
            PointOnThis,
            PointOnOther
        }

        public class Line3D : Geometry
        {
            public static Line3D XAxisLine = new Line3D(Vector3D.Zero, Vector3D.XAxis);
            public static Line3D YAxisLine = new Line3D(Vector3D.Zero, Vector3D.YAxis);
            public static Line3D ZAxisLine = new Line3D(Vector3D.Zero, Vector3D.ZAxis);

            public Vector3D From { get; private set; }
            public Vector3D V { get; private set; }
            public Vector3D To { get { return From + V; } }
            public Vector3D Dir { get { return (To - From).Normalized(); } }

            [JsonIgnore]
            public override Vector3D GeomFrom => From;

            [JsonIgnore]
            public override Vector3D GeomTo => To;

            [JsonIgnore]
            public override IEnumerable<Vector3D> Vertexes
            {
                get
                {
                    yield return From;
                    yield return To;
                }
            }

            /// <summary>
            /// retrieve a unique endpoint representation of this line3d segment (regardless its from-to or to-from order)
            /// such that From.Distance(Vector3D.Zero) less than To.Distance(Vector3D.Zero)
            /// </summary>
            public IEnumerable<Vector3D> DisambiguatedPoints
            {
                get
                {
                    if (From.Distance(Vector3D.Zero) < To.Distance(Vector3D.Zero))
                    {
                        yield return From;
                        yield return To;
                    }
                    else
                    {
                        yield return To;
                        yield return From;
                    }
                }
            }

            public IEnumerable<Vector3D> Points
            {
                get
                {
                    yield return From;
                    yield return To;
                }
            }

            /// <summary>
            /// build segment
            /// </summary>            
            public Line3D(Vector3D from, Vector3D to) : base(GeometryType.Line3D)
            {
                From = from;
                V = to - from;
            }

            /// <summary>
            /// z=0
            /// </summary>            
            public Line3D(double x1, double y1, double x2, double y2) : base(GeometryType.Line3D)
            {
                From = new Vector3D(x1, y1);
                V = new Vector3D(x2, y2) - From;
            }

            public Line3D(double x1, double y1, double z1, double x2, double y2, double z2) : base(GeometryType.Line3D)
            {
                From = new Vector3D(x1, y1, z1);
                V = new Vector3D(x2, y2, z2) - From;
            }

            /// <summary>
            /// build segment from plus the given vector form to
            /// </summary>            
            public Line3D(Vector3D from, Vector3D v, Line3DConstructMode mode) : base(GeometryType.Line3D)
            {
                From = from;
                V = v;
            }

            public override double Length { get { return V.Length; } }

            /// <summary>
            /// Checks if two lines are equals ( it checks agains swapped from-to too )
            /// </summary>        
            public bool EqualsTol(double tol, Line3D other)
            {
                return
                    (From.EqualsTol(tol, other.From) && To.EqualsTol(tol, other.To))
                    ||
                    (From.EqualsTol(tol, other.To) && To.EqualsTol(tol, other.From));
            }          

            /// <summary>
            /// returns the common point from,to between two lines or null if not consecutives
            /// </summary>        
            public Vector3D CommonPoint(double tol, Line3D other)
            {
                if (From.EqualsTol(tol, other.From) || From.EqualsTol(tol, other.To)) return From;
                if (To.EqualsTol(tol, other.From) || To.EqualsTol(tol, other.To)) return To;

                return null;
            }

            /// <summary>
            /// return the segment with swapped from,to
            /// </summary>            
            public Line3D Reverse()
            {
                return new Line3D(To, From);
            }

            /// <summary>
            /// scale from,to of this line using given refpt and factor
            /// </summary>            
            public Line3D Scale(Vector3D refpt, double factor)
            {
                return new Line3D(From.ScaleAbout(refpt, factor), To.ScaleAbout(refpt, factor));
            }

            #region operators
            /// <summary>
            /// multiply Length by given scalar factor
            /// Note : this will change To
            /// </summary>        
            public static Line3D operator *(double s, Line3D l)
            {
                return new Line3D(l.From, l.V * s, Line3DConstructMode.PointAndVector);
            }

            /// <summary>
            /// multiply Length by given scalar factor
            /// Note : this will change To
            /// </summary>        
            public static Line3D operator *(Line3D l, double s)
            {
                return new Line3D(l.From, l.V * s, Line3DConstructMode.PointAndVector);
            }

            /// <summary>
            /// Move this line of given delta adding value either at From, To
            /// </summary>            
            public static Line3D operator +(Line3D l, Vector3D delta)
            {
                return new Sci.Line3D(l.From + delta, l.V, Line3DConstructMode.PointAndVector);
            }

            /// <summary>
            /// Move this line of given delta subtracting value either at From, To
            /// </summary>            
            public static Line3D operator -(Line3D l, Vector3D delta)
            {
                return new Sci.Line3D(l.From - delta, l.V, Line3DConstructMode.PointAndVector);
            }
            #endregion

            /// <summary>
            /// Infinite line contains point.
            /// Note: tol must be Constant.NormalizedLengthTolerance
            /// if comparing normalized vectors
            /// </summary>        
            public bool LineContainsPoint(double tol, double x, double y, double z, bool segmentMode = false)
            {
                return LineContainsPoint(tol, new Vector3D(x, y, z), segmentMode);
            }

            /// <summary>
            /// Infinite line contains point.            
            /// </summary>        
            public bool LineContainsPoint(double tol, Vector3D p, bool segmentMode = false, bool excludeExtreme = false)
            {
                if (this.Length.EqualsTol(tol, 0)) return false;

                var prj = p.Project(this);

                var dprj = p.Distance(prj);

                // check if line contains point
                if (dprj > tol) return false;

                if (segmentMode)
                {
                    // line contains given point if there is a scalar s 
                    // for which p = From + s * V 
                    var s = 0.0;

                    // to find out the scalar we need to test the first non null component 

                    if (!(V.X.EqualsTol(tol, 0))) s = (p.X - From.X) / V.X;
                    else if (!(V.Y.EqualsTol(tol, 0))) s = (p.Y - From.Y) / V.Y;
                    else if (!(V.Z.EqualsTol(tol, 0))) s = (p.Z - From.Z) / V.Z;

                    if (excludeExtreme)
                    {
                        if (p.EqualsTol(tol, From)) return false;
                        if (p.EqualsTol(tol, To)) return false;

                        return (s > 0 && s < 1);
                    }
                    else
                    {
                        // s is the scalar of V vector that runs From->To 

                        if (s >= 0.0 && s <= 1.0) return true;

                        // point on the line but outside exact segment
                        // check with tolerance

                        if (s < 0)
                            return p.EqualsTol(tol, From);
                        else
                            return p.EqualsTol(tol, To);
                    }
                }

                return true;
            }

            /// <summary>
            /// Finite segment contains point.
            /// Note: tol must be Constant.NormalizedLengthTolerance
            /// if comparing normalized vectors
            /// </summary>        
            public bool SegmentContainsPoint(double tol, Vector3D p, bool excludeExtreme = false)
            {
                return LineContainsPoint(tol, p, segmentMode: true, excludeExtreme: excludeExtreme);
            }

            /// <summary>
            /// Finite segment contains point.
            /// Note: tol must be Constant.NormalizedLengthTolerance
            /// if comparing normalized vectors
            /// </summary>        
            public bool SegmentContainsPoint(double tol, double x, double y, double z)
            {
                return LineContainsPoint(tol, x, y, z, segmentMode: true);
            }

            /// <summary>
            /// Find intersection point between this and other line using given tolerance.
            /// Returns null if no intersection, otherwise it returns a point on
            /// the shortest segment ( the one that's perpendicular to either lines )
            /// based on given behavior ( default midpoint ).
            /// </summary>            
            public Vector3D Intersect(double tol, Line3D other,
                LineIntersectBehavior behavior = LineIntersectBehavior.MidPoint)
            {
                var res = Intersect2(tol, other);

                if (res != null)
                {
                    switch (behavior)
                    {
                        case LineIntersectBehavior.MidPoint: return res.MidPoint;
                        case LineIntersectBehavior.PointOnThis: return res.From;
                        case LineIntersectBehavior.PointOnOther: return res.To;
                    }
                }

                // not intersection
                return null;
            }

            /// <summary>
            /// Find intersection point between this and other line using given tolerance.
            /// Returns null if no intersection, otherwise it returns
            /// the shortest segment ( the one that's perpendicular to either lines )            
            /// </summary>        
            public Line3D Intersect2(double tol, Line3D other)
            {
                // this  : t = tf + tu * tv
                // other : o = of + ou * ov                
                // res   : r = rf + ru * rv
                //
                // giving res starting from this and toward other
                //   rf = tf + tu * tv
                //   rv = of + ou * ov - tf - tu * tv
                //
                // result:
                //   r = Line3D(tf + tu * tv, of + ou * ov)
                //   <=>
                //   r perpendicular to t and o :
                //     (1) rv.DotProduct(tv) = 0
                //     (2) rv.DotProduct(ov) = 0
                //                
                //     (1)
                //       rvx * tvx + rvy * tvy + rvz * tvz = 0 <=>
                //       (ofx + ou * ovx - tfx - tu * tvx) * tvx +
                //       (ofy + ou * ovy - tfy - tu * tvy) * tvy +
                //       (ofz + ou * ovz - tfz - tu * tvz) * tvz = 0
                //
                //     (2)
                //       rvx * ovx + rvy * ovy + rvz * ovz = 0 <=>
                //       (ofx + ou * ovx - tfx - tu * tvx) * ovx +
                //       (ofy + ou * ovy - tfy - tu * tvy) * ovy +
                //       (ofz + ou * ovz - tfz - tu * tvz) * ovz = 0
                //
                //     unknowns ( tu, ou )
                //     

                // solution through python sympy
                // 
                /*
                    from sympy import *

                    ou, tu = symbols('ou tu')
                    ofx, ovx, tfx, tvx = symbols('ofx ovx tfx tvx')
                    ofy, ovy, tfy, tvy = symbols('ofy ovy tfy tvy')
                    ofz, ovz, tfz, tvz = symbols('ofz ovz tfz tvz')

                    eq1 = Eq((ofx + ou * ovx - tfx - tu * tvx) * tvx +
                             (ofy + ou * ovy - tfy - tu * tvy) * tvy +
                             (ofz + ou * ovz - tfz - tu * tvz) * tvz, 0)
                    eq2 = Eq((ofx + ou * ovx - tfx - tu * tvx) * ovx +
                             (ofy + ou * ovy - tfy - tu * tvy) * ovy +
                             (ofz + ou * ovz - tfz - tu * tvz) * ovz, 0)

                    print(solve([eq1, eq2], [ou, tu]))                  
                */

                // live.sympy.org url :
                // http://live.sympy.org/?evaluate=from%20sympy%20import%20*%0A%23--%0Aou%2C%20tu%20%3D%20symbols('ou%20tu')%0A%23--%0Aofx%2C%20ovx%2C%20tfx%2C%20tvx%20%3D%20symbols('ofx%20ovx%20tfx%20tvx')%0A%23--%0Aofy%2C%20ovy%2C%20tfy%2C%20tvy%20%3D%20symbols('ofy%20ovy%20tfy%20tvy')%0A%23--%0Aofz%2C%20ovz%2C%20tfz%2C%20tvz%20%3D%20symbols('ofz%20ovz%20tfz%20tvz')%0A%23--%0Aeq1%20%3D%20Eq((ofx%20%2B%20ou%20*%20ovx%20-%20tfx%20-%20tu%20*%20tvx)%20*%20tvx%20%2B%0A%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20(ofy%20%2B%20ou%20*%20ovy%20-%20tfy%20-%20tu%20*%20tvy)%20*%20tvy%20%2B%0A%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20(ofz%20%2B%20ou%20*%20ovz%20-%20tfz%20-%20tu%20*%20tvz)%20*%20tvz%2C%200)%0A%23--%0Aeq2%20%3D%20Eq((ofx%20%2B%20ou%20*%20ovx%20-%20tfx%20-%20tu%20*%20tvx)%20*%20ovx%20%2B%0A%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20(ofy%20%2B%20ou%20*%20ovy%20-%20tfy%20-%20tu%20*%20tvy)%20*%20ovy%20%2B%0A%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20(ofz%20%2B%20ou%20*%20ovz%20-%20tfz%20-%20tu%20*%20tvz)%20*%20ovz%2C%200)%20%20%20%20%20%20%0A%23--%0Asolve(%5Beq1%2C%20eq2%5D%2C%20%5Bou%2C%20tu%5D)%0A%23--%0A

                //  ou:
                //    (
                //      -(tvx**2 + tvy**2 + tvz**2)*(ofx*ovx + ofy*ovy + ofz*ovz - ovx*tfx - ovy*tfy - ovz*tfz) +
                //      (ovx*tvx + ovy*tvy + ovz*tvz)*(ofx*tvx + ofy*tvy + ofz*tvz - tfx*tvx - tfy*tvy - tfz*tvz)
                //    )
                //    /
                //    ((ovx**2 + ovy**2 + ovz**2)*(tvx**2 + tvy**2 + tvz**2) - (ovx*tvx + ovy*tvy + ovz*tvz)**2)
                //
                //  tu:
                //    (
                //      (ovx**2 + ovy**2 + ovz**2)*(ofx*tvx + ofy*tvy + ofz*tvz - tfx*tvx - tfy*tvy - tfz*tvz) -
                //      (ovx*tvx + ovy*tvy + ovz*tvz)*(ofx*ovx + ofy*ovy + ofz*ovz - ovx*tfx - ovy*tfy - ovz*tfz)
                //    )
                //    /
                //    ((ovx**2 + ovy**2 + ovz**2)*(tvx**2 + tvy**2 + tvz**2) - (ovx*tvx + ovy*tvy + ovz*tvz)**2)

                var tfx = this.From.X; var tvx = this.V.X;
                var tfy = this.From.Y; var tvy = this.V.Y;
                var tfz = this.From.Z; var tvz = this.V.Z;

                var ofx = other.From.X; var ovx = other.V.X;
                var ofy = other.From.Y; var ovy = other.V.Y;
                var ofz = other.From.Z; var ovz = other.V.Z;

                var d = ((ovx * ovx + ovy * ovy + ovz * ovz) * (tvx * tvx + tvy * tvy + tvz * tvz) -
                    Pow(ovx * tvx + ovy * tvy + ovz * tvz, 2));

                // no solution
                if (d < double.Epsilon) return null;

                var ou = (
                    -(tvx * tvx + tvy * tvy + tvz * tvz) * (ofx * ovx + ofy * ovy + ofz * ovz - ovx * tfx - ovy * tfy - ovz * tfz) +
                    (ovx * tvx + ovy * tvy + ovz * tvz) * (ofx * tvx + ofy * tvy + ofz * tvz - tfx * tvx - tfy * tvy - tfz * tvz)
                    ) / d;

                var tu = (
                    (ovx * ovx + ovy * ovy + ovz * ovz) * (ofx * tvx + ofy * tvy + ofz * tvz - tfx * tvx - tfy * tvy - tfz * tvz) -
                    (ovx * tvx + ovy * tvy + ovz * tvz) * (ofx * ovx + ofy * ovy + ofz * ovz - ovx * tfx - ovy * tfy - ovz * tfz)
                    ) / d;

                // res
                var rf = this.From + tu * this.V;
                var rt = other.From + ou * other.V;

                if (rf.EqualsTol(tol, rt))
                    return new Line3D(rf, rt);
                else
                    // not intersection within given tolerance
                    return null;
            }

            /// <summary>
            /// Intersects two lines with arbitrary segment mode for each.
            /// </summary>        
            public Vector3D Intersect(double tol, Line3D other, bool thisSegment, bool otherSegment)
            {
                var i = Intersect(tol, other);
                if (i == null) return null;

                if (thisSegment && !SegmentContainsPoint(tol, i)) return null;
                if (otherSegment && !other.SegmentContainsPoint(tol, i)) return null;

                return i;
            }

            /// <summary>
            /// Build a perpendicular vector to this one starting from the given point p.
            /// </summary>        
            public Line3D Perpendicular(double tol, Vector3D p)
            {
                if (LineContainsPoint(tol, p)) return null;

                var pRelVProj = (p - From).Project(V);

                return new Line3D(p, From + pRelVProj);
            }

            public bool Colinear(double tol, Line3D other)
            {
                return
                    (LineContainsPoint(tol, other.From) && LineContainsPoint(tol, other.To))
                    ||
                    (other.LineContainsPoint(tol, From) && other.LineContainsPoint(tol, To));
            }

            public bool IsParallelTo(double tol, Plane3D plane)
            {
                var from_ = this.From.ToUCS(plane.CS);
                var to_ = this.To.ToUCS(plane.CS);

                return from_.Z.EqualsTol(tol, to_.Z);
            }

            /// <summary>
            /// returns null if this line is parallel to the plane,
            /// the intersection point otherwise
            /// </summary>        
            public Vector3D Intersect(double tol, Plane3D plane)
            {
                if (IsParallelTo(tol, plane)) return null;

                // O = plane.Origin    Vx = plane.CS.BaseX    Vy = plane.CS.BaseY
                //
                // plane : O + alpha * Vx + beta * Vy
                // line  : From + gamma * V
                //
                // => m:{ alpha * Vx + beta * Vy - gamma * V } * s = n:{ From - O }

                var m = Matrix3D.FromVectorsAsColumns(plane.CS.BaseX, plane.CS.BaseY, -V);
                var n = From - plane.CS.Origin;
                var s = m.Solve(n);

                return From + s.Z * V;
            }
            public Vector3D MidPoint { get { return (From + To) / 2; } }

            /// <summary>
            /// rotate this segment about given axis
            /// </summary>            
            public Line3D RotateAboutAxis(Line3D axisSegment, double angleRad)
            {
                return new Line3D(From.RotateAboutAxis(axisSegment, angleRad), To.RotateAboutAxis(axisSegment, angleRad));
            }

            /// <summary>
            /// resize this segment to a new one with same From
            /// </summary>            
            public Line3D SetLength(double len)
            {
                return new Line3D(From, V.Normalized() * len, Line3DConstructMode.PointAndVector);
            }

            /// <summary>
            /// move this segment of given delta
            /// </summary>            
            public Line3D Move(Vector3D delta)
            {
                return new Line3D(From + delta, To + delta);
            }

            /// <summary>
            /// Move this segment midpoint to the given coord
            /// </summary>            
            public Line3D MoveMidpoint(Vector3D newMidpoint)
            {
                var mid = MidPoint;
                return Move(newMidpoint - mid);
            }

            /// <summary>
            /// split current segment into one or more depending on which of given split points was found on the segment            
            /// splitted segments start from begin of line
            /// TODO : not optimized
            /// </summary>            
            public IReadOnlyList<Line3D> Split(double tolLen, IReadOnlyList<Vector3D> splitPts)
            {
                var res = new List<Line3D>() { this };

                if (splitPts == null || splitPts.Count == 0) return res;

                var splitPtIdx = 0;

                while (splitPtIdx < splitPts.Count)
                {
                    List<Line3D> repl = null;

                    for (int i = 0; i < res.Count; ++i)
                    {
                        var spnt = splitPts[splitPtIdx];
                        if (res[i].SegmentContainsPoint(tolLen, spnt, excludeExtreme: true))
                        {
                            repl = new List<Line3D>();
                            for (int h = 0; h < res.Count; ++h)
                            {
                                if (h == i)
                                {
                                    var l = res[h];
                                    repl.Add(new Line3D(l.From, spnt));
                                    repl.Add(new Line3D(spnt, l.To));
                                }
                                else
                                    repl.Add(res[h]);
                            }

                            break; // break cause need to reeval
                        }
                    }

                    if (repl != null)
                    {
                        res = repl;
                        continue;
                    }
                    else
                        splitPtIdx++;
                }

                return res;
            }

            /// <summary>
            /// if this segment from matches the given point returns this;
            /// if this segment to matches the given point return this with from,to swapped;
            /// precondition: this segment must have from or to equals given from
            /// </summary>            
            public Line3D EnsureFrom(double tolLen, Vector3D pt)
            {
                if (From.EqualsTol(tolLen, pt)) return this;
                if (To.EqualsTol(tolLen, pt)) return Reverse();
                throw new System.Exception($"not found valuable from-to in seg [{this}] that can satisfy from or to equals [{pt}]");
            }

            public string CadScript
            {
                get
                {
                    return string.Format(CultureInfo.InvariantCulture, "_LINE {0},{1},{2} {3},{4},{5}\r\n",
                        From.X, From.Y, From.Z, To.X, To.Y, To.Z);
                }
            }

            public override EntityObject DxfEntity
            {
                get
                {
                    return this.ToLine();
                }
            }

            /// <summary>
            /// hash string with given tolerance
            /// </summary>            
            public string ToString(double tolLen)
            {
                var pts_en = DisambiguatedPoints.GetEnumerator();

                var res = "";

                while (pts_en.MoveNext())
                {
                    if (res.Length > 0) res += "_";

                    res += pts_en.Current.ToString(tolLen);
                }

                return res;
            }

            /// <summary>
            /// build an invariant string representation w/3 digits
            /// (f.x, f.y, f.z)-(t.x, t.y, t.z) L=len Δ=(v.x, v.y, v.z)
            /// </summary>            
            public override string ToString()
            {
                return this.ToString(digits: 3);
            }

            /// <summary>
            /// build an invariant string representation w/given digits
            /// (f.x, f.y, f.z)-(t.x, t.y, t.z) L=len Δ=(v.x, v.y, v.z)
            /// </summary>      
            public string ToString(int digits = 3)
            {
                return $"{From.ToString(digits)}-{To.ToString(digits)} L={Length.ToString(digits)} Δ={(To - From).ToString(digits)}";
            }

            public override IEnumerable<Vector3D> Divide(int cnt, bool include_endpoints = false)
            {
                var step = Length / cnt * V.Normalized();
                var p = GeomFrom;
                if (include_endpoints) yield return p;
                --cnt;
                while (cnt > 0)
                {
                    p = p + step;
                    yield return p;
                    --cnt;
                }
                if (include_endpoints) yield return GeomTo;
            }

            /// <summary>
            /// build a segment with same from and vector normalized
            /// </summary>            
            public Line3D Normalized()
            {
                return new Line3D(From, V.Normalized(), Line3DConstructMode.PointAndVector);
            }

           

            /// <summary>
            /// return segment with swapped from,to
            /// </summary>
            public Line3D Swapped
            {
                get
                {
                    return new Line3D(To, From);
                }
            }

            /// <summary>
            /// return inverted segment
            /// </summary>
            public Line3D Inverted
            {
                get
                {
                    return new Line3D(From, -V, Line3DConstructMode.PointAndVector);
                }
            }

            public override BBox3D BBox(double tol_len, double tol_rad)
            {
                return new BBox3D(new[] { From, To });
            }

            /// <summary>
            /// returns bisect of two given segment/lines
            /// ( if given segment not share nodes but intesects returned bisect start from ip )
            /// bisect choosen will be the one between this and other withing shortest angle
            /// 
            /// if two given lines are parallel and parallelRotationAxis is given then
            /// bisect results as this segment rotated PI/2 about given axis using To as rotcenter
            /// </summary>            
            public Line3D Bisect(double tol_len, Line3D other, Vector3D parallelRotationAxis = null)
            {
                if (V.IsParallelTo(tol_len, other.V))
                {
                    if (parallelRotationAxis == null) return null;

                    var p = From;

                    if (To.EqualsTol(tol_len, other.From) || To.EqualsTol(tol_len, other.To))
                        p = To;

                    return new Line3D(p, V.RotateAboutAxis(parallelRotationAxis, PI / 2), Line3DConstructMode.PointAndVector);
                }

                var ip = this.Intersect(tol_len, other);
                if (ip == null) return null;

                var k = From.EqualsTol(tol_len, ip) ? To : From;
                var k2 = other.From.EqualsTol(tol_len, ip) ? other.To : other.From;

                var c = (k - ip).RotateAs(tol_len, (k - ip), (k2 - ip), angleFactor: .5);

                return new Line3D(ip, c, Line3DConstructMode.PointAndVector);
            }

        }

        public class Line3DEqualityComparer : IEqualityComparer<Line3D>
        {
            double tol;            

            public Line3DEqualityComparer(double _tol)
            {
                tol = _tol;                
            }

            public bool Equals(Line3D x, Line3D y)
            {
                return x.EqualsTol(tol, y);
            }

            public int GetHashCode(Line3D obj)
            {
                return 0;
            }

        }

    }

    public static partial class Extensions
    {

        public static string ToCadScript(this IEnumerable<Line3D> lines)
        {
            var sb = new StringBuilder();

            foreach (var l in lines)
            {
                sb.AppendLine(l.CadScript);
            }

            return sb.ToString();
        }

        public static Line3D ToLine3D(this netDxf.Entities.Line line)
        {
            return new Line3D(line.StartPoint, line.EndPoint);
        }

        /// <summary>
        /// retrieve s[0].from, s[1].from, ... s[n-1].from, s[n-1].to points
        /// </summary>        
        public static IEnumerable<Vector3D> PolyPoints(this IEnumerable<Line3D> segs)
        {
            var en = segs.GetEnumerator();

            Line3D seg = null;

            while (en.MoveNext())
            {
                seg = en.Current;
                yield return seg.From;
            }

            yield return seg.To;
        }

        /// <summary>
        /// merge colinear overlapped segments into single
        /// result segments direction and order is not ensured
        /// pre: segs must colinear
        /// </summary>        
        public static IEnumerable<Line3D> MergeColinearSegments(this IEnumerable<Line3D> _segs, double tol_len)
        {
            var segs = new List<Line3D>(_segs);

            bool found_overlaps;
            do
            {
                var to_remove = new List<Line3D>();
                var to_add = new List<Line3D>();
                found_overlaps = false;

                for (int i = 0; !found_overlaps && i < segs.Count; ++i)
                {
                    for (int j = 0; j < segs.Count; ++j)
                    {
                        if (i == j) continue;

                        var i_contains_j_from = segs[i].SegmentContainsPoint(tol_len, segs[j].From);
                        var i_contains_j_to = segs[i].SegmentContainsPoint(tol_len, segs[j].To);

                        if (!segs[i].Colinear(tol_len, segs[j])) continue;

                        // i contains j entirely
                        if (i_contains_j_from && i_contains_j_to)
                        {
                            to_remove.Add(segs[j]);
                            found_overlaps = true;
                            break;
                        }

                        // i contains only j from but not j to
                        if (i_contains_j_from)
                        {
                            to_remove.Add(segs[i]);
                            to_remove.Add(segs[j]);
                            if (segs[i].V.Concordant(tol_len, segs[j].V))
                                to_add.Add(new Line3D(segs[i].From, segs[j].To));
                            else
                                to_add.Add(new Line3D(segs[i].To, segs[j].To));

                            found_overlaps = true;
                            break;
                        }

                        // i contains only j to but not j from
                        if (i_contains_j_to)
                        {
                            to_remove.Add(segs[i]);
                            to_remove.Add(segs[j]);
                            if (segs[i].V.Concordant(tol_len, segs[j].V))
                                to_add.Add(new Line3D(segs[j].From, segs[i].To));
                            else
                                to_add.Add(new Line3D(segs[i].From, segs[j].From));

                            found_overlaps = true;
                            break;
                        }
                    }
                }

                to_remove.ForEach(w => segs.Remove(w));
                segs.AddRange(to_add);
            }
            while (found_overlaps);

            return segs;
        }

        /// <summary>
        /// autointersect given list of segments
        /// ( duplicates and overlapping are removed )
        /// TODO : dummy function, optimize
        /// </summary>       
        public static IReadOnlyList<Line3D> AutoIntersect(this IReadOnlyList<Line3D> segs, double tolLen,
        bool mergeColinearSegments = true, IEnumerable<Vector3D> addictionalSplitPoints = null)
        {
            segs = segs.MergeColinearSegments(tolLen).ToList();

            var segCmp = new Line3DEqualityComparer(tolLen);
            var vecCmp = new Vector3DEqualityComparer(tolLen);

            // line_hs -> split points
            var splitPts = new Dictionary<Line3D, HashSet<Vector3D>>(segCmp);

            // fill splitPts dictionary with list of segments split points
            for (int i = 0; i < segs.Count; ++i)
            {
                for (int j = 0; j < segs.Count; ++j)
                {
                    if (i == j) continue;

                    var seg_i = segs[i];
                    var seg_j = segs[j];

                    var q = seg_i.Intersect(tolLen, seg_j, true, true);
                    if (q != null)
                    {
                        HashSet<Vector3D> i_hs = null;
                        HashSet<Vector3D> j_hs = null;

                        if (!q.EqualsTol(tolLen, seg_i.From) && !q.EqualsTol(tolLen, seg_i.To))
                        {
                            if (!splitPts.TryGetValue(seg_i, out i_hs))
                            {
                                i_hs = new HashSet<Vector3D>(vecCmp);
                                splitPts.Add(seg_i, i_hs);
                            }
                            i_hs.Add(q);
                        }

                        if (!q.EqualsTol(tolLen, seg_j.From) && !q.EqualsTol(tolLen, seg_j.To))
                        {
                            if (!splitPts.TryGetValue(seg_j, out j_hs))
                            {
                                j_hs = new HashSet<Vector3D>(vecCmp);
                                splitPts.Add(seg_j, j_hs);
                            }
                            j_hs.Add(q);
                        }
                    }
                }
            }

            // process addictional split points
            if (addictionalSplitPoints != null)
            {
                foreach (var pt in addictionalSplitPoints)
                {
                    foreach (var seg in segs)
                    {
                        if (seg.SegmentContainsPoint(tolLen, pt, excludeExtreme: true))
                        {
                            HashSet<Vector3D> hs = null;
                            if (!splitPts.TryGetValue(seg, out hs))
                            {
                                hs = new HashSet<Vector3D>(vecCmp);
                                splitPts.Add(seg, hs);
                            }
                            hs.Add(pt);
                        }
                    }
                }
            }

            // split segment by split points and rebuild res list
            if (splitPts.Count > 0)
            {
                HashSet<Vector3D> qSplitPts = null;
                var res = new List<Line3D>();
                for (int i = 0; i < segs.Count; ++i)
                {
                    if (splitPts.TryGetValue(segs[i], out qSplitPts))
                        res.AddRange(segs[i].Split(tolLen, qSplitPts.ToList()));
                    else
                        res.Add(segs[i]);
                }
                segs = res;
            }

            return segs;
        }

    }

}
