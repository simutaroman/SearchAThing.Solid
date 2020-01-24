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
using System.Linq;

namespace SearchAThing.Sci
{

    // https://en.wikipedia.org/wiki/List_of_physical_quantities

    public static class PQCollection
    {

        //-------------------------------------------------------------------
        // Physical Quantities
        //-------------------------------------------------------------------

        static Dictionary<string, PhysicalQuantity> _dict_pq;
        static Dictionary<string, PhysicalQuantity> dict_pq
        {
            get
            {
                if (_dict_pq == null) _dict_pq = PhysicalQuantities.ToDictionary(k => k.Name, v => v);
                return _dict_pq;
            }
        }
        public static PhysicalQuantity ByName(string pq_name)
        {
            PhysicalQuantity pq = null;

            dict_pq.TryGetValue(pq_name, out pq);

            return pq;
        }

        static List<PhysicalQuantity> physicalQuantities;
        public static IEnumerable<PhysicalQuantity> PhysicalQuantities
        {
            get
            {
                if (physicalQuantities == null)
                {
                    physicalQuantities = new List<PhysicalQuantity>();

                    physicalQuantities.Add(Adimensional);
                    physicalQuantities.Add(Frequency);

                    physicalQuantities.Add(Length);
                    physicalQuantities.Add(Mass);
                    physicalQuantities.Add(Time);
                    physicalQuantities.Add(ElectricCurrent);
                    physicalQuantities.Add(Temperature);
                    physicalQuantities.Add(AmountOfSubstance);
                    physicalQuantities.Add(LuminousIntensity);

                    //-------------------------------------------------------

                    physicalQuantities.Add(Length2);
                    physicalQuantities.Add(Length3);
                    physicalQuantities.Add(PlaneAngle);
                    physicalQuantities.Add(Pressure);
                    physicalQuantities.Add(Acceleration);
                    physicalQuantities.Add(Force);
                    physicalQuantities.Add(Speed);
                    physicalQuantities.Add(BendingMoment);
                    physicalQuantities.Add(Energy);
                    physicalQuantities.Add(ElectricalConductance);
                    physicalQuantities.Add(ElectricalConductivity);
                    physicalQuantities.Add(Power);
                    physicalQuantities.Add(Turbidity);
                    physicalQuantities.Add(VolumetricFlowRate);
                }
                return physicalQuantities;
            }
        }

        //-------------------------------------------------------------------
        // Base quantity
        //-------------------------------------------------------------------

        #region Base quantity        

        public static readonly PhysicalQuantity Adimensional = new PhysicalQuantity("Adimensional", typeof(MUCollection.Adimensional));

        public static readonly PhysicalQuantity Frequency = new PhysicalQuantity("Frequency", typeof(MUCollection.Frequency));

        // https://en.wikipedia.org/wiki/List_of_physical_quantities

        public static readonly PhysicalQuantity Length = new PhysicalQuantity("Length", typeof(MUCollection.Length));

        public static readonly PhysicalQuantity Mass = new PhysicalQuantity("Mass", typeof(MUCollection.Mass));

        public static readonly PhysicalQuantity Time = new PhysicalQuantity("Time", typeof(MUCollection.Time));

        public static readonly PhysicalQuantity ElectricCurrent = new PhysicalQuantity("ElectricCurrent", typeof(MUCollection.ElectricCurrent));

        public static readonly PhysicalQuantity Temperature = new PhysicalQuantity("Temperature",
            typeof(MUCollection.Temperature), MeasureUnitConversionTypeEnum.NonLinear);

        public static readonly PhysicalQuantity AmountOfSubstance = new PhysicalQuantity("AmountOfSubstance", typeof(MUCollection.AmountOfSubstance));

        public static readonly PhysicalQuantity LuminousIntensity = new PhysicalQuantity("LuminousIntensity", typeof(MUCollection.LuminousIntensity));

        #endregion

        //-------------------------------------------------------------------
        // Derived quantity
        //-------------------------------------------------------------------

        #region Derived quantity

        public static readonly PhysicalQuantity PlaneAngle = new PhysicalQuantity("PlaneAngle", typeof(MUCollection.PlaneAngle));

        // solidAngle

        // absorbedDoseRate

        public static readonly PhysicalQuantity Acceleration = new PhysicalQuantity("Acceleration", typeof(MUCollection.Acceleration));

        public static readonly PhysicalQuantity Turbidity = new PhysicalQuantity("Turbidity", typeof(MUCollection.Turbidity));

        // angularAcceleration

        /*
        static PhysicalQuantity angularSpeed;
        public static PhysicalQuantity AngularSpeed
        {
            get
            {
                if (angularSpeed == null) angularSpeed = new PhysicalQuantity("AngularSpeed");

                return angularSpeed;
            }
        }
        */

        // angularMomentum

        // area
        public static readonly PhysicalQuantity Length2 = new PhysicalQuantity("Length2", typeof(MUCollection.Length2));

        // areaDensity

        // capacitance

        // catalyticActivity

        // catalyticActivityConcentration

        // checmicalPotential

        // molarConcentration

        // crackle

        // currentDensity

        // doseEquivalent

        // dynamicViscosity

        // electricCharge

        // electricChargeDensity

        // electricDisplacement

        // electricFieldStrength

        public static readonly PhysicalQuantity ElectricalConductance = new PhysicalQuantity("ElectricalConductance", typeof(MUCollection.ElectricalConductance));

        public static readonly PhysicalQuantity ElectricalConductivity = new PhysicalQuantity("ElectricalConductivity", typeof(MUCollection.ElectricalConductivity));

        // electicPotential

        // electicalResistance

        public static readonly PhysicalQuantity Energy = new PhysicalQuantity("Energy", typeof(MUCollection.Energy));

        // energyDensity

        // entropy

        public static readonly PhysicalQuantity Force = new PhysicalQuantity("Force", typeof(MUCollection.Force));

        // fuelEfficiency

        // impulse

        /*
        static PhysicalQuantity frequency;
        public static PhysicalQuantity Frequency
        {
            get
            {
                if (frequency == null) frequency = new PhysicalQuantity("Frequency");

                return frequency;
            }
        }
        */

        // halfLite

        // heat

        // heatCapacity

        // heatFluxDensity

        // illuminance

        // impedance

        // indexOfRefraction

        // inductance

        // irradiance

        // indensity

        // jerk

        // jounce

        /*
        static PhysicalQuantity linearDensity;
        public static PhysicalQuantity LinearDensity
        {
            get
            {
                if (linearDensity == null) linearDensity = new PhysicalQuantity("LinearDensity");

                return linearDensity;
            }
        }
        */

        // luminous flux

        // machNumber

        // magneticFieldStrength

        // magneticFlux

        // magneticFluxDensity

        // magnetization

        // massFraction

        /*
        static PhysicalQuantity massDensity;
        public static PhysicalQuantity MassDensity
        {
            get
            {
                if (massDensity == null) massDensity = new PhysicalQuantity("MassDensity");

                return massDensity;
            }
        }
        */

        // meanLifetime

        // molarEnergy

        // molarHeatCapacity

        /*
        static PhysicalQuantity momentOfIntertia;
        public static PhysicalQuantity MomentOfInertia
        {
            get
            {
                if (momentOfIntertia == null) momentOfIntertia = new PhysicalQuantity("MomentOfInertia");

                return momentOfIntertia;
            }
        }
        */

        /*
        static PhysicalQuantity momentum;
        public static PhysicalQuantity Momentum
        {
            get
            {
                if (momentum == null) momentum = new PhysicalQuantity("Momentum");

                return momentum;
            }
        }
        */

        // permeability

        // permittivity

        public static readonly PhysicalQuantity Power = new PhysicalQuantity("Power", typeof(MUCollection.Power));

        public static readonly PhysicalQuantity Pressure = new PhysicalQuantity("Pressure", typeof(MUCollection.Pressure));

        // pop

        // radioActivity

        // radioDose

        // radiance

        // radiantIntensity

        // reactionRrate

        // refractiveIndex

        public static readonly PhysicalQuantity Speed = new PhysicalQuantity("Speed", typeof(MUCollection.Speed));

        // specificEnergy

        // specificHeatCapacity

        // specificVolume

        // spin

        // strain        
        //Spublic static readonly PhysicalQuantity Strain = new PhysicalQuantity("Strain");

        /*
        static PhysicalQuantity stress;
        public static PhysicalQuantity Stress
        {
            get
            {
                if (stress == null) stress = new PhysicalQuantity("Stress");

                return stress;
            }
        }
        */

        // surfaceTension

        // thermalConductivity

        // torque

        public static readonly PhysicalQuantity BendingMoment = new PhysicalQuantity("BendingMoment", typeof(MUCollection.BendingMoment));

        // velocity

        public static readonly PhysicalQuantity Length3 = new PhysicalQuantity("Length3", typeof(MUCollection.Length3));

        public static readonly PhysicalQuantity VolumetricFlowRate = new PhysicalQuantity("VolumetricFlowRate", typeof(MUCollection.VolumetricFlowRate));

        /*
        static PhysicalQuantity volume;
        public static PhysicalQuantity Volume
        {
            get
            {
                if (volume == null) volume = new PhysicalQuantity("Volume");

                return volume;
            }
        }
        */

        // waveLength

        // waveNumber

        /*
        static PhysicalQuantity weight;
        public static PhysicalQuantity Weight
        {
            get
            {
                if (weight == null) weight = new PhysicalQuantity("Weight");

                return weight;
            }
        }
        */

        // work

        // youngModulus
        #endregion

    }

}
