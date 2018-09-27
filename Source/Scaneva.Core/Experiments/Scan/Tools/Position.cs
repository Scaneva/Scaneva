#region Copyright (C)
// ---------------------------------------------------------------------------------------------------------------
//  <copyright file="Position.cs" company="Scaneva">
// 
//  Copyright (C) 2018 Roche Diabetes Care GmbH (Kirill Sliozberg, Christoph Pieper)
// 
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program. If not, see http://www.gnu.org/licenses/.
//  </copyright>
//  <summary>
//  Url: https://github.com/Scaneva
//  </summary>
//  Inspiration from the following sources: 
//  http://www.ruhr-uni-bochum.de/elan/
// --------------------------------------------------------------------------------------------------------------------
#endregion

using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Scaneva.Core
{
    public class Position
    {
        //positions
        private double mdblX, mdblY, mdblZ;

        public Position()
        {

        }

        public Position(double x, double y, double z)
        {
            mdblX = x;
            mdblY = y;
            mdblZ = z;
        }

        [DisplayName("X")]
        public double X { get => mdblX; set => mdblX = value; }
        [DisplayName("Y")]
        public double Y { get => mdblY; set => mdblY = value; }
        [DisplayName("Z")]
        public double Z { get => mdblZ; set => mdblZ = value; }

        [Browsable(false)]
        [XmlIgnore]
        public Position Copy
        //returns a copy of the object
        {
            get
            {
                Position c = new Position();
                c.X = this.X;
                c.Y = this.Y;
                c.Z = this.Z;
                return c;
            }
        }

        [Browsable(false)]
          public Position Delta(Position _minuend)
        //calculates a position containing the differences between this and minuend
        {
            Position d = new Position();
            d.X = this.X - _minuend.X;
            d.Y = this.Y - _minuend.Y;
            d.Z = this.Z - _minuend.Z;
            return d;
        }

        [Browsable(false)]
        public Position Sum(Position _summand)
        //calculates the sum of this and Summand
        {
            Position s = new Position();
            s.X = this.X + _summand.X;
            s.Y = this.Y + _summand.Y;
            s.Z = this.Z + _summand.Z;
            return s;
        }

        [Browsable(false)]
        [XmlIgnore]
        public string Serialize
        //Serialize the position into a string
        {
            get
            {
                string[] arr = new string[3];
                //todo: check format options
                arr[0] = Convert.ToString(this.X);
                arr[1] = Convert.ToString(this.Y);
                arr[2] = Convert.ToString(this.Z);
                return "(" + String.Join(";", arr) + ")";
            }

            set
            {
                //string[] arr = new string[2];
                if (value != "")
                {
                    //todo: remove brackets!!
                    string[] arr = value.Split(';');
                    double.TryParse(arr[0], out double _X);//todo: implement error check and handling
                    double.TryParse(arr[1], out double _Y);
                    double.TryParse(arr[2], out double _Z);
                    this.X = _X;
                    this.Y = _Y;
                    this.Z = _Z;
                }
            }
        }

        [Browsable(false)]
        public string Printable()
        {
            return this.Serialize;
        }

        [Browsable(false)]
        public string HumanReadable()//[byte mode])
        {
            string[] arr = new string[3];
            arr[0] = "X: " + Convert.ToString(Math.Round(this.X, 2)) + " µm";
            arr[1] = "Y: " + Convert.ToString(Math.Round(this.Y, 2)) + " µm";
            arr[2] = "Z: " + Convert.ToString(Math.Round(this.Z, 2)) + " µm";
            return String.Join(", ", arr);
        }

        public override string ToString()
        {
            return Serialize;
        }
    }
}
