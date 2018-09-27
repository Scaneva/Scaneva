#region Copyright (C)
// ---------------------------------------------------------------------------------------------------------------
//  <copyright file="FeedbackController.cs" company="Scaneva">
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
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using Scaneva.Tools;

namespace Scaneva.Core
{
    public enum enuFeedbackMode
    {
        Absolute = 0,
        Relative = 1,
        Normalized = 2,
    }

    [Flags]
    public enum enuFeedbackStatusFlags
    {
        NotInitialized = 0,             // flags feedback controller as invalid / not configured
        Ready = 1,                      // the controller is properly configured, no HW errors or overloads - caller should go ahead

        InRange = 2,                    //  tip is in somewhere the feedback range 
        AtSetpoint = 4,                 //  tip is at the desired setpoint in the feedback range, tip is in somewhere the feedback range

        SensorError = 8,                // Sensor fault, caller should abort movement - fatal error condition
        LimitsExceeded = 16,            // Safety limits exceeded? (Sensor overload counts as well) caller should ask user if to abort movement
        PositionerError = 32,           // Positioner fault, caller should abort movement - fatal error condition

        Timeout = 64,                   //  Feedback Timeout
        Aborted = 128,                  //  Process aborted by user - can only be set from outside, has to be unset internally

    }

    //possible operations - only used for asynchronous mode. never set and used in synchronous mode.
    public enum enuFeedbackOperation
    {
        FBGotoSetpoint = 1,
        FBSearchFeedback = 2,
        FBExitFeedback = 4,
    }

    //in asychronous mode the controller depends on an external timer which periodically calls TimerEvent
    public enum enuFeedbackTiming
    {
        FBSync = 1,                     //synchronous / blocking mode
        FBAsync = 2,                    //asynchronous, non-blocking mode - relies on timer
    }

    public class FBPositionUpdatedEventArgs : EventArgs
    {
        public FBPositionUpdatedEventArgs(double position, double signal)
        {
            Position = position;
            Signal = signal;
        }

        public double Position { get; }
        public double Signal { get; }
    }

    class FeedbackController : ParametrizableObject
    {

        private Dictionary<string, IHWManager> hwStore;
        private Dictionary<string, TransducerChannel> transducerChannels;

        //***** internal state data comes here ******
        private enuFeedbackTiming mTiming = enuFeedbackTiming.FBAsync;
        private enuFeedbackStatusFlags mStatus = 0;
        private double SetPoint = 0;
        //current baseline
        private double BulkSignal = double.NaN;

        private PIDController PID;

        /// <summary>
        /// An event to notify about each movement step
        /// </summary>        
        public event EventHandler<FBPositionUpdatedEventArgs> FBPosotionUpdated;

        //Hardware
        private IPositioner mPositioner = null;
        private TransducerChannel mSensor = null;

        public FeedbackController(LogHelper log): base(log)
        {
            settings = new FeedbackControllerSettings();
        }

        public FeedbackControllerSettings Settings
        {
            get
            {
                return (FeedbackControllerSettings)settings;
            }
            set
            {
                settings = value;
            }
        }

        public Dictionary<string, IHWManager> HWStore
        {
            get
            {
                return hwStore;
            }
            set
            {
                var positioners = value.Where(x => typeof(IPositioner).IsAssignableFrom(x.Value.GetType())).Select(x => x);
                Settings.Positioners = positioners.ToDictionary(item => item.Key, item => (IPositioner)item.Value);

                transducerChannels = new Dictionary<string, TransducerChannel>();
                var transducers = value.Where(x => typeof(ITransducer).IsAssignableFrom(x.Value.GetType())).Select(x => x).ToArray();

                // Iterate Transducers
                foreach (KeyValuePair<string, IHWManager> ele in transducers)
                {
                    ITransducer ducer = (ITransducer)ele.Value;
                    // Iterate Channels
                    foreach (TransducerChannel chan in ducer.Channels)
                    {
                        string name = ele.Key + "." + chan.Name;
                        transducerChannels.Add(name, chan);
                    }
                }

                //Settings.TransducerChannels = transducerChannels.Select(d => d.Value).ToList();
                Settings.TransducerChannels = transducerChannels;

                //Settings.ListofTransducerChans = transducerChannels.Keys.ToArray();

                hwStore = value;

            }
        }

        /// <summary>
        /// Invoke the FBPosotionUpdated event
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnFBPosotionUpdated(FBPositionUpdatedEventArgs e)
        {
            FBPosotionUpdated?.Invoke(this, e);
        }

        public enuFeedbackStatusFlags Initialize()
        {
            mPositioner = Settings.Positioners[Settings.Positioner];
            if (mPositioner != null)
            {
                if (mPositioner.Status == enuPositionerStatus.Ready)
                {
                    mSensor = Settings.TransducerChannels[Settings.Channel];
                    if (mSensor != null)
                    {
                        if (mSensor.Status == enuSensorStatus.OK)
                        {
                            abortFlag = false;
                            PID = new PIDController(log);
                            PID.Settings = Settings.PIDController;
                            if (PID.Error == 0)
                            {
                                mStatus = enuFeedbackStatusFlags.Ready;
                            }
                            else
                            {
                                log.Add("Could not initialize PID controller, check PID controller settings!", "ERROR");
                            }
                        }
                        else
                        {
                            log.Add("Could not initialize sensor, check transudcer settings!", "ERROR");
                        }
                    }
                    else
                    {
                        log.Add("Could not initialize sensor, check feedbackcontroller settings!", "ERROR");
                    }
                }
                else
                {
                    log.Add("Could not initialize positioner, check positioner settings!", "ERROR");
                }
            }
            else
            {
                log.Add("Could not initialize positioner, check feedback controller settings!", "ERROR");
            }
            return mStatus;
        }

        //Performs status check of the involved HW, returns with adjusted flags state. Does not touch any flags related to its feedback state
        public enuFeedbackStatusFlags Status()
        {
            /*
            //first we consider the controller as configured
            mStatus |= enuFeedbackStatusFlags.OK;
            mStatus &= ~enuFeedbackStatusFlags.SensorError;
            mStatus &= ~enuFeedbackStatusFlags.PositionerError;

            // check the positioner
            if (mPositioner.Status.HasFlag(enuPositionerStatus.Error))
            {
                mStatus &= ~enuFeedbackStatusFlags.OK;
                mStatus |= enuFeedbackStatusFlags.PositionerError;
            }
            //check the sensor
            if (!mSensor.Status.HasFlag(enuSensorStatus.OK))
            {
                mStatus &= ~enuFeedbackStatusFlags.OK;
                mStatus |= enuFeedbackStatusFlags.SensorError;
            }
            */
            return mStatus;
        }

        enuFeedbackStatusFlags CheckFeedback(double signal)
        {
            mStatus &= ~enuFeedbackStatusFlags.InRange;
            mStatus &= ~enuFeedbackStatusFlags.AtSetpoint;

            if ((mSensor.Status.HasFlag(enuSensorStatus.Overload)) ||
                (signal < Settings.MinSafetyLimit) || (signal > Settings.MaxSafetyLimit))
            {
                mStatus |= enuFeedbackStatusFlags.LimitsExceeded;
            }

            double minchange = Math.Abs(BulkSignal * 0.02);  // 2%
            if (((SetPoint >= BulkSignal) && (signal >= (BulkSignal + minchange)))
                || ((SetPoint < BulkSignal) && (signal <= (BulkSignal - minchange))))
            {
                mStatus |= enuFeedbackStatusFlags.InRange;
            }

            if ((signal >= Settings.MinSetpoint) && (signal <= Settings.MaxSetpoint))
            {
                mStatus |= enuFeedbackStatusFlags.AtSetpoint;
            }

            return mStatus;
        }

        enuFeedbackStatusFlags AcquireBulksignal()
        {
            if (Status().HasFlag(enuFeedbackStatusFlags.Ready))
            {
                double bsum = 0;
                int i;
                for (i = 1; i <= Settings.BulkSamples; i++)
                {
                    bsum += mSensor.GetAveragedValue();
                }
                BulkSignal = (bsum / i);
            }
            else
            {
                log.Add("FeedbackController is not configured in AcquireBulksignal().", "ERROR");
            }
            return Status();
        }

        private bool abortFlag = false;

        public void Abort()
        {
            abortFlag = true;            
        }

        public enuFeedbackStatusFlags GoToSetpoint()
        {
            mTiming = enuFeedbackTiming.FBSync;
            if (!AcquireBulksignal().HasFlag(enuFeedbackStatusFlags.Ready))
            {
                log.Add("Error while acquiring bulksignal in GotoSetpoint().", "ERROR");
                mStatus |= enuFeedbackStatusFlags.Aborted;
                return Status();
            }

            if (BulkSignal == double.NaN)
            {
                log.Add("No bulksignal acquired before call to GotoSetpoint()?", "ERROR");
                mStatus |= enuFeedbackStatusFlags.Aborted;
                return Status();
            }

            Stopwatch stopwatch = new Stopwatch();

            if (!Status().HasFlag(enuFeedbackStatusFlags.Ready))
            {
                log.Add("FeedbackController is not configured in GotoSetpoint().", "ERROR");
                return Status();
            }

            if (mTiming != enuFeedbackTiming.FBSync)
            {
                log.Add("GotoSetpoint(): Controller in not in synchronous mode - aborting.");
                mStatus |= enuFeedbackStatusFlags.Aborted;
                return Status();
            }

            PID.Setpoint = ((Settings.MinSetpoint + Settings.MaxSetpoint) / 2);
            PID.Reset();

            do
            {
                if (abortFlag)
                {
                    mStatus |= enuFeedbackStatusFlags.Aborted;
                    log.Add("GotoSetpoint() aborted by user");
                    return Status();
                }
                mStatus = GotoSetpointStep(stopwatch.ElapsedMilliseconds);

                if (mStatus.HasFlag(enuFeedbackStatusFlags.AtSetpoint)
                    || mStatus.HasFlag(enuFeedbackStatusFlags.LimitsExceeded))
                {
                    return mStatus;
                }
                System.Threading.Thread.Sleep(Settings.LoopDelay); //todo: make it properly considering time passed
            } while (stopwatch.ElapsedMilliseconds < Settings.TimeOut);
            return enuFeedbackStatusFlags.Timeout;
        }

        private enuFeedbackStatusFlags GotoSetpointStep(long millis)
        {
            double signal = mSensor.GetAveragedValue();
            enuFeedbackStatusFlags stat = CheckFeedback(signal);

            OnFBPosotionUpdated(new FBPositionUpdatedEventArgs(mPositioner.AxisAbsolutePosition(enuAxes.ZAxis), signal));

            if (stat.HasFlag(enuFeedbackStatusFlags.LimitsExceeded))
            {
                log.Add("GotoSetpointStep(): Sensor response exceeded safety limits or sensor overload", "ERROR");
                return stat;
            }

            if (stat.HasFlag(enuFeedbackStatusFlags.Timeout))
            {
                log.Add("GotoSetpointStep(): Timeout", "ERROR");
                return enuFeedbackStatusFlags.Timeout;
            }

            if (stat.HasFlag(enuFeedbackStatusFlags.AtSetpoint))
            {
                return stat;
            }

            Position Correction = new Position();
            Correction.Z = PID.SimpleCorrection(signal);

            if (!mPositioner.RelativePosition(Correction).HasFlag(enuPositionerStatus.Ready))
            {
                return enuFeedbackStatusFlags.Aborted;
            }

            log.AddStatusUpdate(0, mPositioner.AbsolutePosition());

            return enuFeedbackStatusFlags.Ready;
        }
    }
}