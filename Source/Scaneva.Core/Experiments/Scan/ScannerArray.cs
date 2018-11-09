#region Copyright (C)
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
using System.Threading;
using Scaneva.Core.Experiments.ScanEva;
using Scaneva.Tools;

namespace Scaneva.Core.Experiments
{

    public class ScannerArray : IScanner
    {
        ScanArraySettings.ScanMode Mode = ScanArraySettings.ScanMode.Comb;
        Position mStartPosition = new Position();     //Startpos. in abs. Koordinaten
        Position mEndPosition = new Position();       // Endpos. in abs. Koordinaten

        Position mSpeeds = new Position();
        Position mReverseSpeeds = new Position();
        Position mIncrements = new Position();
        Position mLengths = new Position();

        Position mPreMove = new Position();
        Position mPostMove = new Position();
        IPositioner mPositioner; //Positioner for scan
        LogHelper log = null;

        int[] mScanPointIndex = new int[] { 0, 0, 0 };
        int[] mScanPoints = new int[] { 0, 0, 0 };

        private bool bNewRow = false;
        private bool bNewSlice = false;

        long XDelay, YDelay, ZDelay;
        TiltCorrection mTilt;
        enuScannerErrors mStatus = 0;

        public int[] ScanPointIndex { get => mScanPointIndex; }
        public int[] NumScanPoints { get => mScanPoints; }

        public ScannerArray(ScanArraySettings.ScanMode _mode, IPositioner _pos, Position _lengths, Position _increments,
            Position _speeds, Position _revspeeds, Position _premove, Position _postmove, TiltCorrection _tilt, long _xDelay, long _yDelay, long _zDelay, LogHelper log)
        {
            this.log = log;
            Mode = _mode;
            mPositioner = _pos;
            mLengths = _lengths;
            mIncrements = _increments;
            mSpeeds = _speeds;
            mReverseSpeeds = _revspeeds;
            mPreMove = _premove;
            mPostMove = _postmove;
            mTilt = _tilt;
            XDelay = _xDelay;
            YDelay = _yDelay;
            ZDelay = _zDelay;

            mScanPoints[0] = (_lengths.X != 0) ? ((int)Math.Floor(Math.Abs(_lengths.X / _increments.X)) + 1) : 1;
            mScanPoints[1] = (_lengths.Y != 0) ? ((int)Math.Floor(Math.Abs(_lengths.Y / _increments.Y)) + 1) : 1;
            mScanPoints[2] = (_lengths.Z != 0) ? ((int)Math.Floor(Math.Abs(_lengths.Z / _increments.Z)) + 1) : 1;

            mScanPointIndex = new int[] { 0, 0, 0 };
        }

        public enuScannerErrors Initialize()
        {
            mStatus = 0;
            mScanPointIndex = new int[] { 0, 0, 0 };

            //todo: check parameters, reflect correctness in the status flag
            //store current absolute position as starting position and calculate end position
            mStartPosition = mPositioner.AbsolutePosition();
            mEndPosition = mStartPosition.Sum(mLengths);
            if (mPositioner.ValidatePosition(ref mEndPosition) == enuPositionerStatus.Ready)
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
            if (mPositioner.RelativePosition(mPreMove) != enuPositionerStatus.Ready)
            {
                mStatus = enuScannerErrors.Error;
                return mStatus;
            }

            Position oldpos = new Position();
            oldpos = mPositioner.AbsolutePosition();  //this is the current absolute position
            Position Dest = CalculateNextAbsolutePosition(oldpos);// here we get a new absolute position to go as next, also the delay is considered there
            if (Dest == null)
            {//no next position, scan finished
                mStatus |= enuScannerErrors.Finished;
                return mStatus;
            }

            if (mTilt != null)
            {//consider the tilt corrected Z-height if tilt is set
                Dest.Z = mTilt.CalculateZ(Dest);
            }

            // and move to the next position
            // if Scan Mode is Comb we do the X-Movement first
            if (Mode == ScanArraySettings.ScanMode.Comb)
            {
                if (mPositioner.MoveAbsolut(enuAxes.XAxis, Dest.X, mPositioner.Speed(enuAxes.XAxis)) != enuPositionerStatus.Ready)
                {
                    mStatus = enuScannerErrors.Error;
                    return mStatus;
                }
            }
            // move to dest position
            if (mPositioner.AbsolutePosition(Dest) != enuPositionerStatus.Ready)
            {
                mStatus = enuScannerErrors.Error;
                return mStatus;
            }

            log.AddStatusUpdate(0, Dest);

            //postmovement hook: usually move down to surface to reduce travel distance for FBC.
            //The hook is less, then the premovement. Not really meaningful while using tilt correction
            if (mPositioner.RelativePosition(mPostMove) != enuPositionerStatus.Ready)
            {
                mStatus = enuScannerErrors.Error;
                return mStatus;
            }

            // All movements are done => now do the apropriate delay
            Thread.Sleep((int)XDelay);  // Allways do the X Delay
            if (bNewRow)
            {
                Thread.Sleep((int)YDelay); // On each new line add the Y Delay
            }
            if (bNewSlice)
            {
                Thread.Sleep((int)ZDelay); // On each new plane add the Z Delay
            }

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
            log.AddStatusUpdate(0, Dest);
            //before the next scan a call to Prepare() is enforced by this. This ensures that the start
            //position is set properly for each scan.
            mStatus = enuScannerErrors.NotInitialized;
            return mStatus;
        }
        
        //returns the new position in absolute coordinates and global scope.
        //expects pos to be the current absolute position (global scope).
        Position CalculateNextAbsolutePosition(Position _pos)
        {
            bNewRow = false;
            bNewSlice = false;

            switch (Mode)
            {
                case ScanArraySettings.ScanMode.Comb:
                case ScanArraySettings.ScanMode.Saw:
                    // We only keep the Z-component of the current position (if it is not a z- scan)

                    Position cdest = _pos.Copy;
                    // Increment Indices
                    mScanPointIndex[0]++;

                    //todo: integrate delays
                    // end of a Row?
                    if (mScanPointIndex[0] >= mScanPoints[0])
                    {
                        mScanPointIndex[0] = 0;
                        mScanPointIndex[1]++;
                        bNewRow = true;

                        // end of slice?
                        if (mScanPointIndex[1] >= mScanPoints[1])
                        {
                            mScanPointIndex[1] = 0;
                            mScanPointIndex[2]++;
                            bNewSlice = true;
                        }
                    }

                    // is the scan finished?
                    if ((mScanPointIndex[2] >= mScanPoints[2]))
                    {
                        return null;
                    }

                    cdest.X = mStartPosition.X + mScanPointIndex[0] * mIncrements.X;
                    cdest.Y = mStartPosition.Y + mScanPointIndex[1] * mIncrements.Y;

                    // For z-scan ignore current z-Position and calculate
                    if (mScanPoints[2] > 1)
                    {
                        cdest.Z = cdest.Z + mScanPointIndex[2] * mIncrements.Z;
                    }

                    return cdest;

                //finish, return sum of relativ pos and scan start position
                //validation of the position is the responsibility of the caller.
                case ScanArraySettings.ScanMode.Meander:
                    // We only keep the Z-component of the current position (if it is not a z- scan)
                    Position mdest = _pos.Copy;

                    if (mScanPointIndex[1] % 2 == 0) // even line: 0, 2, 4, 6... then we scan from left to right
                    {
                        mScanPointIndex[0]++;
                        // end of a Row?
                        if (mScanPointIndex[0] >= mScanPoints[0])
                        {
                            // we scan the next line from right to left, thus we start at mScanPoints[0] - 1
                            mScanPointIndex[0] = mScanPoints[0] - 1;
                            mScanPointIndex[1]++;
                            bNewRow = true;
                        }
                    }
                    else
                    {// odd line: 1, 3, 5...  then we scan from right to left 
                        mScanPointIndex[0]--;
                        // end of a Row?
                        if (mScanPointIndex[0] < 0)
                        {
                            // we scan the next line from left to right
                            mScanPointIndex[0] = 0;
                            mScanPointIndex[1]++;
                            bNewRow = true;
                        }
                    }

                    // end of a slice?
                    if (mScanPointIndex[1] >= mScanPoints[1])
                    {
                        mScanPointIndex[1] = 0;
                        mScanPointIndex[2]++;
                        bNewSlice = true;
                    }

                    // is the scan finished?
                    if ((mScanPointIndex[2] >= mScanPoints[2]))
                    {
                        return null;
                    }

                    mdest.X = mStartPosition.X + mScanPointIndex[0] * mIncrements.X;
                    mdest.Y = mStartPosition.Y + mScanPointIndex[1] * mIncrements.Y;

                    // For z-scan ignore current z-Position and calculate
                    if (mScanPoints[2] > 1)
                    {
                        mdest.Z = mdest.Z + mScanPointIndex[2] * mIncrements.Z;
                    }

                    return mdest;
                //finish, return sum of relativ pos and scan start position
                //validation of the position is the responsibility of the caller.

                default:
                    return null;
            }
        }

        public Position Position()
        {// Position relativ zur Startposition
            return mPositioner.AbsolutePosition().Delta(mStartPosition);
        }

    }
}