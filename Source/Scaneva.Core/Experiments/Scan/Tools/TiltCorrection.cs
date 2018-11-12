#region Copyright (C)
// ---------------------------------------------------------------------------------------------------------------
//  <copyright file="TiltCorrection.cs" company="Scaneva">
// 
//  Copyright (C) 2018 Roche Diabetes Care GmbH (Christoph Pieper)
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
using Scaneva.Tools;

namespace Scaneva.Core
{
    public class TiltCorrection
    {
        IPositioner mPositioner; //Positioner for tilt compensation

        // 3 points in the plane
        private Position F;
        private Position G;
        private Position H;

        // components of the final coordinate form nx*x + ny*y + nz*z + d = 0 are the components of
        // the normal vector and a constant d (see below)
        private double d;
        private double offset;
        private Position n = new Position();
        public PositionStore PositionStore { get; set; }

        public TiltCorrection(IPositioner _pos, Position _Pos1, Position _Pos2, Position _Pos3, double _Offset)
        {
            mPositioner = _pos;
            F = _Pos1.Copy;
            G = _Pos2.Copy;
            H = _Pos3.Copy;
            offset = _Offset;
            ReCalculatePlane();
        }

        public void ReCalculatePlane()
        {
            Position fg = new Position();
            Position fh = new Position();

            //calculate 2 vectors in the plane (from F to G and F to H).
            fg.X = G.X - F.X;
            fg.Y = G.Y - F.Y;
            fg.Z = G.Z - F.Z;
            fh.X = H.X - F.X;
            fh.Y = H.Y - F.Y;
            fh.Z = H.Z - F.Z;

            //calculate a normal vector by using the 'Kreuzprodukt'
            n.X = (fg.Y * fh.Z) - (fg.Z * fh.Y);
            n.Y = (fg.Z * fh.X) - (fg.X * fh.Z);
            n.Z = (fg.X * fh.Y) - (fg.Y * fh.X);
            
            // objLogger.LogEvent(Me, "ReCalculatePlane in clsPlane: N = " & n.HumanReadable)
            d = -n.X * H.X - n.Y * H.Y - n.Z * H.Z;
            //objLogger.LogEvent(Me, "ReCalculatePlane in clsPlane: d = " & d)
        }

        //gets a position and calculates the change in Z component in a way, that the p.Z + change is
        //in the plane parallel to the internal plane with offset offset.
        public double CalculateDeltaZ(Position _p)
        {
            return CalculateZ(_p) - _p.Z;
        }

        //gets a position and calculates its Z component in a way, that the returned position is
        //in the plane parallel to the internal plane with the offset "offset".
        public double CalculateZ(Position _p)
        {
            double tx;
            double ty;
            double tz;

            tx = _p.X;
            ty = _p.Y;

            //from the 'Koordinatenform' results for the Z component:
            // z = (-d - nx*x - ny*y)/nz
            if (n.Z != 0)
            {
                tz = (-d - n.X * tx - n.Y * ty) / n.Z;
                //and add the offset to get a plane parallel to the plane stored in this object
                tz = tz + offset;
                return tz;
            }
            else return 0;
            //TODO: log error
        }

        public enuPositionerStatus CompensateTilt()
        {
            Position pos = new Position();
            //calculate the necessary change in Z and store it suitable for the positioner
            Position apos = PositionStore.CurrentAbsolutePosition();
            pos.Z = CalculateDeltaZ(apos);

            //perform Z-axis re-positioneng
            if (mPositioner.SetRelativePosition(pos) != enuPositionerStatus.Ready)
            {
                //log error, stop scan
                return enuPositionerStatus.Error;
            }
            return enuPositionerStatus.Ready;
        }
    }
}

