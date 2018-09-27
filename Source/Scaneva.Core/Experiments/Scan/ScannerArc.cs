#region Copyright (C)
// ---------------------------------------------------------------------------------------------------------------
//  <copyright file="ScannerArc.cs" company="Scaneva">
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
// --------------------------------------------------------------------------------------------------------------------
#endregion

using System;

namespace Scaneva.Core.Experiments
{
    public class ScannerArc : IScanner
    {
        Position mStartPosition = new Position();     //Startpos. in abs. Koordinaten
        Position mEndPosition = new Position();       // Endpos. in abs. Koordinaten

        Position mSpeeds = new Position();
        Position mReverseSpeeds = new Position();
        Double mRadius = 0;
        Double mRadIncr = 0;
        Double mAngIncr = 0;
        int mCircle = 0;
        int mIndex = 0;
        int mCount = 0;
        double mAngle = 0;

        Position mPreMove = new Position();
        Position mPostMove = new Position();
        IPositioner mPositioner; //Positioner for scan

        TiltCorrection mTilt;
        enuScannerErrors mStatus = 0;

        public ScannerArc(IPositioner _pos, Position _parameters,
            Position _speeds, Position _revspeeds, Position _premove, Position _postmove, TiltCorrection _tilt)
        {
            mPositioner = _pos;
            mRadius = _parameters.X;
            mRadIncr = _parameters.Y;
            mAngIncr = _parameters.Z;

            mSpeeds = _speeds;
            mReverseSpeeds = _revspeeds;
            mPreMove = _premove;
            mPostMove = _postmove;
            mTilt = _tilt;
        }

        public enuScannerErrors Initialize()
        {
            mStatus = 0;
            //todo: check parameters, reflect correctness in the status flag
            //store current absolute position as starting position and calculate end position
            mStartPosition = mPositioner.AbsolutePosition();
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
            mPositioner.RelativePosition(mPreMove); //todo: error check

            Position oldpos = mPositioner.AbsolutePosition();  //this is the current absolute position
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

        Position CalculateNextAbsolutePosition(Position _pos)
        {
            // We only keep the Z-component of the current position
            Position dest = _pos.Copy;

            if (mIndex < mCount)
            {
                mIndex++;
            }
            else if (mCircle < (mRadius / mRadIncr))
            {
                mCircle++;
                mAngle = 2 * Math.Asin(mAngIncr / (2 * mRadIncr * mCircle)); //in RADIAN
                mCount = Convert.ToInt16(Math.Abs(2 * Math.PI / mAngle));
                mIndex = 1;
            }
            else
            {
                return null;
            }

            // use X- and Y- component of start Position as offset
            dest.X = mStartPosition.X + mRadIncr * mCircle * Math.Cos(mAngle * mIndex);
            dest.Y = mStartPosition.Y + mRadIncr * mCircle * Math.Sin(mAngle * mIndex);
            return dest;
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

            mStatus = enuScannerErrors.NotInitialized;
            return mStatus;
        }


        public Position Position()
        {// Position relativ zur Startposition
            return mPositioner.AbsolutePosition().Delta(mStartPosition);
        }

 
        /*
          ' gets called from the Scanner object right after the tip has been moved to a new position
         ' gets current position can perform some preparation before an experiment is done (e.g.
         ' approaching the tip into feedback range).
         ' May change 'Position' and the caller has to re-position (but without calling the Hooks this
         ' time!). In this case FeedbackDestinationChanged must be set in the return value.
         ' May change the tip position directly with objPositioner. In this case FeedbackPositionChanged
         ' must be set in the return value.
         ' Returns an integer in which specific flags may be set.
 */
        /*
               enuFeedbackStatusFlags PostMovementHook(Position _currentposition, Position _lastposition)
               {
                   enuFeedbackStatusFlags res;
                   Position delta;
                   Position newpos;
                   LastErrMsg = "";
                   LastErr = 0;

                   if (Status.HasFlag(enuFeedbackStatusFlags.FeedbackControllerInvalid))
                   {
                       LastErrMsg = "FeedbackController is not configured in PreMovementHook().";
                       // Call objLogger.LogEvent(Me, "FeedbackController is not configured in PreMovementHook().", ll_error)
                       LastErr = 1;
                       return enuFeedbackStatusFlags.FeedbackAbort;
                   }
                   //the hooks are only reasonable in synchronous mode. In asynchronous mode the controller would
                   //correct the z position during any movement.
                   if (Timing != enuFeedbackTiming.FBSync)
                   {
                       //Call objLogger.LogEvent(Me, "PreMoveHook: Controller is not in synchronous mode - ignoring call to hook")
                       return enuFeedbackStatusFlags.FeedbackProceed;
                   }
                   if (mPositioner.RelativePosition(Settings.PreMovementHookChange) != enuPositionerStatus.Ready)
                   {
                       LastErrMsg = "Tip could not be re-positioned after ExitFeedback() in PreMovementHook().";
                       //Call objLogger.LogEvent(Me, "Tip could not be re-positioned after ExitFeedback() in PreMovementHook().", ll_error)
                       LastErr = 10;
                       return enuFeedbackStatusFlags.FeedbackAbort;     //Caller should abort
                   }
                   res = Approach();

                   newpos = mPositioner.AbsolutePosition(); //currentposition is absolute.
                   delta = newpos.Delta(_currentposition);
                   if ((delta.X != 0) || (delta.Y != 0) || (delta.Z != 0))
                   {
                       //FeedbackPositionChanged tells caller, that tip was moved.
                       res |= enuFeedbackStatusFlags.FeedbackPositionChanged;
                       //set new current position.
                       _currentposition = newpos;
                   }
                   return res;
               }


                      enuFeedbackStatusFlags PreMovementHook(Position _destination, Position _currentposition)
               {
                   //TODO: tranfer in to scanner!
                   /*' gets called from the Scanner object right before the tip is moved to a new position
                   ' gets current position and destination and can perform some preparation (e.g. withdrawing
                   ' the tip from the feedback).
                   ' May change destination and set FeedbackDestinationChanged in the return value, the caller
                   ' has to go the changed destination then.
                   ' May change the tip position directly with objPositioner. In this case FeedbackPositionChanged
                   ' must be set in the return value.
                   ' Returns an integer in which specific flags may be set.

               enuFeedbackStatusFlags res;
               Position delta;
               Position newpos;
               LastErrMsg = "";
                   LastErr = 0;

                   if (Status.HasFlag(enuFeedbackStatusFlags.FeedbackControllerInvalid))
                   {
                       LastErrMsg = "FeedbackController is not configured in PreMovementHook().";
                       // Call objLogger.LogEvent(Me, "FeedbackController is not configured in PreMovementHook().", ll_error)
                       LastErr = 1;
                       return enuFeedbackStatusFlags.FeedbackAbort;
                   }
                   //the hooks are only reasonable in synchronous mode. In asynchronous mode the controller would
                   //correct the z position during any movement.
                   if (Timing != enuFeedbackTiming.FBSync)
                   {
                       //Call objLogger.LogEvent(Me, "PreMoveHook: Controller is not in synchronous mode - ignoring call to hook")
                       return enuFeedbackStatusFlags.FeedbackProceed;
                   }

                   //exit feedback range if it was not disabled by the user.
                   if (Settings.ExitFeedback)
                   {
                       res = ExitFeedback();
                   }
                   else
                   {
                       res = enuFeedbackStatusFlags.FeedbackProceed;
                   }

                   if (res.HasFlag(enuFeedbackStatusFlags.FeedbackProceed))
                   {
                       //wenn erfolgreich, mPreMovementHookChange re-positionieren
                       if (mPositioner.RelativePosition(PreMovementHookChange) != enuPositionerStatus.Ready)
                       {
                           LastErrMsg = "Tip could not be re-positioned after ExitFeedback() in PreMovementHook().";
                           //Call objLogger.LogEvent(Me, "Tip could not be re-positioned after ExitFeedback() in PreMovementHook().", ll_error)
                           LastErr = 10;
                           return enuFeedbackStatusFlags.FeedbackAbort;     //Caller should abort
                       }
                   }

                   //did we change our position? Check the difference between currentposition and newpos.
                   newpos = mPositioner.AbsolutePosition(); //currentposition is absolute.
                   delta = newpos.Delta(_currentposition);
                   if ((delta.X != 0) || (delta.Y != 0) || (delta.Z != 0))
                   {
                       //FeedbackPositionChanged tells caller, that tip was moved.
                       res |= enuFeedbackStatusFlags.FeedbackPositionChanged;
                       //add our change to the destination position.
                       _destination = _destination.Sum(delta);
                   }
                   return res;
               }
           */




    }
}
