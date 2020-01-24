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

using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows.Data;
using System.Globalization;
using SearchAThing.Sci;

namespace SearchAThing
{

    namespace Sci
    {

        [DataContract]
        public class MeasureUnit : IEquatable<MeasureUnit>
        {
            /// <summary>
            /// all measure units for any physical quantity
            /// this list is used to avoid double registration of a measure unit with same name
            /// for a given physical quantity
            /// </summary>
            [BsonIgnore]
            [DataMember]
            static List<MeasureUnit> AllMeasureUnits = new List<MeasureUnit>();

            static Dictionary<int, int> global_static_id_counter = new Dictionary<int, int>();

            [BsonIgnore]
            [DataMember]
            internal int id;

            [DataMember]
            public string Name { get; private set; }

            [DataMember]
            public PhysicalQuantity PhysicalQuantity { get; private set; }

            void Init(PhysicalQuantity physicalQuantity, string name, MeasureUnit convRefUnit = null)
            {
                PhysicalQuantity = physicalQuantity;

                if (AllMeasureUnits
                    .Where(r => r.PhysicalQuantity.id == physicalQuantity.id)
                    .Any(r => r.Name == name))
                    throw new Exception($"A registered measure unit [{name}] already exists for the physical quantity [{physicalQuantity.Name}]");

                if (physicalQuantity.MUConversionType == MeasureUnitConversionTypeEnum.Linear &&
                    convRefUnit == null && physicalQuantity.LinearConversionRefMU != null)
                    throw new Exception(
                        $"A reference measure unit [{physicalQuantity.LinearConversionRefMU}] already exists for the physical quantity [{physicalQuantity.Name}]" +
                        $"Need to specify a valid existing convRefUnit with related convRefFactor to specify measure unit scale factor");

                if (global_static_id_counter.ContainsKey(physicalQuantity.id))
                    id = ++global_static_id_counter[physicalQuantity.id];
                else
                    global_static_id_counter.Add(physicalQuantity.id, id = 0);

                Name = name;
                PhysicalQuantity = PhysicalQuantity;
            }

            public MeasureUnit(PhysicalQuantity physicalQuantity, string name, MeasureUnit convRefUnit = null, double convRefFactor = 0)
            {
                Init(physicalQuantity, name, convRefUnit);

                physicalQuantity.RegisterMeasureUnit(this, convRefUnit, convRefFactor);
            }

            public MeasureUnit(PhysicalQuantity physicalQuantity, string name, Func<MeasureUnit, MeasureUnit, double, double> convRefFunctor)
            {
                Init(physicalQuantity, name);

                physicalQuantity.RegisterMeasureUnit(this, convRefFunctor);
            }

            /// <summary>
            /// Builds a Measure object of value * given mu
            /// </summary>        
            public static Measure operator *(double value, MeasureUnit mu)
            {
                return new Measure(value, mu);
            }

            public static Measure operator *(double? value, MeasureUnit mu)
            {
                if (value.HasValue) return value.Value * mu;

                return null;
            }

            /// <summary>
            /// retrieve correspondent measure unit ( same physical quantity ) from given domain
            /// </summary>            
            public MeasureUnit Related(MUDomain mud)
            {
                if (PhysicalQuantity.id == PQCollection.Adimensional.id) return MUCollection.Adimensional.adim;

                return mud.ByPhysicalQuantity(PhysicalQuantity).MU;
            }

            public override string ToString()
            {
                return Name;
            }           

            public bool Equals(MeasureUnit other)
            {
                return id == other.id;
            }

            public double Tolerance(IModel model)
            {
                var mudomain = model.MUDomain.ByPhysicalQuantity(PhysicalQuantity);

                if (mudomain.MU.id == id)
                    return mudomain.DefaultTolerance;
                else
                    return mudomain.ConvertTo(this).DefaultTolerance;
            }

        };

    }

    public static partial class Extensions
    {

        /// <summary>
        /// convert nullable double from to measure units
        /// </summary>        
        public static double? Convert(this double? value, MeasureUnit from, MeasureUnit to)
        {
            if (!value.HasValue) return null;

            return value.Value.Convert(from, to);
        }

        /// <summary>
        /// convert nullable double from to measure units
        /// </summary>        
        public static double? Convert(this double? value, IMUDomain mud, MeasureUnit to)
        {
            if (!value.HasValue) return null;

            return value.Value.Convert(mud, to);
        }

        /// <summary>
        /// convert nullable double from to measure units
        /// </summary>        
        public static double? Convert(this double? value, MeasureUnit from, IMUDomain mud)
        {
            if (!value.HasValue) return null;

            return value.Value.Convert(from, mud);
        }

        /// <summary>
        /// convert given value from to measure units
        /// </summary>        
        public static double Convert(this double value, MeasureUnit from, MeasureUnit to)
        {
            return Measure.Convert(value, from, to);
        }

        /// <summary>
        /// convert given value from to measure units
        /// to measure unit is given from the correspondent physical quantity measure unit in the given domain
        /// </summary>        
        public static double Convert(this double value, MeasureUnit from, IMUDomain mud)
        {
            return Measure.Convert(value, from, mud);
        }

        /// <summary>
        /// convert given value from to measure units
        /// from measure unit is given from the correspondent physical quantity measure unit in the given domain
        /// </summary>        
        public static double Convert(this double value, IMUDomain from, MeasureUnit to)
        {
            return Measure.Convert(value, from, to);
        }

        /// <summary>
        /// convert given value from the given measure unit in the domain corresponding to the physical quantity of given to
        /// and build a measure with given to measure unit
        /// </summary>        
        public static Measure ConvertToMeasure(this double value, IMUDomain from, MeasureUnit to)
        {
            return value.Convert(from, to) * to;
        }

    }

}
