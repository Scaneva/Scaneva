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

        public Position()
        {

        }

        public Position(double x, double y, double z, double a = 0)
        {
            X = x;
            Y = y;
            Z = z;
            A = a;
        }

        [DisplayName("X")]
        public double X { get; set; }
        [DisplayName("Y")]
        public double Y { get; set; }
        [DisplayName("Z")]
        public double Z { get; set; }
        [DisplayName("A")]
        public double A { get; set; }

        [Browsable(false)]
        [XmlIgnore]
        public Position Copy
        //returns a copy of the object
        {
            get
            {
                Position c = new Position();
                c.X = X;
                c.Y = Y;
                c.Z = Z;
                c.A = A;
                return c;
            }
        }

        [Browsable(false)]
          public Position Delta(Position _delta)
        {
            Position d = new Position();
            d.X = X - _delta.X;
            d.Y = Y - _delta.Y;
            d.Z = Z - _delta.Z;
            d.A = A - _delta.A;
            return d;
        }

        [Browsable(false)]
        public Position Sum(Position _sum)
        //calculates the sum of this and Summand
        {
            Position s = new Position();
            s.X = X + _sum.X;
            s.Y = Y + _sum.Y;
            s.Z = Z + _sum.Z;
            s.A = A + _sum.A;
            return s;
        }

        [Browsable(false)]
        [XmlIgnore]
        public string Serialize
        //Serialize the position into a string
        {
            get
            {
                string[] arr = new string[4];
                //todo: check format options
                arr[0] = Convert.ToString(this.X);
                arr[1] = Convert.ToString(this.Y);
                arr[2] = Convert.ToString(this.Z);
                arr[3] = Convert.ToString(this.A);
                return "(" + String.Join(";", arr) + ")";
            }

            set
            {
                if (value != "")
                {
                    //todo: remove brackets!!
                    string[] arr = value.Split(';');
                    double.TryParse(arr[0], out double _X);//todo: implement error check and handling
                    double.TryParse(arr[1], out double _Y);
                    double.TryParse(arr[2], out double _Z);
                    double.TryParse(arr[3], out double _A);
                    X = _X;
                    Y = _Y;
                    Z = _Z;
                    A = _A;
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
            string[] arr = new string[4];
            arr[0] = "X: " + Convert.ToString(Math.Round(X, 2)) + " µm";
            arr[1] = "Y: " + Convert.ToString(Math.Round(Y, 2)) + " µm";
            arr[2] = "Z: " + Convert.ToString(Math.Round(Z, 2)) + " µm";
            arr[3] = "A: " + Convert.ToString(Math.Round(A, 2)) + " µm";
            return String.Join(", ", arr);
        }

        public override string ToString()
        {
            return Serialize;
        }
    }
}
