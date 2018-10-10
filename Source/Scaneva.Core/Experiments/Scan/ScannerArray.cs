﻿#region Copyright (C)
// ---------------------------------------------------------------------------------------------------------------
//  <copyright file="ScannerArray.cs" company="Scaneva">
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

namespace Scaneva.Core.Experiments
{
  
    public class ScannerArray: IScanner
    {
        Position mStartPosition = new Position();     //Startpos. in abs. Koordinaten
        Position mEndPosition = new Position();       // Endpos. in abs. Koordinaten

        Position mSpeeds = new Position();
        Position mReverseSpeeds = new Position();
        Position mIncrements = new Position();
        Position mLengths = new Position();

        Position mPreMove = new Position();
        Position mPostMove = new Position();
        IPositioner mPositioner; //Positioner for scan

        int[] mScanPointIndex = new int[] { 0, 0, 0 };
        int[] mScanPoints = new int[] { 0, 0, 0 };

        TiltCorrection mTilt;
        enuScannerErrors mStatus = 0;

        public ScannerArray(IPositioner _pos, Position _lengths, Position _increments,
            Position _speeds, Position _revspeeds, Position _premove, Position _postmove, TiltCorrection _tilt)
        {
            mPositioner = _pos;
            mLengths = _lengths;
            mIncrements = _increments;
            mSpeeds = _speeds;
            mReverseSpeeds = _revspeeds;
            mPreMove = _premove;
            mPostMove = _postmove;
            mTilt = _tilt;

            mScanPoints[0] = (int)Math.Floor(Math.Abs(_lengths.X / _increments.X)) + 1;
            mScanPoints[1] = (int)Math.Floor(Math.Abs(_lengths.Y / _increments.Y)) + 1;
            mScanPoints[2] = (int)Math.Floor(Math.Abs(_lengths.Z / _increments.Z)) + 1;
        }

        public enuScannerErrors Initialize()
        {
            mStatus = 0;
            mScanPointIndex = new int[] { 0, 0, 0 };

            //todo: check parameters, reflect correctness in the status flag
            //store current absolute position as starting position and calculate end position
            mStartPosition = mPositioner.AbsolutePosition();
            mEndPosition = mStartPosition.Sum(mLengths);
            if (mPositioner.ValidatePosition (ref mEndPosition) == enuPositionerStatus.Ready )
            {
                if (mPositioner.ValidateSpeeds(ref mSpeeds) == enuPositionerStatus.Ready)
                {
                    mStatus = enuScannerErrors.Initialized;
                    mStatus |= enuScannerErrors.Ready;
                } 
            }
            return mStatus;
        }

        public enuScannerErrors Reset()
        {
            mStatus = 0;
            return mStatus;
        }

        public enuScannerErrors NextPosition()
        {
            if (!mStatus.HasFlag(enuScannerErrors.Initialized))
            {
                //todo log error
                return mStatus;
            }

            //premovement hook: usually move away from surface to avoid damage of the tip
            mPositioner.RelativePosition(mPreMove); //todo: error check

            Position oldpos = new Position();
            oldpos = mPositioner.AbsolutePosition();  //this is the current absolute position
            Position Dest = CalculateNextAbsolutePosition(oldpos);// here we get a new absolute position to go as next
            if (Dest == null)
            {//no next position, scan finished
                mStatus |= enuScannerErrors.Finished;
                return mStatus;
            }

            if (mTilt != null)
            {//consider the tilt corrected Z-height if tilt is set
                Dest.Z = mTilt.CalculateZ(Dest);
            }
            //and move to the next position 
            mPositioner.AbsolutePosition(Dest);//todo: error check

            //postmovement hook: usually move down to surface to reduce travel distance for FBC.
            //The hook is less, then the premovement. Not really meaningful while using tilt correction
            mPositioner.RelativePosition(mPostMove); //todo: error check
            return mStatus;
        }

        public enuScannerErrors BackToStart()
        {
            if (!mStatus.HasFlag(enuScannerErrors.Initialized))
            {
                //todo log error
                return mStatus;
            }

            //just back to start position
            Position Dest = mStartPosition.Copy;

            //and move to the next position 
            mPositioner.AbsolutePosition(Dest);//todo: error check

            //before the next scan a call to Prepare() is enforced by this. This ensures that the start
            //position is set properly for each scan.
            mStatus = enuScannerErrors.NotInitialized;
            return mStatus;
        }

        //returns the new position in absolute coordinates and global scope.
        //expects pos to be the current absolute position (global scope).
        Position CalculateNextAbsolutePosition(Position _pos)
        {
            // We only keep the Z-component of the current position (if it is not a z- scan)
            Position dest = _pos.Copy;

            // Increment Indices
            mScanPointIndex[0]++;

            // end of Line?
            if (mScanPointIndex[0] >= mScanPoints[0])
            {
                mScanPointIndex[0] = 0;
                mScanPointIndex[1]++;

                // end of column?
                if (mScanPointIndex[1] >= mScanPoints[1])
                {
                    mScanPointIndex[1] = 0;
                    mScanPointIndex[2]++;
                }
            }

            // is the scan finished?
            if ((mScanPointIndex[2] >= mScanPoints[2]))
            {
                return null;
            }

            dest.X = mStartPosition.X + mScanPointIndex[0] * mIncrements.X;
            dest.Y = mStartPosition.Y + mScanPointIndex[1] * mIncrements.Y;

            // For z-scan ignore current z-Position and calculate
            if (mScanPoints[2] > 1)
            {
                dest.Z = dest.Z + mScanPointIndex[2] * mIncrements.Z;
            }

            return dest;
            //finish, return sum of relativ pos and scan start position
            //validation of the position is the responsibility of the caller.
        }

        public Position Position()
        {// Position relativ zur Startposition
            return mPositioner.AbsolutePosition().Delta(mStartPosition);
        }
    }
}